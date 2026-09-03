using FaF.Editor.Attributes;
using FaF.Editor.UI.Properties;
using Godot;

namespace FaF.Editor.DataModel.Physical;

# nullable enable

public class PointInstance : Instance
{
    private static Vector3 RadToDeg(Vector3 vector) => new(Mathf.RadToDeg(vector.X),Mathf.RadToDeg(vector.Y),Mathf.RadToDeg(vector.Z));
    private static Vector3 DegToRad(Vector3 vector) => new(Mathf.DegToRad(vector.X),Mathf.DegToRad(vector.Y),Mathf.DegToRad(vector.Z));

    [Save]
    public Transform3D Transform
    {
        get => Node3DRepresentation != null ? Node3DRepresentation.Transform : Transform3D.Identity;
        set { if (Node3DRepresentation != null) Node3DRepresentation.Transform = value; PropertiesContainer.RefreshTransformationPropertySelectors(); }
    }

    public Transform3D GlobalTransform
    {
        get => Node3DRepresentation != null ? Node3DRepresentation.GlobalTransform : Transform3D.Identity;
        set { if(Node3DRepresentation != null) Node3DRepresentation.GlobalTransform = value; PropertiesContainer.RefreshTransformationPropertySelectors(); }
    }

    [EditorAccess("Position")]
    public Vector3 Position {get => Transform.Origin; set => Transform = new(Transform.Basis, value);}

    [EditorAccess("Rotation")]
    public Vector3 Rotation {get => RadToDeg(Transform.Basis.GetEuler(EulerOrder.Xyz)); set => Transform = new(Basis.FromEuler(DegToRad(value), EulerOrder.Xyz), Transform.Origin);}

    [EditorAccess("Global Position")]
    public Vector3 GlobalPosition {get => GlobalTransform.Origin; set => GlobalTransform = new(GlobalTransform.Basis, value);}

    [EditorAccess("Global Rotation")]
    public Vector3 GlobalRotation {get => RadToDeg(GlobalTransform.Basis.GetEuler(EulerOrder.Xyz)); set => GlobalTransform = new(Basis.FromEuler(DegToRad(value), EulerOrder.Xyz), GlobalTransform.Origin);}

    public Node3D? Node3DRepresentation {get; private set;}

    protected override void UpdateNode()
    {
        base.UpdateNode();
        if (NodeRepresentation is Node3D node3D)
        {
            node3D.Transform = Transform;
        }
    }

    protected override Node? InternalCreateNode(bool addToTree = true)
    {
        Node3DRepresentation = new Node3D();
        // EditorRoot.ENVIRONMENT_REPRESENTATION.AddChild(Node3DRepresentation);
        
        return Node3DRepresentation;
    }
}