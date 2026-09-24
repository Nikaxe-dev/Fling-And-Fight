using System;
using FaF.Core;
using FaF.Debug;
using FaF.Game.Players;
using Godot;

namespace FaF.Game.Networking;

/// <summary>
/// Manages all of the networking of FaF including starting a server, joining a server, and disconnection from that server.
/// </summary>
public partial class NetworkManager : Manager<NetworkManager>
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

    // /// <summary>
    // /// <b>COMMON</b> -
    // /// Emitted when the game enters a world (LOADING THE SCENE)
    // /// </summary>
    // [Signal] public delegate void WorldEnteredEventHandler();

    // /// <summary>
    // /// <b>COMMON</b> -
    // /// Emitted when the game exits a world (UNLOADING THE SCENE TO THE MAIN MENU / PROGRAM END)
    // /// </summary>
    // [Signal] public delegate void WorldExitedEventHandler();

    public static readonly string DEFAULT_IP = "127.0.0.1";
    public static readonly int DEFAULT_PORT = 56565;

    public static readonly int DEFAULT_MAX_PLAYERS = 30;

    /// <summary>
    /// Whether the server/client is connected properly.
    /// </summary>
    public static bool IS_CONNECTED {get; private set;} = false;
    
    // RUNTIME

    public ENetMultiplayerPeer Peer;
    public bool IsServer = false;

    // OVERRIDES

    public override void _Ready()
    {
        base._Ready();

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
    /// THIS IS NETWORKING RELATED ONLY. RUN THE GAMEMANAGER METHOD FOR LOADING THE WORLD TOO.
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
            LOGGER.LOG(LogType.ERROR, $"Failed to create server: {err}", "ServerLifeCycle", true);
            return err;
        }

        Multiplayer.MultiplayerPeer = Peer;
        IsServer = true;

        LOGGER.LOG(LogType.INFO, $"Successfully created server at 127.0.0.1:{PORT}", "ServerLifeCycle", true);
        IS_CONNECTED = true;

        // EmitSignal(SignalName.WorldEntered);

        return Error.Ok;
    }

    /// <summary>
    /// <b>CLIENT</b> -
    /// Connects to an already existing server. Run after switching to the maps scene.
    /// THIS IS NETWORKING RELATED ONLY. RUN THE GAMEMANAGER METHOD FOR LOADING THE WORLD TOO.
    /// </summary>
    /// <param name="IP_ADDRESS"></param>
    /// <param name="PORT"></param>
    /// <returns></returns>
    public Error StartClient(string IP_ADDRESS = "127.0.0.1", int PORT = 56565)
    {
        Peer = new();
        var err = Peer.CreateClient(IP_ADDRESS, PORT);

        if (err != Error.Ok)
        {
            LOGGER.LOG(LogType.ERROR, $"Failed to create client & join server at {IP_ADDRESS}:{PORT} with Godot error '${err}'", "ClientLifeCycle", true);
        }

        Multiplayer.MultiplayerPeer = Peer;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, $"Connecting to server {IP_ADDRESS}:{PORT}", "ClientLifeCycle", true);
        IS_CONNECTED = true;

        // EmitSignal(SignalName.WorldEntered);

        return Error.Ok;
    }

    public void StopServer()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, "Stopping server networking", "ServerLifeCycle", true);

        // EmitSignal(SignalName.WorldExited);
    }

    public void StopClient()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, "Stopping client networking", "ClientLifeCycle", true);

        // EmitSignal(SignalName.WorldExited);
    }

    // CONNECTIONS

    private void OnServerDisconnected()
    {
        LOGGER.LOG(LogType.WARNING, "Client disconnected from server", "ClientLifeCycle", true);
        StopClient();

        IS_CONNECTED = false;
    }

    private void OnConnectionFailed()
    {
        LOGGER.LOG(LogType.WARNING, "Client failed to connect to server", "ClientLifeCycle", true);
        StopClient();
        EmitSignal(SignalName.ConnectionFailed);

        IS_CONNECTED = false;
    }

    private void OnConnectedToServer()
    {
        LOGGER.LOG(LogType.INFO, $"Connected to server! PEER_ID: {Multiplayer.GetUniqueId()}", "ClientLifeCycle", true);
        EmitSignal(SignalName.ConnectionEstablished);

        IS_CONNECTED = true;
    }

    private void OnPeerDisconnected(long id)
    {
        LOGGER.LOG(LogType.INFO, $"Peer disconnected: {id}", "PeerConnections", true);
        EmitSignal(SignalName.PlayerDisconnected, (int)id);
    }

    private void OnPeerConnected(long id)
    {
        LOGGER.LOG(LogType.INFO, $"Peer connected: {id}", "PeerConnections", true);
        EmitSignal(SignalName.PlayerConnected, (int)id);
    }
}