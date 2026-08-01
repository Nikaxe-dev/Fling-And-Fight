using System;
using FaF.Debug;
using FaF.Game.Players;
using Godot;

namespace FaF.Game.Networking;

public partial class NetworkManager : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Networking/NetworkManager");

    // CODE CONFIGURATION

    /// <summary>
    /// <b>COMMON</b> -
    /// Emitted when a player connects to the server.
    /// </summary>
    /// <param name="PEER_ID"></param>
    [Signal] public delegate void PlayerConnectedEventHandler(int PEER_ID);

    /// <summary>
    /// <b>COMMON</b> -
    /// Emitted when a player disconnects from the server.
    /// </summary>
    /// <param name="PEER_ID"></param>
    [Signal] public delegate void PlayerDisconnectedEventHandler(int PEER_ID);

    /// <summary>
    /// <b>CLIENT</b> -
    /// Emitted when the client connects to the server.
    /// </summary>
    [Signal] public delegate void ConnectionEstablishedEventHandler();

    /// <summary>
    /// <b>CLIENT</b> -
    /// Emitted when the client disconnects from the server.
    /// </summary>
    [Signal] public delegate void ConnectionFailedEventHandler();

    /// <summary>
    /// <b>COMMON</b> -
    /// Emitted when the game enters a world (LOADING THE SCENE)
    /// </summary>
    [Signal] public delegate void WorldEnteredEventHandler();

    /// <summary>
    /// <b>COMMON</b> -
    /// Emitted when the game exits a world (UNLOADING THE SCENE TO THE MAIN MENU / PROGRAM END)
    /// </summary>
    [Signal] public delegate void WorldExitedEventHandler();

    public static readonly string DEFAULT_IP = "127.0.0.1";
    public static readonly int DEFAULT_PORT = 56565;

    public static readonly int DEFAULT_MAX_PLAYERS = 30;
    
    // RUNTIME

    public ENetMultiplayerPeer Peer;
    public bool IsServer = false;

    // OVERRIDES

    public override void _Ready()
    {
        Instance = this;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    // FUNCTIONS

    /// <summary>
    /// <b>SERVER</b> -
    /// Creates a new server. Run after switching to the maps scene.
    /// </summary>
    /// <param name="PORT"></param>
    /// <param name="MAX_PLAYERS"></param>
    /// <returns></returns>
    public Error StartServer(int PORT = 56565, int MAX_PLAYERS = 30)
    {
        Peer = new();
        var err = Peer.CreateServer(PORT, MAX_PLAYERS);

        if (err != Error.Ok)
        {
            LOGGER.LOG(LogType.ERROR, $"Failed to create server: {err}", "ServerLifeCycle");
            return err;
        }

        Multiplayer.MultiplayerPeer = Peer;
        IsServer = true;

        LOGGER.LOG(LogType.INFO, $"Successfully created server at 127.0.0.1:{PORT}", "ServerLifeCycle");

        EmitSignal(SignalName.WorldEntered);

        return Error.Ok;
    }

    /// <summary>
    /// <b>CLIENT</b> -
    /// Connects to an already existing server. Run after switching to the maps scene.
    /// </summary>
    /// <param name="IP_ADDRESS"></param>
    /// <param name="PORT"></param>
    /// <returns></returns>
    public Error JoinServer(string IP_ADDRESS = "127.0.0.1", int PORT = 56565)
    {
        Peer = new();
        var err = Peer.CreateClient(IP_ADDRESS, PORT);

        if (err != Error.Ok)
        {
            LOGGER.LOG(LogType.ERROR, $"Failed to create client & join server at {IP_ADDRESS}:{PORT} with Godot error '${err}'", "ClientLifeCycle");
        }

        Multiplayer.MultiplayerPeer = Peer;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, $"Connecting to server {IP_ADDRESS}:{PORT}", "ClientLifeCycle");

        EmitSignal(SignalName.WorldEntered);

        return Error.Ok;
    }

    public void StopServer()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, "Successfully stopped server", "ServerLifeCycle");

        EmitSignal(SignalName.WorldExited);

        GetTree().Quit();
    }

    public void StopClient()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, "Successfully stopped client", "ClientLifeCycle");

        EmitSignal(SignalName.WorldExited);

        GetTree().ChangeSceneToFile("res://Launcher/MainMenu/TitleScreen.tscn");
    }

    // CONNECTIONS

    private void OnServerDisconnected()
    {
        LOGGER.LOG(LogType.WARNING, "Client disconnected from server", "ClientLifeCycle");
        StopClient();
    }

    private void OnConnectionFailed()
    {
        LOGGER.LOG(LogType.WARNING, "Client failed to connect to server", "ClientLifeCycle");
        StopClient();
        EmitSignal(SignalName.ConnectionFailed);
    }

    private void OnConnectedToServer()
    {
        LOGGER.LOG(LogType.INFO, $"Connected to server! PEER_ID: {Multiplayer.GetUniqueId()}", "ClientLifeCycle");
        EmitSignal(SignalName.ConnectionEstablished);
    }

    private void OnPeerDisconnected(long id)
    {
        LOGGER.LOG(LogType.INFO, $"Peer disconnected: {id}", "PeerConnections");
        EmitSignal(SignalName.PlayerDisconnected, (int)id);
    }

    private void OnPeerConnected(long id)
    {
        LOGGER.LOG(LogType.INFO, $"Peer connected: {id}", "PeerConnections");
        EmitSignal(SignalName.PlayerConnected, (int)id);
    }

    public static NetworkManager Instance {get; private set;}
}