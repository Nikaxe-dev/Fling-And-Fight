using FaF.Debug;
using Godot;

namespace FaF.Services.World;

public partial class Player : Node
{
    #region Identification

    public required long PeerID;
    public required string Username;

    #endregion

    #region _Ready()

    public override void _Ready()
    {
        base._Ready();
        Name = Username;
    }

    #endregion
}