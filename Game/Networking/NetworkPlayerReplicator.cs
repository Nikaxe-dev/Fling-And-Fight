using System;
using FlingAndFight.Game.Character;
using Godot;

namespace FlingAndFight.Game.Networking;

[GlobalClass]
public partial class NetworkPlayerReplicator : MultiplayerSpawner
{
    [Export] public PackedScene ReplicatedScene;

    public override void _Ready()
    {
        Multiplayer.PeerConnected += HandlePeerConnected;
        Multiplayer.PeerDisconnected += HandlePeerDisconnected;
    }

    private void HandlePeerDisconnected(long id)
    {
        if (!Multiplayer.IsServer()) return;
        if (GetNode(SpawnPath).GetNode(id.ToString()) is Player player) player.QueueFree();
    }

    private void HandlePeerConnected(long id)
    {
        if (!Multiplayer.IsServer()) return;

        // RUNS AS SERVER

        // INSTANTIATE SCENE
        Node instance = ReplicatedScene.Instantiate();
        instance.Name = id.ToString();

        // PARENT
        GetNode(SpawnPath).CallDeferred("add_child", instance);
    }
}