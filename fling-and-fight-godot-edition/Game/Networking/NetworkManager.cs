using System;
using FlingAndFight.Game.Debug;
using Godot;

namespace FlingAndFight.Game.Networking;

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
            FaFConsole.PushERROR("NetworkManager/StartServer", $"Failed to create server: {err}");
            return err;
        }

        Multiplayer.MultiplayerPeer = Peer;
        IsServer = true;

        FaFConsole.PrintINFO("NetworkManager/StartServer", $"Successfully created server at 127.0.0.1:{PORT}.");

        return Error.Ok;
    }

    public Error JoinServer(string IP_ADDRESS = "127.0.0.1", int PORT = 56565)
    {
        Peer = new();
        var err = Peer.CreateClient(IP_ADDRESS, PORT);

        if (err != Error.Ok)
        {
            FaFConsole.PushERROR("NetworkManager/JoinServer", $"Failed to create client & join server at {IP_ADDRESS}:{PORT} with Godot error '{err}'");
        }

        Multiplayer.MultiplayerPeer = Peer;
        IsServer = false;

        FaFConsole.PrintINFO("NetworkManager/JoinServer", $"Connecting to server {IP_ADDRESS}:{PORT}.");

        return Error.Ok;
    }

    public void StopServer()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        FaFConsole.PrintINFO("NetworkManager/StopServer", "Successfully stopped server.");

        GetTree().Quit();
    }

    public void StopClient()
    {
        Peer?.Close();
        Peer = null;

        Multiplayer.MultiplayerPeer = null;
        IsServer = false;

        FaFConsole.PrintINFO("NetworkManager/StopClient", "Successfully stopped client.");

        GetTree().ChangeSceneToFile("res://Launcher/MainMenu/TitleScreen.tscn");
    }

    // CONNECTIONS

    private void OnServerDisconnected()
    {
        FaFConsole.PrintWARNING("NetworkManager/Debug", "Client disconnected from server.");
        StopClient();
    }

    private void OnConnectionFailed()
    {
        FaFConsole.PrintWARNING("NetworkManager/Debug", "Client failed to connect to server.");
        StopClient();
        EmitSignal(SignalName.ConnectionFailed);
    }

    private void OnConnectedToServer()
    {
        FaFConsole.PrintINFO("NetworkManager/Debug", $"Connected to server! PEER_ID: {Multiplayer.GetUniqueId()}");
        EmitSignal(SignalName.ConnectionEstablished);
    }

    private void OnPeerDisconnected(long id)
    {
        FaFConsole.PrintINFO("NetworkManager/Debug", $"Peer disconnected: {id}");
        EmitSignal(SignalName.PlayerDisconnected, (int)id);
    }

    private void OnPeerConnected(long id)
    {
        FaFConsole.PrintINFO("NetworkManager/Debug", $"Peer connected: {id}");
        EmitSignal(SignalName.PlayerConnected, (int)id);
    }

    public static NetworkManager Instance {get; private set;}
}