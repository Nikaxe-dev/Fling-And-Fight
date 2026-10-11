using FaF.Core.Debug;
using Godot;

namespace FaF.Core.Services.Abstract;

public abstract partial class NetworkService : Service
{
    protected Debug.Logger LOGGER => LoggerFactory.GetLogger(Game, "FaF", "Networking");

    #region Configuration

    public readonly int DEFAULT_PORT = 56565;
    public readonly int DEFAULT_MAX_PLAYERS = 30;

    #endregion

    #region Signals

    // emitted for server too

    [Signal] public delegate void PeerConnectedEventHandler(long peerID);
    [Signal] public delegate void PeerDisconnectedEventHandler(long peerId);

    // only emiited for clients connecting to the server

    [Signal] public delegate void ClientPeerConnectedEventHandler(long peerID);
    [Signal] public delegate void ClientPeerDisconnectedEventHandler(long peerID);

    #endregion

    #region State

    public bool IsNetworkConnected {get => Peer != null;}

    public string ServerIP {get; protected set;}
    public int ServerPort {get; protected set;}

    public long LocalPeerID {get => Multiplayer.GetUniqueId();}

    public ENetMultiplayerPeer Peer;

    #endregion

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
    }

    private void SetupNetworkLogs()
    {
        PeerConnected += peerID => LOGGER.IMPORTANT($"Peer connected: {peerID}");
        PeerDisconnected += peerID => LOGGER.IMPORTANT($"Peer disconnected: {peerID}");
    }

    public override void LoadService()
    {
        base.LoadService();

        SetupNetworkLogs();
        SetupServiceSignals();
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    protected abstract void CommunicateServerInfoToPeer(Godot.Collections.Dictionary<string, Variant> info);
}