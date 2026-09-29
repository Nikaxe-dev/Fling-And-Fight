using FaF.Debug;
using Godot;

namespace FaF.Services.World;

public partial class WorldService : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services/WorldService");

    #region Resources
    
    private static readonly PackedScene WorldRootScene = ResourceLoader.Load<PackedScene>("res://GameFiles/Services/World/WorldRoot.tscn");

    #endregion

    #region Signals

    [Signal] public delegate void WorldLoadedEventHandler();
    [Signal] public delegate void WorldRootLoadedEventHandler();

    #endregion

    #region State

    public WorldRoot WorldRoot {get; private set;}
    
    #endregion

    #region World Loading
    
    public void LoadWorldRoot()
    {
        LOGGER.LOG(Enums.LogType.INFO, "Loading world root", "WorldLoading", true);

        WorldRoot = WorldRootScene.Instantiate<WorldRoot>();
        GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToNode, WorldRoot);

        EmitSignal(SignalName.WorldRootLoaded);
    }

    public void LoadWorldMap(string worldID)
    {
        LOGGER.LOG(Enums.LogType.INFO, $"Loading world '{worldID}'", "WorldLoading", true);

        EmitSignal(SignalName.WorldLoaded);
    }

    #endregion
}