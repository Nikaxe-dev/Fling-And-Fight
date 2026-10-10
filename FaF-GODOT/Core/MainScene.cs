using Godot;

namespace FaF;


/// <summary>
/// THROWAWAY (NOT USED) - See the Game singleton for program startup, this immediately removes itself from the scene tree.
/// </summary>
public partial class MainScene : Node
{
    public override void _Ready()
        => QueueFree();
}
