using System;
using System.Threading.Tasks;
using FaF.Data;
using FaF.Data.RegistryObjects;
using FaF.Debug;
using FaF.Game.Networking;
using FaF.Game.World;
using Godot;

namespace FaF.Game;

/// <summary>
/// Singleton controlling the global game state and providing functions to load worlds, saves, ect. <br/>
/// <br/>
/// To create a server: <br/>
/// 1. Start the server on the network through <c>NetworkManager.StartServer(int PORT)</c> <br/>
/// 2. Load the given world. <br/>
/// <br/>
/// Whenever a peer connects, send a message (through RPC) to that peer with the ID of the world to load. <br/>
/// <br/>
/// To create a client: <br/>
/// 1. Start the client on the network through <c>NetworkManager.StartClient(string IP, int PORT)</c> <br/>
/// 2. Nothing else, wait until TransmitLoadedWorldToClient() is called. <br/>
/// <br/>
/// On TransmitLoadedWorldToClient(string WorldID), load the given WorldID.
/// </summary>
public sealed partial class GameManager : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("GameManager");

    public static GameManager Instance {get; private set;}

    public override void _Ready()
    {
        Instance = this;

        ContentLoader.LoadWorldRegistryFolder();
        ContentLoader.LoadWorlds();
    }

    // GAMESTATE

    public static bool IS_IN_GAME {get; private set;} = false;
    public static string LOADED_WORLD {get; private set;}
    public static string GLOBAL_SERVER_ID {get; private set;} = "";

    public static Node GAME_ROOT {get; private set;}
    public static WorldRoot WORLD_ROOT {get; private set;}

    public static bool has_server_communicated_info {get; private set;} = false;

    // CONSTANT

    private static readonly PackedScene RESOURCE_GAME_ROOT = ResourceLoader.Load<PackedScene>("res://Game/World/GameRoot.tscn");

    // Static method bindings for GDScript
    
    public static bool GetIsInGame() => IS_IN_GAME;
    public static string GetLoadedWorld() => LOADED_WORLD;
    public static string GetGlobalServerID() => GLOBAL_SERVER_ID;

    public static WorldRegistry ContentLoaderGetWorld(string FullID) => ContentLoader.GetWorld(FullID);

    // HELPERS

    private static string GenerateGlobalServerID()
    {
        return ((int)GD.RandRange(11111111111111111111, 9999999999999999999)).ToString();
    }

    // SIGNALS

    [Signal] public delegate void JoinedServerEventHandler();
    [Signal] public delegate void SwitchedToTitleScreenEventHandler();
    [Signal] public delegate void ClientPeerLoadedWorldEventHandler(int PeerID);

    public void SwitchToTitleScreen()
    {
        GetTree().ChangeSceneToFile("res://UI/Menus/Title/TitleScreen.tscn");
        EmitSignal(SignalName.SwitchedToTitleScreen);
    }


    /// <summary>
    /// Loads the world with no client or server specific loading.
    /// </summary>
    /// <param name="WorldID"></param>
    private static void LoadWorld(string WorldID)
    {
        LOGGER.LOG(LogType.INFO, $"Loading the scene of world '{WorldID}'", "Worlds", true);

        WorldRegistry worldRegistry = ContentLoader.GetWorld(WorldID);
        if (worldRegistry != null)
        {
            // GetTree().ChangeSceneToPacked(worldRegistry.Scene);
            var world = worldRegistry.Scene.Instantiate();
            world.Name = "WorldRoot";

            if (world is WorldRoot newWorldRoot) WORLD_ROOT = newWorldRoot;

            GAME_ROOT.AddChild(world);
            
            LOADED_WORLD = WorldID;
            IS_IN_GAME = true;
        } else
        {
            LOGGER.LOG(LogType.ERROR, $"World '{WorldID}' does not exist.", "Worlds");
        }
    }

    private void LoadGameRoot()
    {
        // GetTree().ChangeSceneToFile("res://Game/World/GameRoot.tscn");
        // GAME_ROOT = GetTree().CurrentScene;

        GetTree().CurrentScene?.QueueFree();
        GAME_ROOT = RESOURCE_GAME_ROOT.Instantiate();
        GetTree().Root.AddChild(GAME_ROOT);
        GetTree().CurrentScene = GAME_ROOT;
    }

    private void OnPeerConnected(int ID)
    {
        if (Multiplayer.IsServer()) RpcId(ID, MethodName.CommunicateServerInfo, LOADED_WORLD, GLOBAL_SERVER_ID);
    }

    private void OnPeerDisconnected(int ID)
    {
        
    }

    /// <summary>
    /// Starts a new FaF server, loading the given world and creating a ENetMultiplayerPeer server.
    /// </summary>
    /// <param name="WorldID"></param>
    /// <param name="PORT"></param>
    public void CreateServer(string WorldID, int PORT = 56565)
    {
        GLOBAL_SERVER_ID = GenerateGlobalServerID();

        LoadGameRoot();
        LoadWorld(WorldID);
        
        NetworkManager.Instance.StartServer();

        NetworkManager.Instance.PlayerConnected += OnPeerConnected;
        NetworkManager.Instance.PlayerDisconnected += OnPeerDisconnected;
    }

    /// <summary>
    /// Stops the server and disconnects any connections.
    /// </summary>
    public void StopServer()
    {
        NetworkManager.Instance.PlayerConnected -= OnPeerConnected;
        NetworkManager.Instance.PlayerDisconnected -= OnPeerDisconnected;

        NetworkManager.Instance.StopServer();
        IS_IN_GAME = false;
        GetTree().Quit();
    }

    /// <summary>
    /// Starts the FaF client, for now just simply running StartClient() on the network manager. <br/>
    /// The method TransmitLoadedWorldToClient() loads the world later.
    /// </summary>
    /// <param name="IP_ADDRESS"></param>
    /// <param name="PORT"></param>
    public void CreateClient(string IP_ADDRESS = "127.0.0.1", int PORT = 56565)
    {
        has_server_communicated_info = false;

        LoadGameRoot();

        NetworkManager.Instance.StartClient(IP_ADDRESS, PORT);

        NetworkManager.Instance.PlayerConnected += OnPeerConnected;
        NetworkManager.Instance.PlayerDisconnected += OnPeerDisconnected;
    }

    /// <summary>
    /// Stops the client and disconnects any connections. <br/>
    /// Also switches scenes to the title screen.
    /// </summary>
    public void StopClient()
    {
        NetworkManager.Instance.PlayerConnected -= OnPeerConnected;
        NetworkManager.Instance.PlayerDisconnected -= OnPeerDisconnected;

        NetworkManager.Instance.StopClient();
        IS_IN_GAME = false;
        has_server_communicated_info = false;
        
        SwitchToTitleScreen();
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void CommunicateServerInfo(string WorldID, string GlobalServerID)
    {
        if (Multiplayer.IsServer()) return;
        if (has_server_communicated_info) LOGGER.LOG(LogType.WARNING, "CommunicateServerInfo() ignored: It has already been run", "ServerClientCommunication");
        LOGGER.LOG(LogType.INFO, $"Server communicated info to client (WorldID: {WorldID}, GlobalServerID: {GlobalServerID})", "ServerClientCommunication", true);

        has_server_communicated_info = true;

        LoadWorld(WorldID);
        GLOBAL_SERVER_ID = GlobalServerID;

        EmitSignal(SignalName.JoinedServer);
    }
}