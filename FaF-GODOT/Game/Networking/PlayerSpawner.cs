using System;
using FaF.Game.Players;
using Godot;

namespace FaF.Game.Networking;

public partial class PlayerSpawner : MultiplayerSpawner
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
        if (GetNode(SpawnPath).GetNode(id.ToString()) is Player player) player.HandleDisconnect();
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