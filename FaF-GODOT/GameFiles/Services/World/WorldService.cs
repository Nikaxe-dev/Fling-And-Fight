using FaF.Debug;
using Godot;

namespace FaF.Services.World;

public partial class WorldService() : Service([])
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services/WorldService");

    #region Resources
    
    private static readonly PackedScene WorldRootScene = ResourceLoader.Load<PackedScene>("res://GameFiles/Services/World/WorldRoot.tscn");

    #endregion

    #region Signals

    [Signal] public delegate void WorldLoadedEventHandler(string worldID);
    [Signal] public delegate void WorldRootLoadedEventHandler();

    #endregion

    #region State

    public WorldRoot WorldRoot {get; private set;}
    public string LoadedWorldID {get; private set;}
    
    #endregion

    #region World Loading
    
    public void LoadWorldRoot()
    {
        WorldRoot = WorldRootScene.Instantiate<WorldRoot>();
        GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToNode, WorldRoot);

        EmitSignal(SignalName.WorldRootLoaded);
    }

    public void LoadWorldMap(string worldID)
    {
        LoadedWorldID = worldID;
        EmitSignal(SignalName.WorldLoaded, worldID);
    }

    #endregion

    #region LoadService

    private void SetupWorldLogs()
    {
        WorldLoaded += worldID => LOGGER.LOG(Enums.LogType.INFO, $"Loaded map of world '{worldID}'", "WorldLoading", true);
        WorldRootLoaded += () => LOGGER.LOG(Enums.LogType.INFO, "Loaded world root", "WorldLoading", true);
    }

    protected override void _LoadService()
    {
        base._LoadService();
        SetupWorldLogs();
    }

    #endregion
}