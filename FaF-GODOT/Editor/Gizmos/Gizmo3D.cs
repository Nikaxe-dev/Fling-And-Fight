using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FaF.Editor.Gizmos;

[GlobalClass]
public abstract partial class FaFGizmo3D : Node3D
{
    protected static void UpdateLine(MeshInstance3D line, Vector3 start, Vector3 end, Color color)
    {
        ImmediateMesh mesh = new();

        OrmMaterial3D material = new()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = color
        };

        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
        mesh.SurfaceAddVertex(start);
        mesh.SurfaceAddVertex(end);
        mesh.SurfaceEnd();

        line.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        line.Mesh = mesh;
    }

    protected static void UpdateLine(ImmediateMesh mesh, Vector3 start, Vector3 end)
    {
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        mesh.SurfaceAddVertex(start);
        mesh.SurfaceAddVertex(end);
        mesh.SurfaceEnd();
    }

    protected void CreateLine(ImmediateMesh mesh, Vector3 start, Vector3 end)
    {
        UpdateLine(mesh, start, end);
    }

    protected MeshInstance3D CreateLine(Vector3 start, Vector3 end, Color color)
    {
        MeshInstance3D line = new();
        UpdateLine(line, start, end, color);
        return line;
    }

    protected static readonly Vector3[] CUBE_VERTICES = [
        // front 4 vertices
        new Vector3(-1,-1,1)/2,
        new Vector3(1,-1,1)/2,
        new Vector3(1,1,1)/2,
        new Vector3(-1,1,1)/2,

        // back 4 vertices
        new Vector3(-1,-1,-1)/2,
        new Vector3(1,-1,-1)/2,
        new Vector3(1,1,-1)/2,
        new Vector3(-1,1,-1)/2,
    ];

    protected static void CreateLineLoop(ImmediateMesh mesh, Vector3[] vertices)
    {
        List<Vector3> LoopedVertices = [.. vertices];
        LoopedVertices.Add(vertices[0]);

        mesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
        foreach (var vertex in LoopedVertices)
        {
            mesh.SurfaceAddVertex(vertex);
        }
        mesh.SurfaceEnd();
    }

    protected void UpdateOutlineBox(MeshInstance3D box, Vector3 position, Basis basis, Vector3 size, Color color, float thickness, bool visibleThroughWalls)
    {
        ImmediateMesh mesh = new();
        
        OrmMaterial3D material = new()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = color,
            NoDepthTest = visibleThroughWalls,
        };

        Transform3D transform = new(basis, position);

        for (int i = 0; i<CUBE_VERTICES.Length; i++)
        {
            CUBE_VERTICES[i] = transform * CUBE_VERTICES[i];
        }

        CreateLineLoop(mesh, [CUBE_VERTICES[0],CUBE_VERTICES[1],CUBE_VERTICES[2],CUBE_VERTICES[3]]);
        CreateLineLoop(mesh, [CUBE_VERTICES[4],CUBE_VERTICES[5],CUBE_VERTICES[6],CUBE_VERTICES[7]]);
        for (int i = 0; i<4; i++)
        {
            CreateLine(mesh, CUBE_VERTICES[i], CUBE_VERTICES[i+4]);
        }

        box.Scale = size;

        box.MaterialOverride = material;
        box.Mesh = mesh;
    }

    protected MeshInstance3D CreateOutlineBox(Vector3 position, Basis basis, Vector3 size, Color color, float thickness, bool visibleThroughWalls)
    {
        MeshInstance3D box = new();
        UpdateOutlineBox(box, position, basis, size, color, thickness, visibleThroughWalls);
        return box;
    }

    protected static void UpdatePoint(MeshInstance3D point, Vector3 position, float radius, Color color)
    {
        OrmMaterial3D material = new()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = color
        };

        SphereMesh mesh = new()
        {
            Radius = radius,
            Height = radius*2,
            Material = material
        };

        point.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        point.Position = position;

        point.Mesh = mesh;
    }

    protected static MeshInstance3D CreatePoint(Vector3 position, float radius, Color color)
    {
        MeshInstance3D point = new();
        UpdatePoint(point, position, radius, color);
        return point;
    }

    protected static void UpdateBox(MeshInstance3D box, Vector3 position, Vector3 size, Color color)
    {
        OrmMaterial3D material = new()
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = color
        };

        BoxMesh mesh = new()
        {
            Size = size,
            Material = material
        };

        box.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        box.Position = position;

        box.Mesh = mesh;
    }

    protected static MeshInstance3D CreateBox(Vector3 position, Vector3 size, Color color)
    {
        MeshInstance3D box = new();
        UpdateBox(box, position, size, color);
        return box;
    }

    protected bool Enabled = true;

    public virtual void Enable() {
        Visible = true;
        Enabled = true;
    }

    public virtual void Disable()
    {
        Visible = false;
        Enabled = false;
    }
}