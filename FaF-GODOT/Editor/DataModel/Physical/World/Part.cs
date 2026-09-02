using FaF.Editor.Attributes;
using Godot;

namespace FaF.Editor.DataModel.Physical.World;

# nullable enable

public class Part : PointInstance
{
    private Vector3 _size {get; set;} = Vector3.One;

    [EditorAccess("Size"), Save]
    public Vector3 Size {get => _size; set { _size = value; if (StaticBody3DRepresentation != null) StaticBody3DRepresentation.Scale = Size; }}

    private Color _color = new("#FFFFFF");

    [EditorAccess("Color"), Save("Color")]
    public Color PartColor {get => _color;
        set
        {
            _color = value;
            UpdateMeshMaterial();
        }
    }

    private string _material = "SmoothPlastic";

    [EditorAccess("Material"), Save]
    public string Material {get => _material;
        set
        {
            _material = value;
            UpdateMeshMaterial();
        }
    }

    private static readonly Color DEFAULT_COLOR = new("#FFFFFF");
    protected void UpdateMeshMaterial()
    {
        if (MeshInstance3DRepresentation != null)
        {
            MeshInstance3DRepresentation.MaterialOverride = GetMaterial(Material);

            if (PartColor != DEFAULT_COLOR)
            {
                // make material unique so this change doesnt apply to every other part with the same material (look back on this for possible optimization)
                var newMaterial = (BaseMaterial3D)MeshInstance3DRepresentation.MaterialOverride.Duplicate();
                newMaterial.AlbedoColor = PartColor;

                MeshInstance3DRepresentation.MaterialOverride = newMaterial;
            }
        }
    }

    private string _shape = "Box";

    [EditorAccess("Shape ID"), Save]
    public string Shape {get => _shape; set
        {
            if (CollisionShape3DRepresentation != null) CollisionShape3DRepresentation.Shape = GetShape(Shape);
            if (MeshInstance3DRepresentation != null) MeshInstance3DRepresentation.Mesh = GetMesh(Shape);
            _shape = Shape;
        }
    }

    public StaticBody3D? StaticBody3DRepresentation {get; private set;}

    public MeshInstance3D? MeshInstance3DRepresentation {get; private set;}
    public CollisionShape3D? CollisionShape3DRepresentation {get; private set;}

    // PRESET SHAPES

    public static readonly BoxMesh BoxShapeMesh = new() {Size = Vector3.One};
    public static readonly SphereMesh SphereShapeMesh = new() {Radius = 0.5f};
    public static readonly CylinderMesh CylinderShapeMesh = new() {TopRadius = 0.5f, BottomRadius = 0.5f};

    public static readonly BoxShape3D BoxShapeCollider = new() {Size = Vector3.One};
    public static readonly SphereShape3D SphereShapeCollider = new() {Radius = 0.5f};
    public static readonly CylinderShape3D CylinderShapeCollider = new() {Radius = 0.5f};

    // PRESET MATERIALS

    public static readonly StandardMaterial3D SmoothPlasticMaterial = new();
    public static readonly StandardMaterial3D GrassMaterial = ResourceLoader.Load<StandardMaterial3D>("Editor/PresetAssets/Materials/Grass/Material.tres");

    private static Mesh GetMesh(string Shape) => Shape switch
    {
        "Box" => BoxShapeMesh,
        "Sphere" => SphereShapeMesh,
        "Cylinder" => CylinderShapeMesh,
        _ => BoxShapeMesh
    };

    private static Shape3D GetShape(string Shape) => Shape switch
    {
        "Box" => BoxShapeCollider,
        "Sphere" => SphereShapeCollider,
        "Cylinder" => CylinderShapeCollider,
         _ => BoxShapeCollider
    };

    private static Material GetMaterial(string MaterialID) => MaterialID switch
    {
        "SmoothPlastic" => SmoothPlasticMaterial,
        "Grass" => GrassMaterial,
        _ => SmoothPlasticMaterial
    };

    protected override Node? InternalCreateNode(bool addToTree = true)
    {
        base.InternalCreateNode(addToTree);

        StaticBody3DRepresentation = new StaticBody3D();

        MeshInstance3DRepresentation = new MeshInstance3D()
        {
            Mesh = GetMesh(Shape)
        };
        StaticBody3DRepresentation?.AddChild(MeshInstance3DRepresentation);

        CollisionShape3DRepresentation = new CollisionShape3D()
        {
            Shape = GetShape(Shape)
        };
        StaticBody3DRepresentation?.AddChild(CollisionShape3DRepresentation);

        Node3DRepresentation?.AddChild(StaticBody3DRepresentation);

        if (StaticBody3DRepresentation != null) StaticBody3DRepresentation.Scale = Size;

        UpdateMeshMaterial();

        return Node3DRepresentation;
    }

    protected override void UpdateNode()
    {
        base.UpdateNode();
    }
}