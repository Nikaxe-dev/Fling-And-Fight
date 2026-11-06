using Godot;

namespace FaF.Core.World;

public partial class Player : Node
{
    public required long PeerID;
    public required string Username;

    public override void _Ready()
    {
        base._Ready();
        Name = Username;
    }
}