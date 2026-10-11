using FaF.Client;
using FaF.Server;
using Godot;

namespace FaF.Core;

public abstract partial class Service : Node
{
    public GameInstance Game => GetOwner<GameInstance>();
    public ServerInstance Server => GetOwner<ServerInstance>();
    public ClientInstance Client => GetOwner<ClientInstance>();

    public virtual void LoadService() {}
}