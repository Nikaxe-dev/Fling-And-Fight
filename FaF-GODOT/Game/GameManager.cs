using System;
using FaF.Data;
using FaF.Data.RegistryObjects;
using FaF.Debug;
using FaF.Game.Networking;
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

    public static bool IS_IN_GAME {get; private set;} = false;
    public static string LOADED_WORLD {get; private set;}
    public static string GLOBAL_SERVER_ID {get; private set;} = "";

    public static int PLAYER_COUNT {get; private set;} = 0;

    private static bool has_server_communicated_info = false;

    // Static method bindings for GDScript
    
    public static bool GetIsInGame() => IS_IN_GAME;
    public static string GetLoadedWorld() => LOADED_WORLD;
    public static string GetGlobalServerID() => GLOBAL_SERVER_ID;
    public static int GetPlayerCount() => PLAYER_COUNT;

    public static WorldRegistry ContentLoaderGetWorld(string FULL_ID) => ContentLoader.GetWorld(FULL_ID);

    // HELPERS

    private static string GenerateGlobalServerID()
    {
        return ((int)GD.RandRange(11111111111111111111, 9999999999999999999)).ToString();
    }

    // SIGNALS

    [Signal] public delegate void JoinedServerEventHandler();
    [Signal] public delegate void SwitchedToTitleScreenEventHandler();

    public void SwitchToTitleScreen()
    {
        GetTree().ChangeSceneToFile("res://UI/Menus/Title/TitleScreen.tscn");
        EmitSignal(SignalName.SwitchedToTitleScreen);
    }


    /// <summary>
    /// Loads the world with no client or server specific loading.
    /// </summary>
    /// <param name="WorldID"></param>
    public void LoadWorld(string WorldID)
    {
        LOGGER.LOG(LogType.INFO, $"Loading the scene of world '{WorldID}'", "Worlds", true);

        WorldRegistry worldRegistry = ContentLoader.GetWorld(WorldID);
        if (worldRegistry != null)
        {
            GetTree().ChangeSceneToPacked(worldRegistry.Scene);
            LOADED_WORLD = WorldID;
            IS_IN_GAME = true;
        } else
        {
            LOGGER.LOG(LogType.ERROR, $"World '{WorldID}' does not exist.", "Worlds");
        }
    }

    private void OnPeerConnected(int ID)
    {
        if (Multiplayer.IsServer()) RpcId(ID, MethodName.CommunicateServerInfo, LOADED_WORLD, GLOBAL_SERVER_ID);
        if (ID != 1) PLAYER_COUNT += 1;
    }

    private void OnPeerDisconnected(int ID)
    {
        if (ID != 1) PLAYER_COUNT -= 1;
    }

    /// <summary>
    /// Starts a new FaF server, loading the given world and creating a ENetMultiplayerPeer server.
    /// </summary>
    /// <param name="WorldID"></param>
    /// <param name="PORT"></param>
    public void CreateServer(string WorldID, int PORT = 56565)
    {
        GLOBAL_SERVER_ID = GenerateGlobalServerID();

        NetworkManager.Instance.StartServer();
        LoadWorld(WorldID);

        PLAYER_COUNT = 0;

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

        PLAYER_COUNT = 0;
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
        NetworkManager.Instance.StartClient(IP_ADDRESS, PORT);

        PLAYER_COUNT = 1;

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

        PLAYER_COUNT = 0;
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void CommunicateServerInfo(string WorldID, string GlobalServerID)
    {
        if (Multiplayer.IsServer()) return;
        if (has_server_communicated_info) LOGGER.LOG(LogType.WARNING, "CommunicateServerInfo() ignored: It has already been run", "ServerClientCommunication");

        has_server_communicated_info = true;

        LoadWorld(WorldID);
        GLOBAL_SERVER_ID = GlobalServerID;

        EmitSignal(SignalName.JoinedServer);

        LOGGER.LOG(LogType.INFO, $"Server communicated info to client (WorldID: {WorldID}, GlobalServerID: {GlobalServerID})", "ServerClientCommunication");
    }
}