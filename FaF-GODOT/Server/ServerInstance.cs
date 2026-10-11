using FaF.Core;
using FaF.Core.Debug;

namespace FaF.Server;

public partial class ServerInstance(int port, string worldID) : GameInstance, INodeLoader<IServerService>
{
    private Core.Debug.Logger LOGGER => LoggerFactory.GetLogger(this, "FaF", "ServerInstance");

    public readonly int Port = port;
    public readonly string WorldID = worldID;

    public override void Start()
    {
        base.Start();

        LOGGER.IMPORTANT("Starting ServerInstance");
        ((INodeLoader<IServerService>)this).LoadDerived();

        // if (this is ILoader<IServerService> loader)
        //     loader.LoadDerived();
    }
}