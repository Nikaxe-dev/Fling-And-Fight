using FaF.Client.Tree.Game.Services;

namespace FaF.Core.Tree.Game.Implementations;

public partial interface IClientInstance
{
    public ClientNetworkService NetworkService => GetNode<ClientNetworkService>("NetworkService");
    public ClientModLoader ModLoader => GetNode<ClientModLoader>("ModLoader");
    public ClientPlayerService PlayerService => GetNode<ClientPlayerService>("PlayerService");
}