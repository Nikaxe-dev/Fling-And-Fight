using System.Collections.Generic;
using Godot;

namespace FaF.Editor.Gizmos;

[GlobalClass]
public partial class SelectionBoxGizmo3D : FaFGizmo3D
{
    [Export] public Color OutlineColor = new (1,1,1);
    [Export] public float OutlineThickness = 5;
    [Export] public bool VisibleThroughWalls = false;

    public List<OutlineBoxGizmo3D> Outlines = [];
    public List<Node> Targets = [];

    protected void Unapply()
    {
        foreach (var item in Outlines)
        {
            if (IsInstanceValid(item)) item.QueueFree();
        }
        Outlines.RemoveAll(_ => true);
    }

    protected static Aabb GetNodeAabb(Node node, bool ExcludeTopLevelTransform = true)
    {
        Aabb bounds = new();

        if (node.IsQueuedForDeletion()) return bounds;

        if (node is VisualInstance3D visualInstance) bounds = visualInstance.GetAabb();

        foreach (var item in node.GetChildren())
        {
            var childBounds = GetNodeAabb(item, false);
            if (bounds.Size == Vector3.Zero) bounds = childBounds;
            else bounds = bounds.Merge(childBounds);
        }

        if (!ExcludeTopLevelTransform && node is Node3D node3D) bounds = node3D.Transform * bounds;
        
        return bounds;
    }

    public void Apply()
    {
        Unapply();

        foreach (var item in Targets)
        {
            OutlineBoxGizmo3D outline = new();

            Outlines.Add(outline);
            item.AddChild(outline, @internal: InternalMode.Back);

            outline.OutlineSize = GetNodeAabb(item).Size;
            outline.OutlineColor = OutlineColor;
            outline.VisibleThroughWalls = VisibleThroughWalls;

            if (!Enabled) outline.Disable();
        }
    }

    public override void Enable()
    {
        base.Enable();

        foreach (var outline in Outlines)
        {
            outline.Enable();
        }
    }

    public override void Disable()
    {
        base.Disable();

        foreach (var outline in Outlines)
        {
            outline.Disable();
        }
    }
}