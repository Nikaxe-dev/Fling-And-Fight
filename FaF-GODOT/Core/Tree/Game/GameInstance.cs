using FaF.Core.Implementations;
using FaF.Core.Lib;
using FaF.Core.Tree.Game.Implementations;
using FaF.Core.Tree.Game.Nodes;
using FaF.Core.Tree.Game.Services;
using FaF.Core.Tree.Game.Services.Common;
using FaF.Core.Tree.World;
using Godot;

namespace FaF.Core.Tree.Game;

public abstract partial class GameInstance : SubViewport, IGameInstance, INodeLoader<GameService, ICoreObject>
{
    #region Services

    public RegistryHolder RegistryHolder => GetNode<RegistryHolder>("RegistryHolder");
    public ModLoader ModLoader => GetNode<ModLoader>("ModLoader");
    public NetworkService NetworkService => GetNode<NetworkService>("NetworkService");
    public PlayerService<Player> PlayerService => GetNode<PlayerService<Player>>("PlayerService");

    #endregion

    public virtual void Start()
    {
        OwnWorld3D = true;
        GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), GetPath());

        ((INodeLoader<GameService, ICoreObject>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is ICoreObject && child is GameService service)
                service.LoadService();
    }

    public virtual void Stop() {}
}