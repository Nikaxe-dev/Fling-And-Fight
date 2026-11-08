using Chickensoft.GodotNodeInterfaces;

namespace FaF.Core.Tree.Game.Implementations;

public partial interface IGameInstance : ISubViewport
{
    public void Start();
    public void Stop();
}