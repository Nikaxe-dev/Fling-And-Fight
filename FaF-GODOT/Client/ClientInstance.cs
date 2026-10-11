using FaF.Core;
using FaF.Core.Debug;

namespace FaF.Client;

public partial class ClientInstance : GameInstance, INodeLoader<IClientService>
{
    private Core.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ClientInstance");

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ClientInstance");
        ((INodeLoader<IClientService>)this).LoadDerived();
    }

    public void ConnectTo(string serverIP, string serverPort)
    {
        
    }
}