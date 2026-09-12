using Godot;

namespace FaF.Editor.Gizmos;

public abstract partial class AutoScalingGizmo3D : FaFGizmo3D
{
    [Export] public float TargetSize = 1;

    public override void _Process(double delta)
    {
        var camera = GetViewport().GetCamera3D();

        if (camera == null) return;

        var distance = GlobalPosition.DistanceTo(camera.GlobalPosition);
        Scale = Vector3.One * distance * TargetSize;
    }
}