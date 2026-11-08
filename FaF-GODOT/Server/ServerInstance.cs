using FaF.Core.Lib;
using FaF.Core.Lib.Debug;
using FaF.Core.Tree.Game;
using FaF.Core.Tree.Game.Implementations;
using FaF.Core.Tree.Game.Nodes;
using FaF.Server.Implementations;
using FaF.Server.Tree.Game.Services;
using Godot;

namespace FaF.Server;

public partial class ServerInstance(int port, string worldID) : GameInstance, IServerInstance, INodeLoader<GameService, IServerObject>
{
    private Core.Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ServerInstance");

    #region Services

    public new ServerNetworkService NetworkService => GetNode<ServerNetworkService>("NetworkService");
    public new ServerModLoader ModLoader => GetNode<ServerModLoader>("ModLoader");

    #endregion

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ServerInstance");
        ((INodeLoader<GameService, IServerObject>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is IServerObject && child is GameService service)
                service.LoadService();
        
        NetworkService.CreateServer(port, NetworkService.DEFAULT_MAX_PLAYERS);
    }

    public override void Stop()
    {
        base.Stop();
    }
}