using FaF.Core.Services;
using FaF.Core.Services.AbstractServices;
using FaF.Server.Modding.ModExtensions;

namespace FaF.Server.Services.ServerServices;

public partial class ServerModLoader : ModLoader, IServerService, INodeLoader<IServerModExtension>
{
    public override void LoadService()
    {
        ((INodeLoader<IServerModExtension>)this).LoadDerived();
        base.LoadService();
    }
};