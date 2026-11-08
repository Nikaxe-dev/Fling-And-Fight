using FaF.Server.Tree.Game.Services;

namespace FaF.Core.Tree.Game.Implementations;

public partial interface IServerInstance
{
    public ServerNetworkService NetworkService => GetNode<ServerNetworkService>("NetworkService");
    public ServerModLoader ModLoader => GetNode<ServerModLoader>("ModLoader");
    public ServerPlayerService PlayerService => GetNode<ServerPlayerService>("PlayerService");
}