using System;
using System.Threading;
using System.Threading.Tasks;
using FaF.Debug;
using FaF.Game.Players;
using Godot;

namespace FaF.Game.Networking;

/// <summary>
/// MultiplayerSpawner usually located under MapRoot/Networking/PlayerSpawner & handles the replication of players to all clients.
/// </summary>
public partial class PlayerSpawner : MultiplayerSpawner
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Networking/Spawners/PlayerSpawner");

    [Export] public PackedScene ReplicatedScene;

    public override void _Ready()
    {
        LOGGER.LOG(LogType.TRACE, "PlayerSpawner _Ready() called.");

        Multiplayer.PeerConnected += HandlePeerConnected;
        Multiplayer.PeerDisconnected += HandlePeerDisconnected;
    }

    private void HandlePeerDisconnected(long id)
    {
        LOGGER.LOG(LogType.TRACE, $"PlayerSpawner _HandlePeerDisconnected() called. (PeerID: {id})");

        if (!Multiplayer.IsServer()) return;
        if (GetNode(SpawnPath).GetNode(id.ToString()) is Player player) player.HandleDisconnect();
    }

    private void HandlePeerConnected(long id)
    {
        LOGGER.LOG(LogType.TRACE, $"PlayerSpawner _HandlePeerConnected() called. (PeerID: {id})");

        if (!Multiplayer.IsServer()) return;

        // RUNS AS SERVER

        // INSTANTIATE SCENE
        Node instance = ReplicatedScene.Instantiate();
        instance.Name = id.ToString();

        // the client might not have loaded the world yet
        // instead run an async function that waits for the world to be loaded
        // similar to the CharacterSpawner's way
        // _ = FinishPlayerInitialization(instance, id);

        // PARENT
        GetNode(SpawnPath).CallDeferred("add_child", instance);
    }

    private async Task FinishPlayerInitialization(Node instance, long PeerID)
    {
        while (!GameManager.IS_IN_GAME)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        GetNode(SpawnPath).CallDeferred("add_child", instance);
    }
}