using Godot;

namespace FaF.Visuals.Camera;

/// <summary>
/// A node3d that is always at the current cameras position.
/// </summary>
[GlobalClass]
public partial class CameraPivot : Node3D
{
    public override void _Process(double delta)
    {
        var currentCamera = GetViewport().GetCamera3D();
        if (currentCamera != null)
        {
            GlobalPosition = currentCamera.GlobalPosition;
        }
    }
}