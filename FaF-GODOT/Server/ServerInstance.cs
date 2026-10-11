using FaF.Core;
using FaF.Core.Debug;
using FaF.Server.Services;
using Godot;

namespace FaF.Server;

public partial class ServerInstance(int port, string worldID) : GameInstance, INodeLoader<IServerService>
{
    private Core.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ServerInstance");

    #region Services

    public ServerNetworkService NetworkService => GetNode<ServerNetworkService>("NetworkService");

    #endregion

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ServerInstance");
        ((INodeLoader<IServerService>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is Service service)
                service.LoadService();
        
        NetworkService.CreateServer(port, NetworkService.DEFAULT_MAX_PLAYERS);
    }

    public override void Stop()
    {
        base.Stop();
    }
}