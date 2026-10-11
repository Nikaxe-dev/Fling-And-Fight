using System;
using FaF.Core.Debug;
using FaF.Core.Services.Content.Resources;
using Godot;

namespace FaF.Core.Services.World;

public partial class WorldService() : Service(["RunService"])
{
    private static readonly Debug.Logger LOGGER = Loggers.World;

    #region Resources
    
    private static readonly PackedScene WorldRootScene = ResourceLoader.Load<PackedScene>("res://Core/Services/World/WorldRoot.tscn");

    #endregion

    #region Signals

    [Signal] public delegate void WorldLoadedEventHandler(string worldID);
    [Signal] public delegate void WorldRootLoadedEventHandler();

    #endregion

    #region State

    public WorldRoot WorldRoot {get; private set;}
    public Node MapRoot {get; private set;}
    public string LoadedWorldID {get; private set;}

    private string ServerInfoWorldID = null;
    
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
        try {
            WorldResource world = Game.ContentService.GetWorld(worldID);
            MapRoot = world.WorldMapScene.Instantiate();
            WorldRoot.AddChild(MapRoot);

            LoadedWorldID = worldID;
            EmitSignal(SignalName.WorldLoaded, worldID);
        } catch (Exception exception)
        {
            // handle exception by stopping the current game session

            LOGGER.ERROR($"Encountered exception while loading the map of world '{worldID}':\n{exception.Message}");
            Game.RunService.StopSession(22);
        }
    }

    #endregion

    #region LoadService

    private void SetupWorldLogs()
    {
        WorldLoaded += worldID => LOGGER.IMPORTANT($"Loaded map of world '{worldID}'");
        WorldRootLoaded += () => LOGGER.IMPORTANT("Loaded world root");
    }

    private void LoadClient()
    {
        Game.RunService.ClientStarting += (_,_) =>
        {
            ServerInfoWorldID = null;
            LoadWorldRoot();

            if (ServerInfoWorldID != null && MapRoot == null)
                LoadWorldMap(ServerInfoWorldID);
        };

        Game.NetworkService.ServerInfoCommunicatedToClient += info =>
        {
            ServerInfoWorldID = info["WorldID"].AsString();
            
            if (WorldRoot != null && MapRoot == null)
                LoadWorldMap(ServerInfoWorldID);
        };
    }

    private void LoadServer()
    {
        Game.RunService.ServerStarted += (worldID, _) =>
        {
            LoadWorldRoot();
            LoadWorldMap(worldID);
        };

        Game.NetworkService.ServerInfoConstructed += info => info["WorldID"] = LoadedWorldID;
    }

    protected override void _LoadService()
    {
        base._LoadService();
        SetupWorldLogs();

        LoadClient();
        LoadServer();
    }

    #endregion
}