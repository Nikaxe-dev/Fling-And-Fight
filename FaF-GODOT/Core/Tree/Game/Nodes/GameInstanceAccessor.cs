using FaF.Core.Extensions;
using FaF.Core.Tree.Game.Implementations;
using Godot;

namespace FaF.Core.Tree.Game.Nodes;

public abstract partial class GameInstanceAccessor : Node
{
    public GameInstance Game => this.GetGame();
    public IServerInstance Server => this.GetServer();
    public IClientInstance Client => this.GetClient();
}