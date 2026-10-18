using Chickensoft.GodotNodeInterfaces;

namespace FaF.Core;

public partial interface IGameInstance : ISubViewport
{
    public void Start();
    public void Stop();
}