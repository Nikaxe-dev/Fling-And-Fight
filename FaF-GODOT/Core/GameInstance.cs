using FaF.Core.Services;
using FaF.Core.Services.ProgramServices;
using Godot;

namespace FaF.Core;

public abstract partial class GameInstance : SubViewport, IGameInstance, INodeLoader<ICoreService>
{
    #region Services

    public RegistryHolder RegistryHolder => GetNode<RegistryHolder>("RegistryHolder");

    #endregion

    public virtual void Start()
    {
        OwnWorld3D = true;
        GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface(), GetPath());

        ((INodeLoader<ICoreService>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is ICoreService && child is Service service)
                service.LoadService();
    }

    public virtual void Stop() {}
}