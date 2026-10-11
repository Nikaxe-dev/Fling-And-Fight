using FaF.Core.Debug;
using Godot;

namespace FaF.Core.Services.Lifecycle;

public partial class NetworkService() : Service([])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.Network;

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

    // server & client communication

    [Signal] public delegate void ServerInfoCommunicatedToClientEventHandler(Godot.Collections.Dictionary<string, Variant> info);

    // use this from other services in order to assign members of ServerInfo
    [Signal] public delegate void ServerInfoConstructedEventHandler(Godot.Collections.Dictionary<string, Variant> info);

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

    #region LoadService()

    private void SetupNetworkLogs()
    {
        PeerConnected += peerID =>               LOGGER.IMPORTANT($"Peer connected: {peerID}");
        PeerDisconnected += peerID =>            LOGGER.IMPORTANT($"Peer disconnected: {peerID}");

        ClientConnectionEstablished += () =>     LOGGER.IMPORTANT($"Connected to server! My peer id: {LocalPeerID}");
        ClientConnectionFailed += () =>          LOGGER.IMPORTANT($"Failed to connect to server!");

        ClientDisconnectedFromServer += () =>    LOGGER.IMPORTANT($"Disconnected from the server!");

        CreatedServer += () =>                   LOGGER.IMPORTANT("Successfully created server");
        FailedToCreateServer += creationError => LOGGER.IMPORTANT($"Failed to create server with error: {creationError}");

        CreatedClient += () =>                   LOGGER.IMPORTANT("Successfully created client");
        FailedToCreateClient += creationError => LOGGER.IMPORTANT($"Failed to create client with error: {creationError}");
    }

    private void SetupServiceSignals()
    {
        Multiplayer.PeerConnected += peerID =>
        {
            EmitSignal(SignalName.PeerConnected, peerID);
            // filter server
            if (peerID != 1)
                EmitSignal(SignalName.ClientPeerConnected, peerID);
        };

        Multiplayer.PeerDisconnected += peerID =>
        {
            EmitSignal(SignalName.PeerDisconnected, peerID);
            // filter server
            if (peerID != 1)
                EmitSignal(SignalName.ClientPeerDisconnected, peerID);
        };

        // connection signals

        Multiplayer.ConnectedToServer += () => EmitSignal(SignalName.ClientConnectionEstablished);
        Multiplayer.ConnectionFailed += () => EmitSignal(SignalName.ClientConnectionFailed);

        Multiplayer.ServerDisconnected += () => EmitSignal(SignalName.ClientDisconnectedFromServer);
    }

    protected override void _LoadService()
    {
        base._LoadService();

        SetupNetworkLogs();
        SetupServiceSignals();
        SetupServerInfoCommunication();
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

        EmitSignal(SignalName.ClientPeerConnected, LocalPeerID);
        EmitSignal(SignalName.PeerConnected, LocalPeerID);

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

    #region Communication

    private void SetupServerInfoCommunication()
    {
        ClientPeerConnected += peerID =>
        {
            if (!IsServer)
                return;

            // construct the info, pass it to the constructed event for others to assign to, then send the constructed info to the connected peer

            Godot.Collections.Dictionary<string, Variant> info = [];
            EmitSignal(SignalName.ServerInfoConstructed, info);
            RpcId(peerID, MethodName.CommunicateServerInfoToPeer, info);
        };
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void CommunicateServerInfoToPeer(Godot.Collections.Dictionary<string, Variant> info)
    {
        if (IsServer)
            return;
        
        LOGGER.IMPORTANT($"Server communicated info {info} to client");
        EmitSignal(SignalName.ServerInfoCommunicatedToClient, info);
    }

    #endregion
}