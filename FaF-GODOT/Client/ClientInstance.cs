using FaF.Client.Services;
using FaF.Core;
using FaF.Core.Debug;
using Godot;

namespace FaF.Client;

public partial class ClientInstance : GameInstance, INodeLoader<IClientService>
{
    private Core.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ClientInstance");

    #region Services

    public ClientNetworkService NetworkService => GetNode<ClientNetworkService>("NetworkService");

    #endregion

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ClientInstance");
        ((INodeLoader<IClientService>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is Service service)
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