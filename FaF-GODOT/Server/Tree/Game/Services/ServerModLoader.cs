using FaF.Core.Lib;
using FaF.Core.Tree.Game.Modding.Extensions;
using FaF.Core.Tree.Game.Services.Common;
using FaF.Server.Implementations;

namespace FaF.Server.Tree.Game.Services;

public partial class ServerModLoader : ModLoader, IServerObject, INodeLoader<ModExtension, IServerObject>
{
    public override void LoadService()
    {
        ((INodeLoader<ModExtension, IServerObject>)this).LoadDerived();
        base.LoadService();
    }
};