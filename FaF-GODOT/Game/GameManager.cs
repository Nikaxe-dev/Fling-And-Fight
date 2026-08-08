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

    public static bool IS_IN_GAME = false;
    public static string LOADED_WORLD {get; private set;}

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
        if (Multiplayer.IsServer()) RpcId(ID, MethodName.TransmitLoadedWorldToClient, LOADED_WORLD);
    }

    /// <summary>
    /// Starts a new FaF server, loading the given world and creating a ENetMultiplayerPeer server.
    /// </summary>
    /// <param name="WorldID"></param>
    /// <param name="PORT"></param>
    public void CreateServer(string WorldID, int PORT = 56565)
    {
        NetworkManager.Instance.StartServer();
        LoadWorld(WorldID);

        NetworkManager.Instance.PlayerConnected += OnPeerConnected;
    }

    /// <summary>
    /// Stops the server and disconnects any connections.
    /// </summary>
    public void StopServer()
    {
        NetworkManager.Instance.PlayerConnected -= OnPeerConnected;

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
        NetworkManager.Instance.StartClient(IP_ADDRESS, PORT);
    }

    /// <summary>
    /// Stops the client and disconnects any connections. <br/>
    /// Also switches scenes to the title screen.
    /// </summary>
    public void StopClient()
    {
        NetworkManager.Instance.StopClient();
        IS_IN_GAME = false;
        GetTree().ChangeSceneToFile("res://UI/Menus/Title/TitleScreen.tscn");
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void TransmitLoadedWorldToClient(string WorldID)
    {
        if (!Multiplayer.IsServer()) LoadWorld(WorldID);
    }
}