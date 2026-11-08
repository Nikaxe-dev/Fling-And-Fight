using FaF.Core.Tree.Game.Services.Common;
using FaF.Server.Implementations;
using FaF.Server.Tree.World;
using Godot;

namespace FaF.Server.Tree.Game.Services;

public partial class ServerPlayerService : PlayerService<ServerPlayer>, IServerObject
{
    public override void LoadService()
    {
        base.LoadService();

        // TODO: implement proper usernames with steam support
        
        Game.NetworkService.ClientPeerConnected += peerID =>
        {
            // tell new player about already existing players
            foreach (ServerPlayer player in GetPlayers())
                RpcId(peerID, PlayerService<ServerPlayer>.MethodName.SpawnPlayer, player.PeerID, player.Username);

            // add new player to peers
            Rpc(PlayerService<ServerPlayer>.MethodName.SpawnPlayer, peerID, $"guest{GD.RandRange(1111,9999)}");
        };
        
        Game.NetworkService.ClientPeerDisconnected += peerID
            => Rpc(PlayerService<ServerPlayer>.MethodName.DespawnPlayer, peerID);
    }
}