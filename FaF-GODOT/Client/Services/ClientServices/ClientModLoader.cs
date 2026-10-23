using FaF.Client.Modding.ModExtensions;
using FaF.Core.Services;
using FaF.Core.Services.AbstractServices;

namespace FaF.Client.Services.ClientServices;

public partial class ClientModLoader : ModLoader, IClientService, INodeLoader<IClientModExtension>
{
    public override void LoadService()
    {
        ((INodeLoader<IClientModExtension>)this).LoadDerived();
        base.LoadService();
    }
}