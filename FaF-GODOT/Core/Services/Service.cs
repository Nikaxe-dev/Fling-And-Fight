using Godot;

namespace FaF.Core.Services;

public abstract partial class Service : Node
{
    public GameInstance Game => GetOwner<GameInstance>();
    public IServerInstance Server => GetOwner<IServerInstance>();
    public IClientInstance Client => GetOwner<IClientInstance>();

    public virtual void LoadService() {}
}