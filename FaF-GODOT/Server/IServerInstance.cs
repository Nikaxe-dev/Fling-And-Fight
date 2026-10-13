using FaF.Server.Services.ServerServices;

namespace FaF.Core;

public partial interface IServerInstance
{
    public ServerNetworkService NetworkService => GetNode<ServerNetworkService>("NetworkService");
}