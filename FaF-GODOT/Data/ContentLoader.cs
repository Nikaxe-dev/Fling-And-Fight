using System;
using System.Collections.Generic;
using FaF.Data.RegistryObjects;
using FaF.Debug;
using Godot;

namespace FaF.Data;

/// <summary>
/// FaF content loader. Contains all functions related to loading registries.
/// </summary>
public static class ContentLoader
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Data/ContentLoader");

    public static readonly string BUILTIN_DATA_PATH = "res://Data";
    public static readonly string UGC_DATA_PATH = "user://Data"; // OS.GetExecutablePath().GetBaseDir().PathJoin("Data");

    private static readonly List<WorldRegistry> Worlds = [];

    // ItemLike
    private static readonly List<PropRegistry> Props = [];
    private static readonly List<GearRegistry> Gears = [];

    // AvatarItems
    private static readonly List<TShirtRegistry> TShirts = [];
    private static readonly List<ShirtRegistry> Shirts = [];
    private static readonly List<PantsRegistry> Pants = [];
    private static readonly List<AccessoryRegistry> Accessories = [];

    // Misc
    
    private static readonly List<SongRegistry> Songs = [];

    private static readonly List<WorldEventRegistry> WorldEvents = [];

    private static readonly List<GrablineShapeRegistry> GrablineShapes = [];
    private static readonly List<GrablineSkinRegistry> GrablineSkins = [];

    /// <summary>
    /// Clears all of the registries with the exception of Worlds.
    /// </summary>
    public static void ClearRegistries()
    {
        Props.Clear();
        Gears.Clear();

        TShirts.Clear();
        Shirts.Clear();
        Pants.Clear();
        Accessories.Clear();

        Songs.Clear();

        WorldEvents.Clear();

        GrablineShapes.Clear();
        GrablineSkins.Clear();
    }

    private static void ExpectDir(string directory)
    {
        DirAccess dir = DirAccess.Open(directory);

        if (dir == null)
        {
            LOGGER.LOG(LogType.DEBUG, $"(ExpectDir) Attempted to create non-existant directory '{directory}' with result '{DirAccess.MakeDirAbsolute(directory)}'.");
        }
    }

    /// <summary>
    /// Loads a worlds data. NOTE: This means it loads the props, gears, avatar items, ect. NOT THE MAP.
    /// </summary>
    public static void LoadWorldData(string WorldID)
    {
        LOGGER.LOG(LogType.INFO, $"Loading world '{WorldID}'", "WorldDataLoader");

        DirAccess packRoot = DirAccess.Open(BUILTIN_DATA_PATH.PathJoin("Worlds").PathJoin(WorldID));
        packRoot ??= DirAccess.Open(UGC_DATA_PATH.PathJoin("Worlds").PathJoin(WorldID));

        if (packRoot == null) LOGGER.LOG(LogType.ERROR, $"World with ID: '{WorldID}' not found in res:// or user://", "WorldDataLoader"); else {
            // World exists: CONTINUE LOADING !!

            string fullPath = packRoot.GetCurrentDir();

            if (!ResourceLoader.Exists(fullPath.PathJoin("WorldMeta.tres"))) LOGGER.LOG(LogType.ERROR, $"World with ID: '{WorldID}' does not include the required WorldMeta.tres WorldRegistry Resource file", "WorldDataLoader"); else
            {
                // Worlds.Add((WorldRegistry)ResourceLoader.Load(fullPath.PathJoin("WorldMeta.tres")));
                
                LOGGER.LOG(LogType.INFO, "Loading Props", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/Props"), "PropMeta.tres", Props);

                LOGGER.LOG(LogType.INFO, "Loading Gears", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/Gears"), "GearMeta.tres", Gears);


                LOGGER.LOG(LogType.INFO, "Loading TShirts", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/TShirts"), "TShirtMeta.tres", TShirts);

                LOGGER.LOG(LogType.INFO, "Loading Shirts", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/Shirts"), "ShirtMeta.tres", Shirts);

                LOGGER.LOG(LogType.INFO, "Loading Pants", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/Pants"), "PantsMeta.tres", Pants);

                LOGGER.LOG(LogType.INFO, "Loading Accessories", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/Accessories"), "AccessoryMeta.tres", Accessories);


                LOGGER.LOG(LogType.INFO, "Loading GrablineSkins", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/GrablineSkins"), "GrablineSkinMeta.tres", GrablineSkins);

                LOGGER.LOG(LogType.INFO, "Loading GrablineShapes", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/GrablineShapes"), "GrablineShapeMeta.tres", GrablineShapes);


                LOGGER.LOG(LogType.INFO, "Loading Songs", "WorldDataLoader");
                LoadRegistryFolder(WorldID, fullPath.PathJoin("Data/AvatarItems/Music"), "SongMeta.tres", Songs);


                LOGGER.LOG(LogType.INFO, $"World '{WorldID}' successfully loaded", "WorldDataLoader");
            }
        }
    }

    public static void LoadWorlds()
    {
        ExpectDir(UGC_DATA_PATH);
        ExpectDir(UGC_DATA_PATH.PathJoin("Worlds"));

        // Load Built in data then user defined data.
        foreach (string item in DirAccess.GetDirectoriesAt(BUILTIN_DATA_PATH.PathJoin("Worlds"))) LoadWorldData(item.GetFile());
        foreach (string item in DirAccess.GetDirectoriesAt(UGC_DATA_PATH.PathJoin("Worlds"))) LoadWorldData(item.GetFile());
    }

    public static void LoadWorldRegistryFolder()
    {
        LOGGER.LOG(LogType.INFO, "Loading all world registries", "WorldLoader");
        LoadRegistryFolder("Worlds", BUILTIN_DATA_PATH.PathJoin("Worlds"), "WorldMeta.tres", Worlds);
        LoadRegistryFolder("Worlds", UGC_DATA_PATH.PathJoin("Worlds"), "WorldMeta.tres", Worlds);
    }

    private static void LoadRegistryFolder<T>(string WorldID, string directory, string metaResourceFile, List<T> listOfContent) where T : Registry
    {
        if (DirAccess.DirExistsAbsolute(directory))
        {
            foreach (string item in DirAccess.GetDirectoriesAt(directory))
            {
                string metaPath = directory.PathJoin(item.PathJoin(metaResourceFile));
                if (ResourceLoader.Exists(metaPath))
                {
                    var registry = ResourceLoader.Load(metaPath) as T;
                    registry.ID = item.GetFile();
                    registry.NAMESPACE = WorldID;

                    registry.FILE_PATH = metaPath;

                    listOfContent.Add(registry);

                    LOGGER.LOG(LogType.INFO, $"{registry.FULL_ID} resource loaded", "RegistryFolderLoading");
                } else
                {
                    LOGGER.LOG(LogType.ERROR, $"{WorldID}:{item.GetFile()} failed to load: {metaPath} doesnt exist", "RegistryFolderLoading");
                }
            }
        }
    }

    public static WorldRegistry GetWorld(string FULL_ID)
    {
        return Worlds.Find(registry => registry.ID == FULL_ID);
    }

    public static PropRegistry GetProp(string FULL_ID)
    {
        return Props.Find(registry => registry.FULL_ID == FULL_ID);
    }

    public static GearRegistry GetGear(string FULL_ID)
    {
        return Gears.Find(registry => registry.FULL_ID == FULL_ID);
    }


    public static TShirtRegistry GetTShirt(string FULL_ID)
    {
        return TShirts.Find(registry => registry.FULL_ID == FULL_ID);
    }

    public static ShirtRegistry GetShirt(string FULL_ID)
    {
        return Shirts.Find(registry => registry.FULL_ID == FULL_ID);
    }

    public static PantsRegistry GetPants(string FULL_ID)
    {
        return Pants.Find(registry => registry.FULL_ID == FULL_ID);
    }

    public static AccessoryRegistry GetAccessory(string FULL_ID)
    {
        return Accessories.Find(registry => registry.FULL_ID == FULL_ID);
    }

    
    public static GrablineSkinRegistry GetGrablineSkin(string FULL_ID)
    {
        return GrablineSkins.Find(registry => registry.FULL_ID == FULL_ID);
    }

    public static GrablineShapeRegistry GetGrablineShape(string FULL_ID)
    {
        return GrablineShapes.Find(registry => registry.FULL_ID == FULL_ID);
    }


    public static SongRegistry GetSong(string FULL_ID)
    {
        return Songs.Find(registry => registry.FULL_ID == FULL_ID);
    }
}