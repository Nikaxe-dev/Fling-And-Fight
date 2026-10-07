using System.Collections.Generic;
using FaF.Debug;
using Godot;

namespace FaF.Services.World;

public partial class PlayerService() : Service(["NetworkService", "WorldService"])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.Player;

    #region Signals

    [Signal] public delegate void PlayerAddedEventHandler(Player player);
    [Signal] public delegate void PlayerRemovingEventHandler(Player player);

    #endregion

    #region State

    public Player LocalPlayer {get; private set;}
    private List<Player> Players = [];

    #endregion

    #region Nodes

    public Node PlayersContainer {get; private set;}

    #endregion

    #region Helpers

    public Player GetPlayerFromPeerID(long peerID)
        => Players.Find(player => player.PeerID == peerID);
    
    public Player GetPlayerFromUsername(string username)
        => Players.Find(player => player.Username == username);
    
    public Player[] GetPlayers()
        => [.. Players];

    #endregion

    #region Player Handling

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void SpawnPlayer(long peerID, string username)
    {
        if (GetPlayerFromPeerID(peerID) != null)
        {
            LOGGER.WARNING($"Attempted to spawn player with username {username} and peerID {peerID} but they were already spawned in");
            return;
        }

        Player player = new()
        {
            PeerID = peerID,
            Username = username
        };

        Players.Add(player);
        PlayersContainer.AddChild(player);

        EmitSignal(SignalName.PlayerAdded, player);

        if (peerID == Game.NetworkService.LocalPeerID)
            LocalPlayer = player;
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void DespawnPlayer(long peerID)
    {
        if (PlayersContainer.GetNodeOrNull(peerID.ToString()) is Player player)
        {
            EmitSignal(SignalName.PlayerRemoving, player);

            Players.Remove(player);
            player.QueueFree();
        } else
            LOGGER.WARNING($"Attempted to despawn player with peerID '{peerID}' but they did not exist");
    }

    #endregion

    #region LoadService

    private void SetupPlayerLogs()
    {
        PlayerAdded += player => LOGGER.IMPORTANT($"Player '{player.Username}' (with PeerID '{player.PeerID}') added");
        PlayerRemoving += player => LOGGER.IMPORTANT($"Removing player '{player.Username}' (with PeerID '{player.PeerID}')");
    }

    protected override void _LoadService()
    {
        base._LoadService();
        SetupPlayerLogs();

        Game.WorldService.WorldRootLoaded +=
        () => PlayersContainer = Game.WorldService.WorldRoot.GetNode("Players");

        // dont make the mistake of changing the services system to load dependencies first just to realize that wasn't your issue like me 😭

        Game.NetworkService.ClientPeerConnected += peerID =>
        {
            if (Game.NetworkService.IsClient)
                return;
            
            Rpc(MethodName.SpawnPlayer, peerID, $"guest{GD.RandRange(1111, 9999)}");
        };

        Game.NetworkService.ClientPeerDisconnected += peerID =>
        {
            if (Game.NetworkService.IsClient)
                return;
            
            Rpc(MethodName.DespawnPlayer, peerID);
        };
    }

    #endregion
}