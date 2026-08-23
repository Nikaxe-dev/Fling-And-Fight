using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using FaF.Data;
using FaF.Data.RegistryObjects;
using FaF.Debug;
using FaF.Editor.DataModel;
using FaF.Game;
using Godot;

namespace FaF.Editor;

public partial class EditorRoot : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Editor/Root");
    public static EditorRoot Instance {private set; get;}

    public World MapRoot {private set; get;}

    public string CurrentWorkspaceWorldID {private set; get;}
    public string CurrentWorkspaceDirectory {private set; get;}
    public string CurrentWorkspaceMapFileDirectory {private set; get;}

    public List<Instance> RootInstances = [];

    public static readonly string Sample_Path = "res://Editor/Samples";
    public static readonly string Map_File_Extension = "json";
    public static readonly string Map_DOC_TYPE = "FaFMap";
    public static readonly int Map_DOC_VER = 1;

    public void LoadWorld(string WorldID = "FaF", string SampleName = "Baseplate")
    {
        LOGGER.LOG(LogType.INFO, $"Loading world {WorldID} in the editor.", "WorldLoading", true);

        WorldRegistry worldRegistry = ContentLoader.GetWorld(WorldID);

        if (worldRegistry == null) {LOGGER.LOG(LogType.ERROR, $"Attempting to load world '{WorldID}' in the editor, this world was not found.", "WorldLoading", true); return;}

        // the world directory (like res://Data/Worlds/FaF/ or user://Data/Worlds/UGCWorld)
        string worldDirectory = worldRegistry.FILE_PATH.GetBaseDir();
        string mapFileDirectory = worldDirectory.PathJoin($"map.{Map_File_Extension}");

        // create new map file with the contents of one of the samples
        if (!FileAccess.FileExists(mapFileDirectory))
        {
            LOGGER.LOG(LogType.INFO, $"Map file for world {WorldID} not found, creating map file with sample {SampleName}.");
            
            var writeFile = FileAccess.Open(mapFileDirectory, FileAccess.ModeFlags.Write);
            var templateFile = FileAccess.Open(Sample_Path.PathJoin($"{SampleName}.{Map_File_Extension}"), FileAccess.ModeFlags.Read);

            writeFile.StoreString(templateFile.GetAsText());

            // dispose of both files to avoid memory leaks
            writeFile.Close();
            writeFile.Dispose();

            templateFile.Close();
            templateFile.Dispose();
        }

        var file = FileAccess.Open(mapFileDirectory, FileAccess.ModeFlags.Read);
        var document = JsonDocument.Parse(file.GetAsText());

        string documentType = document.RootElement.GetProperty("DOC_TYPE").GetString();

        if (documentType != Map_DOC_TYPE) {LOGGER.LOG(LogType.ERROR, $"Attempted to load world '{WorldID}' in the editor, but the map file has a document type of '{documentType}' instead of '{Map_DOC_TYPE}'.", "WorldLoading", true); return;}

        RootInstances = [];
        
        foreach (JsonElement rootInstanceJSON in document.RootElement.GetProperty("Instances").EnumerateArray())
        {
            var rootInstance = EditorJSON.FromJson<Instance>(rootInstanceJSON);
            LOGGER.LOG(LogType.INFO, $"Loading Root '{rootInstance.ClassName}' Instance '{rootInstance.Name}'.", "WorldLoading");

            RootInstances.Add(rootInstance);

            if (rootInstance is World rootWorld)
            {
                MapRoot = rootWorld;
            }
        }

        // dispose file to avoid memory leaks
        file.Close();
        file.Dispose();

        // set CurrentWorkspace properties
        CurrentWorkspaceWorldID = WorldID;
        CurrentWorkspaceDirectory = worldDirectory;
        CurrentWorkspaceMapFileDirectory = mapFileDirectory;

        LOGGER.LOG(LogType.INFO, $"Successfully loaded world {WorldID} in the editor.", "WorldLoading", true);
    }

    public void Save()
    {
        LOGGER.LOG(LogType.INFO, $"Saving world '{CurrentWorkspaceWorldID}' in the editor to '{CurrentWorkspaceMapFileDirectory}'.", "WorldSaving", true);

        JsonObject ROOT = new()
        {
            ["DOC_TYPE"] = Map_DOC_TYPE,
            ["DOC_VER"] = Map_DOC_VER,
        };

        JsonArray instances = [];
        
        foreach (Instance rootInstance in RootInstances)
        {
            LOGGER.LOG(LogType.INFO, $"Saving Root '{rootInstance.ClassName}' Instance '{rootInstance.Name}'.", "WorldSaving");
            instances.Add(EditorJSON.ToJson(rootInstance));
        }

        ROOT.Add("Instances", instances);

        var file = FileAccess.Open(CurrentWorkspaceMapFileDirectory, FileAccess.ModeFlags.Write);
        file.StoreString(ROOT.ToJsonString());

        // dispose file to avioud memory leaks
        file.Close();
        file.Dispose();

        LOGGER.LOG(LogType.INFO, $"Finished saving world '{CurrentWorkspaceWorldID}' in the editor.", "WorldSaving", true);
    }

    public override void _Ready()
    {
        Instance = this;

        LoadWorld("New_Sedes");

        GameManager.Instance.EmitSignal(GameManager.SignalName.FaFEditorLoaded);
    }
}
