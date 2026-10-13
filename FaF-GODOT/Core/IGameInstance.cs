using Godot;

namespace FaF.Core;

public partial interface IGameInstance
{
    public T GetNode<T>(NodePath node) where T : class;

    public void Start();
    public void Stop();
}