using System;
using FaF.Debug;
using FaF.Game.Players;
using Godot;

namespace FaF.Game.Networking;

// public partial class NetworkManager : Node
// {
//     public readonly static string DEFAULT_IP = "127.0.0.1";
//     public readonly static int DEFAULT_PORT = 56565;

//     public static NetworkManager Instance {get; private set;}

//     public ENetMultiplayerPeer Peer;

//     public override void _Ready()
//     {
//         Instance = this;
//     }

//     public void StartServer(int PORT, int MAX_CHANNELS = 0)
//     {
//         Peer = new ENetMultiplayerPeer();
//         Peer.CreateServer(PORT, 30, MAX_CHANNELS);
//         Multiplayer.MultiplayerPeer = Peer;

//         GD.Print("Started Server");
//     }

//     public void StartClient(string IP_ADDRESS, int PORT)
//     {
//         Peer = new ENetMultiplayerPeer();
//         Peer.CreateClient(IP_ADDRESS, PORT);
//         Multiplayer.MultiplayerPeer = Peer;

//         GD.Print("Started Client");
//     }
// }

public partial class NetworkManager : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Networking/NetworkManager");

    // CODE CONFIGURATION

    [Signal] public delegate void PlayerConnectedEventHandler(int PEER_ID);
    [Signal] public delegate void PlayerDisconnectedEventHandler(int PEER_ID);
    [Signal] public delegate void ConnectionEstablishedEventHandler();
    [Signal] public delegate void ConnectionFailedEventHandler();

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

        return Error.Ok;
    }

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

        return Error.Ok;
    }

    public void StopServer()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, "Successfully stopped server", "ServerLifeCycle");

        GetTree().Quit();
    }

    public void StopClient()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        LOGGER.LOG(LogType.INFO, "Successfully stopped client", "ClientLifeCycle");

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