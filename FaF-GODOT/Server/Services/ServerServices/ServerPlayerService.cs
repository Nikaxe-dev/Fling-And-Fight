using FaF.Server.World;
using FaF.Core.Services.AbstractServices;
using Godot;

namespace FaF.Server.Services.ServerServices;

public partial class ServerPlayerService : PlayerService<ServerPlayer>, IServerService
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