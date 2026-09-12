using Godot;

namespace FaF.Editor.Gizmos;

[Tool, GlobalClass]
public partial class OutlineBoxGizmo3D : FaFGizmo3D
{
    private Color _color = new(1,1,1,1);
    [Export] public Color OutlineColor {get => _color;
        set
        {
            _color = value;
            Apply();
        }
    }

    private Vector3 _size = Vector3.One;
    [Export] public Vector3 OutlineSize {get => _size;
        set
        {
            _size = value;
            Apply();
        }
    }

    private bool _visibleThroughWalls = false;
    [Export] public bool VisibleThroughWalls {get => _visibleThroughWalls;
        set
        {
            _visibleThroughWalls = value;
            Apply();
        }
    }

    private MeshInstance3D box;

    private void Apply()
    {
        UpdateOutlineBox(box, Vector3.Zero, Basis.Identity, OutlineSize, OutlineColor, 1, VisibleThroughWalls);
    }

    public override void _Ready()
    {
        box = CreateOutlineBox(Vector3.Zero, Basis.Identity, OutlineSize, OutlineColor, 1, VisibleThroughWalls);
        AddChild(box);
    }
}