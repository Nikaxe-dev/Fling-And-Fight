using FaF.Client.Services.ClientServices;

namespace FaF.Core;

public partial interface IClientInstance
{
    public ClientNetworkService NetworkService => GetNode<ClientNetworkService>("NetworkService");
}