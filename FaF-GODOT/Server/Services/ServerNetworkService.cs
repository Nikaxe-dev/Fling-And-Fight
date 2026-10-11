using FaF.Core.Services.Abstract;
using Godot;
using Godot.Collections;

namespace FaF.Server.Services;

public partial class ServerNetworkService : NetworkService, IServerService
{
    #region Signals

    [Signal] public delegate void CreatedServerEventHandler();
    [Signal] public delegate void StoppedServerEventHandler();

    public delegate void FailedToCreateServerEventHandler(Error creationError);
    public event FailedToCreateServerEventHandler FailedToCreateServer;

    [Signal] public delegate void ServerInfoConstructedEventHandler(Godot.Collections.Dictionary<string, Variant> info);

    #endregion

    private void SetupNetworkLogs()
    {
        CreatedServer += () => LOGGER.IMPORTANT("Successfully created server");
        FailedToCreateServer += error => LOGGER.ERROR($"Failed to create server with error: {error}");
    }

    public override void LoadService()
    {
        base.LoadService();

        SetupNetworkLogs();
        SetupServerInfoCommunication();
    }

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

    private void SetupServerInfoCommunication()
        => ClientPeerConnected += peerID =>
            {
                Godot.Collections.Dictionary<string, Variant> info = [];
                EmitSignal(SignalName.ServerInfoConstructed, info);
                RpcId(peerID, MethodName.CommunicateServerInfoToPeer, info);
            };

    protected override void CommunicateServerInfoToPeer(Dictionary<string, Variant> info) {}
}