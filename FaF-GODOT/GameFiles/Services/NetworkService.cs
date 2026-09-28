using FaF.Debug;
using FaF.Enums;
using Godot;

namespace FaF.Services;

public partial class NetworkService : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services/NetworkService");

    #region Configuration

    public readonly int DEFAULT_PORT = 56565;
    public readonly int DEFAULT_MAX_PLAYERS = 30;

    #endregion

    #region Signals

    // emitted for server too

    [Signal] public delegate void PeerConnectedEventHandler(long peerID);
    [Signal] public delegate void PeerDisconnectedEventHandler(long peerID);

    // only emmited for clients connecting to the server

    [Signal] public delegate void ClientPeerConnectedEventHandler(long peerID);
    [Signal] public delegate void ClientPeerDisconnectedEventHandler(long peerID);
    
    // client connection to server

    [Signal] public delegate void ClientConnectionEstablishedEventHandler();
    [Signal] public delegate void ClientConnectionFailedEventHandler();

    [Signal] public delegate void ClientDisconnectedFromServerEventHandler();

    // server creation/stopping

    [Signal] public delegate void CreatedServerEventHandler();

    // no signal use due to variant limitations
    public delegate void FailedToCreateServerEventHandler(Error creationError);
    public event FailedToCreateServerEventHandler FailedToCreateServer;

    [Signal] public delegate void StoppedServerEventHandler();

    // client creation/stopping

    [Signal] public delegate void CreatedClientEventHandler();

    // no signal use due to variant limitations
    public delegate void FailedToCreateClientEventHandler(Error creationError);
    public event FailedToCreateClientEventHandler FailedToCreateClient;

    [Signal] public delegate void StoppedClientEventHandler();

    #endregion

    #region State

    public bool IsNetworkConnected {get => Peer != null;}
    public bool IsServer {get => Multiplayer.IsServer() && IsNetworkConnected;}
    public bool IsClient {get => !Multiplayer.IsServer() && IsNetworkConnected;}

    public string ServerIP {get; private set;}
    public int ServerPort {get; private set;}

    public long LocalPeerID {get => Multiplayer.GetUniqueId();}

    public ENetMultiplayerPeer Peer;

    #endregion

    #region _Ready()    

    private void SetupNetworkLogs()
    {
        PeerConnected += peerID => LOGGER.LOG(LogType.INFO, $"Peer connected: {peerID}", "PeerConnections", true);
        PeerDisconnected += peerID => LOGGER.LOG(LogType.INFO, $"Peer disconnected: {peerID}", "PeerConnections", true);

        ClientConnectionEstablished += () => LOGGER.LOG(LogType.INFO, $"Connected to server! My peer id: {LocalPeerID}", "Lifecycle", true);
        ClientConnectionFailed += () => LOGGER.LOG(LogType.ERROR, $"Failed to connect to server!", "Lifecycle", true);

        ClientDisconnectedFromServer += () => LOGGER.LOG(LogType.ERROR, $"Disconnected from the server!", "Lifecycle", true);

        CreatedServer += () => LOGGER.LOG(LogType.INFO, "Successfully created server", "Lifecycle", true);
        FailedToCreateServer += creationError => LOGGER.LOG(LogType.ERROR, $"Failed to create server with error: {creationError}", "Lifecycle", true);

        CreatedClient += () => LOGGER.LOG(LogType.INFO, "Successfully created client", "Lifecycle", true);
        FailedToCreateClient += creationError => LOGGER.LOG(LogType.ERROR, $"Failed to create client with error: {creationError}", "Lifecycle", true);
    }

    private void SetupServiceSignals()
    {
        // peer signals

        Multiplayer.PeerConnected += peerID => EmitSignal(SignalName.PeerConnected, peerID);
        Multiplayer.PeerDisconnected += peerID => EmitSignal(SignalName.PeerDisconnected, peerID);

        Multiplayer.PeerConnected += peerID =>
        {
            // filter server
            if (peerID != 1)
                EmitSignal(SignalName.ClientPeerConnected, peerID);
        };

        Multiplayer.PeerDisconnected += peerID =>
        {
            // filter server
            if (peerID != 1)
                EmitSignal(SignalName.ClientPeerDisconnected, peerID);
        };

        // connection signals

        Multiplayer.ConnectedToServer += () => EmitSignal(SignalName.ClientConnectionEstablished);
        Multiplayer.ConnectionFailed += () => EmitSignal(SignalName.ClientConnectionFailed);

        Multiplayer.ServerDisconnected += () => EmitSignal(SignalName.ClientDisconnectedFromServer);
    }

    public override void _Ready()
    {
        base._Ready();

        SetupNetworkLogs();
        SetupServiceSignals();
    }

    #endregion

    #region Server

    public Error CreateServer(int PORT, int MAX_CLIENTS)
    {
        Peer = new();
        var creationError = Peer.CreateServer(PORT, MAX_CLIENTS);

        if (creationError != Error.Ok)
        {
            FailedToCreateServer?.Invoke(creationError);
            return creationError;
        }

        Multiplayer.MultiplayerPeer = Peer;

        ServerIP = "127.0.0.1";
        ServerPort = PORT;

        EmitSignal(SignalName.CreatedServer);

        return Error.Ok;
    }

    public void StopServer()
    {
        Peer?.Close();

        Peer = null;
        Multiplayer.MultiplayerPeer = null;

        EmitSignal(SignalName.StoppedServer);
    }

    #endregion

    #region Client

    public Error CreateClient(string serverIP, int serverPort)
    {
        Peer = new();
        var creationError = Peer.CreateClient(serverIP, serverPort);

        if (creationError != Error.Ok)
        {
            FailedToCreateClient?.Invoke(creationError);
            return creationError;
        }

        Multiplayer.MultiplayerPeer = Peer;

        ServerIP = serverIP;
        ServerPort = serverPort;

        EmitSignal(SignalName.CreatedClient);

        return Error.Ok;
    }

    public void StopClient()
    {
        Peer?.Close();

        Peer = null;
        Multiplayer.MultiplayerPeer = null;

        EmitSignal(SignalName.StoppedClient);
    }

    #endregion
}