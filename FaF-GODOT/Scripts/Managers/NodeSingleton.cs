using Godot;

namespace FaF.Managers;

public abstract partial class NodeSingleton<ThisType> : Node where ThisType : NodeSingleton<ThisType>
{
    public static ThisType Instance {get; private set;}

    public override void _Ready()
    {
        if (Instance != null)
            QueueFree();

        Instance = (ThisType)this;
    }
}