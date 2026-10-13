using Chickensoft.GodotNodeInterfaces;

namespace FaF.Core;

public partial interface IGameInstance : INode
{
    public void Start();
    public void Stop();
}