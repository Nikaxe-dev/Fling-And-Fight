using FaF.Debug;
using Godot;

namespace FaF.Services.World;

public partial class WorldService() : Service([])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.World;

    #region Resources
    
    private static readonly PackedScene WorldRootScene = ResourceLoader.Load<PackedScene>("res://Core/Services/World/WorldRoot.tscn");

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
        WorldLoaded += worldID => LOGGER.IMPORTANT($"Loaded map of world '{worldID}'");
        WorldRootLoaded += () => LOGGER.IMPORTANT("Loaded world root");
    }

    protected override void _LoadService()
    {
        base._LoadService();
        SetupWorldLogs();
    }

    #endregion
}