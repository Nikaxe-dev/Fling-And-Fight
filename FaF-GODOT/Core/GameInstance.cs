using Godot;

namespace FaF.Core;

public abstract partial class GameInstance : SubViewport, INodeLoader<IGameService>
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

        ((INodeLoader<IGameService>)this).LoadDerived();
    }
}