using FaF.Client.Implementations;
using FaF.Client.Tree.Game.Services;
using FaF.Core.Lib;
using FaF.Core.Lib.Debug;
using FaF.Core.Tree.Game;
using FaF.Core.Tree.Game.Implementations;
using FaF.Core.Tree.Game.Nodes;
using Godot;

namespace FaF.Client;

public partial class ClientInstance : GameInstance, IClientInstance, INodeLoader<GameService, IClientObject>
{
    private Core.Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ClientInstance");

    #region Services

    public new ClientNetworkService NetworkService => GetNode<ClientNetworkService>("NetworkService");
    public new ClientModLoader ModLoader => GetNode<ClientModLoader>("ModLoader");

    #endregion

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ClientInstance");
        ((INodeLoader<GameService, IClientObject>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is IClientObject && child is GameService service)
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