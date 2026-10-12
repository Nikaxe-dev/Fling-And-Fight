using FaF.Core.Services.AbstractServices;
using Godot;
using Godot.Collections;

namespace FaF.Client.Services.ClientServices;

public partial class ClientNetworkService : NetworkService, IClientService
{
    #region Signals

    [Signal] public delegate void ConnectionEstablishedEventHandler();
    [Signal] public delegate void ConnectionFailedEventHandler();

    [Signal] public delegate void DisconnectedFromServerEventHandler();

    [Signal] public delegate void CreatedClientEventHandler();
    [Signal] public delegate void StoppedClientEventHandler();

    public delegate void FailedToCreateClientEventHandler(Error creationError);
    public event FailedToCreateClientEventHandler FailedToCreateClient;

    [Signal] public delegate void ServerInfoCommunicatedEventHandler(Godot.Collections.Dictionary<string, Variant> info);

    #endregion

    private void SetupNetworkLogs()
    {
        ConnectionEstablished += () => LOGGER.IMPORTANT($"Connected to server! My peer id: {LocalPeerID}");
        ConnectionFailed += () => LOGGER.ERROR("Failed to connect to server!");

        DisconnectedFromServer += () => LOGGER.ERROR("Disconnected from server!");

        CreatedClient += () => LOGGER.IMPORTANT("Successfully created client!");
        FailedToCreateClient += creationError => LOGGER.ERROR($"Failed to create client with error: {creationError}");
    }

    private void SetupServiceSignals()
    {
        Multiplayer.ConnectedToServer += () => EmitSignal(SignalName.ConnectionEstablished);
        Multiplayer.ConnectionFailed += () => EmitSignal(SignalName.ConnectionFailed);
        
        Multiplayer.ServerDisconnected += () => EmitSignal(SignalName.DisconnectedFromServer);
    }

    public override void LoadService()
    {
        base.LoadService();

        SetupNetworkLogs();
        SetupServiceSignals();
    }

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

        EmitSignal(NetworkService.SignalName.ClientPeerConnected, LocalPeerID);
        EmitSignal(NetworkService.SignalName.PeerConnected, LocalPeerID);

        return Error.Ok;
    }

    public void StopClient()
    {
        Peer?.Close();

        Peer = null;
        Multiplayer.MultiplayerPeer = null;

        EmitSignal(SignalName.StoppedClient);
    }

    protected override void CommunicateServerInfoToPeer(Dictionary<string, Variant> info)
    {
        LOGGER.IMPORTANT($"Server communicated info {info} to client");
        EmitSignal(SignalName.ServerInfoCommunicated, info);
    }
}