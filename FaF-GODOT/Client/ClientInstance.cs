using FaF.Core;
using FaF.Core.Debug;
using Godot;

namespace FaF.Client;

public partial class ClientInstance : GameInstance, INodeLoader<IClientService>
{
    private Core.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ClientInstance");

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ClientInstance");
        ((INodeLoader<IClientService>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is IGameService service)
                service.LoadService();
    }

    public override void Stop()
    {
        base.Stop();
    }

    public void ConnectTo(string serverIP, string serverPort)
    {
        
    }
}