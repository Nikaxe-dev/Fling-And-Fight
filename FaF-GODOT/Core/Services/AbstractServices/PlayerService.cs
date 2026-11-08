using System.Collections.Generic;
using FaF.Core.Lib.Debug;
using FaF.Core.World;
using Godot;

namespace FaF.Core.Services.AbstractServices;

public abstract partial class PlayerService<PlayerType> : Service
    where PlayerType : Player, new()
{
    protected Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(Game, "FaF", "Player");

    #region Signals

    public delegate void PlayerAddedEventHandler(PlayerType player);
    public event PlayerAddedEventHandler PlayerAdded;

    public delegate void PlayerRemovingEventHandler(PlayerType player);
    public event PlayerRemovingEventHandler PlayerRemoving;

    #endregion

    #region State

    private List<PlayerType> Players = [];

    #endregion

    #region Helpers

    public PlayerType GetPlayerFromPeerID(long peerID)
        => Players.Find(player => player.PeerID == peerID);
    
    public bool TryGetPlayerFromPeerID(long peerID, out PlayerType player)
    {
        player = GetPlayerFromPeerID(peerID);
        return player != null;
    }
    
    public PlayerType GetPlayerFromUsername(string username)
        => Players.Find(player => player.Username == username);
    
    public bool TryGetPlayerFromUsername(string username, out PlayerType player)
    {
        player = GetPlayerFromUsername(username);
        return player != null;
    }
    
    public PlayerType[] GetPlayers()
        => [.. Players];

    #endregion

    #region Player Handling

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    protected virtual void SpawnPlayer(long peerID, string username)
    {
        if (GetPlayerFromPeerID(peerID) != null)
        {
            LOGGER.WARNING($"Attempted to spawn player with username {username} and peerID {peerID} but they were already spawned in");
            return;
        }

        PlayerType player = new()
        {
            PeerID = peerID,
            Username = username
        };

        Players.Add(player);
        AddChild(player);

        PlayerAdded.Invoke(player);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    protected virtual void DespawnPlayer(long peerID)
    {
        if (TryGetPlayerFromPeerID(peerID, out PlayerType player))
        {
            PlayerRemoving.Invoke(player);

            Players.Remove(player);
            player.QueueFree();
        } else
            LOGGER.WARNING($"Attempted to despawn player with peerID {peerID} but they did not exist");
    }

    #endregion

    private void SetupServiceLogs()
    {
        PlayerAdded += player => LOGGER.IMPORTANT($"{player.Username} ({player.PeerID}) added to the game");
        PlayerRemoving += player => LOGGER.IMPORTANT($"{player.Username} ({player.PeerID}) removed from the game");
    }

    public override void LoadService()
    {
        base.LoadService();
        SetupServiceLogs();
    }
}