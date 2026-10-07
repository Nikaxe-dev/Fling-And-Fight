using System.IO;
using FaF.Debug;
using FaF.FileSystem;
using FaF.Services.Content.Resources;
using FaF.Services.Modding.ResourceLoaders;
using Godot;

namespace FaF.Services.Modding;

public partial class ModService() : Service(["ContentService", "AssetService"])
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services/ModService");

    #region Constants

    public static readonly string WORLD_META_EXTENSION = "world";

    public static readonly string PROP_META_EXTENSION = "prop";
    public static readonly string GEAR_META_EXTENSION = "gear";

    public static readonly string MOD_DATA_PATH = "data";
    public static readonly string MOD_ASSETS_PATH = "assets";

    #endregion

    #region Mods

    private void LoadAssetsFolder(ModResource mod, FolderInstance assetsFolder, FolderInstance baseFolder)
    {
        foreach (FSInstance instance in assetsFolder)
            if (instance is FolderInstance folder)
                LoadAssetsFolder(mod, folder, baseFolder);
            else if (instance is FileInstance file && !file.IsGodotImportExtension())
            {
                string assetIdentifier = $"{mod.ID}:{Path.GetRelativePath(baseFolder.Path, file.BaseName)}";
                LOGGER.LOG(Enums.LogType.INFO, $"\t\t\t\t\t\\_ {assetIdentifier} -> {file}", "ModLoading");
                Game.AssetService.AddAssetPath(assetIdentifier, file.Path);
            }
    }

    private void LoadModAssetMappings(ModResource mod, FolderInstance modFolder)
    {
        if (modFolder.TryGetChild("assets", out FSInstance instance))
            if (instance is FolderInstance assetsFolder)
            {
                LOGGER.LOG(Enums.LogType.INFO, "\t\t\t\t\\_ assets:", "ModLoading");
                LoadAssetsFolder(mod, assetsFolder, assetsFolder);
            }
    }

    private void LoadDataFilesInFolder<FileLoader, ResourceType>(string extension, ModResource mod, FolderInstance dataFolder)
        where FileLoader : IContentResourceLoader<ResourceType>, new()
        where ResourceType : ContentResource, new()
    {
        foreach (FSInstance instance in dataFolder)
            if (instance is FolderInstance folder)
                LoadDataFilesInFolder<FileLoader, ResourceType>(extension, mod, folder);
            else if (instance is FileInstance file)
                if (file.IsJsonExtension() && file.GetExtension(0) == extension)
                {
                    ResourceType resource = FileLoader.Load(file, mod.ID);
                    LOGGER.LOG(Enums.LogType.INFO, $"\t\t\t\t\t\t\\_ {typeof(ResourceType).Name} {resource.FULL_ID}");

                    // all that just so I dont have to include an extra parameter
                    // shut up
                    Game.ContentService.Call($"Add{typeof(ResourceType).Name.Replace("Resource", "")}", mod.ID, resource);
                }
    }

    private void LoadDataFolder(ModResource mod, FolderInstance dataFolder)
    {
        LoadDataFilesInFolder<PropResourceLoader, PropResource>(PROP_META_EXTENSION, mod, dataFolder);
        LoadDataFilesInFolder<GearResourceLoader, GearResource>(GEAR_META_EXTENSION, mod, dataFolder);
    }

    private void LoadModData(ModResource mod, FolderInstance modFolder)
    {
        if (modFolder.TryGetChild("data", out FSInstance instance))
            if (instance is FolderInstance dataFolder)
            {
                LOGGER.LOG(Enums.LogType.INFO, "\t\t\t\t\\_ data:", "ModLoading");
                LoadDataFolder(mod, dataFolder);
            }
    }

    private void LoadModWorldMeta(FolderInstance modFolder)
    {
        foreach (FSInstance instance in modFolder)
            if (instance is FileInstance file)
                if (file.IsJsonExtension())
                    if (file.GetExtension(0) == WORLD_META_EXTENSION)
                    {
                        Game.ContentService.AddWorld(WorldResourceLoader.Load(file));
                        LOGGER.LOG(Enums.LogType.INFO, $"\t\t\t\t\\_ {file.FullName}", "ModLoading");
                    }
    }

    public void LoadMod(PackResource _, FolderInstance modFolder)
    {
        LOGGER.LOG(Enums.LogType.INFO, $"\t\t\t\\_ {modFolder.Name} @ {modFolder.Path}:", "ModLoading");

        ModResource mod = Game.ContentService.GetOrCreateMod(modFolder.Name);
        LoadModWorldMeta(modFolder);

        LoadModAssetMappings(mod, modFolder);
        LoadModData(mod, modFolder);
    }

    #endregion
}