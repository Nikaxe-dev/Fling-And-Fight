using FaF.Client.Tree.Game.Services;
using FaF.Server.Tree.Game.Services;

namespace FaF.Core.Tree.Game.Implementations;

public partial interface IClientInstance
{
    public ClientNetworkService NetworkService => GetNode<ClientNetworkService>("NetworkService");
    public ServerModLoader ModLoader => GetNode<ServerModLoader>("ModLoader");
    public ServerPlayerService PlayerService => GetNode<ServerPlayerService>("PlayerService");
}