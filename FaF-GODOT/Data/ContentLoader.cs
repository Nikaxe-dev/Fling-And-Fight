using System;
using System.Collections.Generic;
using System.Text.Json;
using FaF.Data.DataResources;
using FaF.Debug;
using Godot;

namespace FaF.Data;

public static class ContentLoader
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Data/ContentLoader");

    public static readonly string BUILTIN_DATA_PATH = "res://Data";
    public static readonly string UGC_DATA_PATH = "user://Data";

    private static readonly List<ModResource> Mods = [];

    private static readonly List<TShirtResource> TShirts = [];

    public static void ClearData()
    {
        Mods.Clear();

        TShirts.Clear();
    }

    private static void ExpectDir(string directory)
    {
        DirAccess dir = DirAccess.Open(directory);

        if (dir == null)
        {
            LOGGER.LOG(LogType.DEBUG, $"(ExpectDir) Attempted to create non-existant directory '{directory}' with result '{DirAccess.MakeDirAbsolute(directory)}'.");
        }
    }

    public static void LoadMod(string ModID)
    {
        LOGGER.LOG(LogType.INFO, $"Loading mod '{ModID}'", "ModLoader");

        // figure out whether it is a builtin or ugc mod
        DirAccess packRoot = DirAccess.Open(BUILTIN_DATA_PATH.PathJoin("Mods").PathJoin(ModID));
        packRoot ??= DirAccess.Open(UGC_DATA_PATH.PathJoin("Mods").PathJoin(ModID));

        if (packRoot == null)
        {
            LOGGER.LOG(LogType.ERROR, $"Mod with ID: '{ModID}' not found in builtin or ugc mod folder");
            return;
        }

        string fullPath = packRoot.GetCurrentDir();
        string metaPath = fullPath.PathJoin("ModMeta.json");

        if (!ResourceLoader.Exists(metaPath))
        {
            LOGGER.LOG(LogType.ERROR, $"Mod with ID: '{ModID}' failed to load: a ModMeta.json file was not found.");
            return;
        }

        var modResource = LoadModResource(metaPath);
        modResource.ID = ModID;
        modResource.NAMESPACE = "Mods";
        modResource.FILE_PATH = metaPath;
        modResource.FOLDER_PATH = fullPath;

        if (modResource.WorldType == WorldType.GodotScene)
        {
            modResource.Scene = ResourceLoader.Load<PackedScene>(fullPath.PathJoin("Data/World/GodotScene.tscn"));
        }

        Mods.Add(modResource);

        LoadContentFolder<TShirtResource>(ModID, fullPath.PathJoin("Data/Content/TShirts"), (resource, document) =>
        {
            resource.TorsoAsset = LoadAsset(document.RootElement.GetProperty("TorsoAsset").GetString());
        });

        LOGGER.LOG(LogType.INFO, $"Loaded mod '{ModID}'", "ModLoader");
    }

    public static void LoadData()
    {
        ExpectDir(UGC_DATA_PATH);
        ExpectDir(UGC_DATA_PATH.PathJoin("Mods"));

        foreach (string item in DirAccess.GetDirectoriesAt(BUILTIN_DATA_PATH.PathJoin("Mods"))) LoadMod(item.GetFile());
        foreach (string item in DirAccess.GetDirectoriesAt(UGC_DATA_PATH.PathJoin("Mods"))) LoadMod(item.GetFile());
    }

    private static Texture2D LoadAsset(string AssetID)
    {
        var split = AssetID.Split(":");
        
        string NAMESPACE = split[0];
        string ID = split[1];

        ModResource mod = GetMod(NAMESPACE);

        string assetPath = mod.FOLDER_PATH.PathJoin("Assets").PathJoin($"{ID}.png");
        return ResourceLoader.Load<Texture2D>(assetPath);
    }

    private static void LoadContentFolder<T>(string ModID, string directory, Action<T, JsonDocument> callback) where T : ContentResource, new()
    {
        if (DirAccess.DirExistsAbsolute(directory))
        {
            foreach (string contentMeta in DirAccess.GetFilesAt(directory))
            {
                if (contentMeta.GetExtension() == "json")
                {
                    T resource = new();
                    
                    FileAccess file = FileAccess.Open(directory.PathJoin(contentMeta), FileAccess.ModeFlags.Read);
                    JsonDocument document = JsonDocument.Parse(file.GetAsText());

                    LoadContentResource(document, resource);
                    
                    resource.ID = contentMeta.GetBaseName().GetFile();
                    resource.NAMESPACE = ModID;

                    resource.FILE_PATH = directory.PathJoin(contentMeta);
                    
                    callback(resource, document);

                    LOGGER.LOG(LogType.INFO, $"Loaded content {resource.FULL_ID} at {resource.FILE_PATH}", "ModContentLoader");
                }
            }
        }
    }

    private static void LoadContentResource(JsonDocument document, ContentResource resource)
    {
        resource.Name = document.RootElement.GetProperty("Name").GetString();
        resource.Description = document.RootElement.GetProperty("Description").GetString();

        resource.Creator = document.RootElement.GetProperty("Creator").GetString();
    }

    private static ModResource LoadModResource(string FilePath)
    {
        FileAccess file = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
        JsonDocument document = JsonDocument.Parse(file.GetAsText());

        ModResource resource = new();
        LoadContentResource(document, resource);

        if (document.RootElement.TryGetProperty("ShowInGame", out JsonElement ShowInGame))
        {
            resource.ShowInGame = ShowInGame.GetBoolean();
        }

        if (document.RootElement.TryGetProperty("WorldType", out JsonElement WorldTypeElement))
        {
            resource.WorldType = WorldTypeElement.GetString() switch
            {
                "GodotScene" => WorldType.GodotScene,
                _ => WorldType.NoWorld
            };
        }

        file.Close();
        file.Dispose();

        return resource;
    }

    public static ModResource GetMod(string FULL_ID) => Mods.Find(mod => mod.ID == FULL_ID);
    public static TShirtResource GetTShirt(string FULL_ID) => TShirts.Find(mod => mod.FULL_ID == FULL_ID);
}