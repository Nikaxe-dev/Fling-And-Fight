using FaF.Core.Services;
using FaF.Core.Services.AbstractServices;
using FaF.Core.Services.ProgramServices;
using Godot;

namespace FaF.Core;

public abstract partial class GameInstance : SubViewport, IGameInstance, INodeLoader<ICoreService>
{
    #region Services

    public RegistryHolder RegistryHolder => GetNode<RegistryHolder>("RegistryHolder");
    public ModLoader ModLoader => GetNode<ModLoader>("ModLoader");
    public NetworkService NetworkService => GetNode<NetworkService>("NetworkService");

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