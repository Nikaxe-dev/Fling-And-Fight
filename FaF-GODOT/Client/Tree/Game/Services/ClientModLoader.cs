using FaF.Client.Implementations;
using FaF.Core.Lib;
using FaF.Core.Tree.Game.Modding.Extensions;
using FaF.Core.Tree.Game.Services.Common;

namespace FaF.Client.Tree.Game.Services;

public partial class ClientModLoader : ModLoader, IClientObject, INodeLoader<ModExtension, IClientObject>
{
    public override void LoadService()
    {
        ((INodeLoader<ModExtension, IClientObject>)this).LoadDerived();
        base.LoadService();
    }
}