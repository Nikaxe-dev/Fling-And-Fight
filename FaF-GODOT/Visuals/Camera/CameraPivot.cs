using Godot;

namespace FaF.Visuals.Camera;

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