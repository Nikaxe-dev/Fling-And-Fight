using Godot;

namespace FaF.Core.World;

public partial class Player : Node
{
    public long PeerID;
    public string Username;

    public override void _Ready()
    {
        base._Ready();
        Name = Username;
    }
}