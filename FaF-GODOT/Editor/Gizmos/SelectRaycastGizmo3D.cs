using Godot;

namespace FaF.Editor.Gizmos;

public partial class SelectRaycastGizmo3D : FaFGizmo3D
{
    public override void _Ready()
    {
        AddChild(CreatePoint(Vector3.Zero, 0.2f, new Color(1,0,0,0.75f)));
    }
}