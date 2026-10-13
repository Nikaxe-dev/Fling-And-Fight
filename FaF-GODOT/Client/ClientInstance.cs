using FaF.Client.Services.ClientServices;
using FaF.Client.Services;
using FaF.Core;
using FaF.Core.Lib.Debug;
using FaF.Core.Services;
using Godot;

namespace FaF.Client;

public partial class ClientInstance : GameInstance, IClientInstance, INodeLoader<IClientService>
{
    private Core.Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ClientInstance");

    #region Services

    public ClientNetworkService NetworkService => GetNode<ClientNetworkService>("NetworkService");

    #endregion

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ClientInstance");
        ((INodeLoader<IClientService>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is IClientService && child is Service service)
                service.LoadService();
    }

    public override void Stop()
    {
        base.Stop();
    }

    public void ConnectTo(string serverIP, int serverPort)
    {
        NetworkService.CreateClient(serverIP, serverPort);
    }
}