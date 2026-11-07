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
        
        Game.NetworkService.ClientPeerConnected += peerID
            => Rpc(PlayerService<ServerPlayer>.MethodName.SpawnPlayer, peerID, $"guest{GD.RandRange(1111,9999)}");
        
        Game.NetworkService.ClientPeerDisconnected += peerID
            => Rpc(PlayerService<ServerPlayer>.MethodName.DespawnPlayer, peerID);
    }
}