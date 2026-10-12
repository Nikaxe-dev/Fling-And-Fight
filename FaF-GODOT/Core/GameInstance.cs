using Godot;

namespace FaF.Core;

public abstract partial class GameInstance : SubViewport, INodeLoader<ICoreService>
{
    protected Node AddService(Node service)
    {
        AddChild(service);
        return service;
    }

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