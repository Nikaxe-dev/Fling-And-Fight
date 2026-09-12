using Godot;

namespace FaF.Editor.Gizmos;

[GlobalClass]
public partial class OriginGizmo3D : FaFGizmo3D
{
    public MeshInstance3D XLine {get; protected set;}
    public MeshInstance3D YLine {get; protected set;}
    public MeshInstance3D ZLine {get; protected set;}

    public static readonly Color XColor = new(1,0,0,0.75f);
    public static readonly Color YColor = new(0,1,0,0.75f);
    public static readonly Color ZColor = new(0,0,1,0.75f);

    public override void _Ready()
    {
        XLine = CreateLine(Vector3.Right*10000,Vector3.Left*10000,XColor);
        YLine = CreateLine(Vector3.Up*10000,Vector3.Down*10000,YColor);
        ZLine = CreateLine(Vector3.Forward*10000,Vector3.Back*10000,ZColor);

        AddChild(XLine, @internal: InternalMode.Back);
        AddChild(YLine, @internal: InternalMode.Back);
        AddChild(ZLine, @internal: InternalMode.Back);
    }
}