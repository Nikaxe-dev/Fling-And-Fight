using FaF.Editor.Attributes;
using Godot;

namespace FaF.Editor.DataModel.Physical;

# nullable enable

public class PointInstance : Instance
{
    [EditorAccess("Transform"), Save]
    public Transform3D Transform
    {
        get => Node3DRepresentation != null ? Node3DRepresentation.Transform : Transform3D.Identity;
        set { if (Node3DRepresentation != null) Node3DRepresentation.Transform = value; }
    }

    [EditorAccess("GlobalTransform")]
    public Transform3D GlobalTransform
    {
        get => Node3DRepresentation != null ? Node3DRepresentation.GlobalTransform : Transform3D.Identity;
        set { if(Node3DRepresentation != null) Node3DRepresentation.GlobalTransform = value; }
    }

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
        EditorRoot.ENVIRONMENT_REPRESENTATION.AddChild(Node3DRepresentation);
        
        return Node3DRepresentation;
    }
}