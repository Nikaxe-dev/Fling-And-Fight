using System;
using System.Collections.Generic;
using FaF.Editor.DataModel;
using FaF.Editor.DataModel.Physical;
using FaF.Editor.UI;
using Godot;

namespace FaF.Editor.Gizmos;

public partial class TransformGizmo3D : AutoScalingGizmo3D
{
    [Export] private StaticBody3D XDragger;
    [Export] private StaticBody3D YDragger;
    [Export] private StaticBody3D ZDragger;

    public static float Snap = 1;
    public static bool IsInLocalSpace = true;

    public readonly List<Instance> Targets = [];

    private static void SetDraggerEnabled(StaticBody3D dragger, bool Enabled)
    {
        foreach (var child in dragger.GetChildren())
        {
            if (child is CollisionShape3D collider)
            {
                collider.Disabled = !Enabled;
            }
        }
    }

    private void Apply()
    {
        Instance[] pointInstanceTargets = [.. Targets.FindAll(target => target is PointInstance)];

        Vector3 averagePosition = Vector3.Zero;
        Vector3 averageRotation = Vector3.Zero;

        foreach (Instance target in pointInstanceTargets)
        {
            if (target is PointInstance pointInstanceTarget)
            {
                averagePosition += pointInstanceTarget.GlobalPosition / pointInstanceTargets.Length;
                averageRotation += pointInstanceTarget.GlobalRotation / pointInstanceTargets.Length;
            }
        }

        GlobalPosition = averagePosition;
        if (IsInLocalSpace) GlobalRotation = PointInstance.DegToRad(averageRotation);
    }

    private Vector3 GetDraggingAxis()
    {
        if (DraggingDragger == XDragger)
            return IsInLocalSpace ? GlobalTransform.Basis.X.Normalized() : Vector3.Right;

        if (DraggingDragger == YDragger)
            return IsInLocalSpace ? GlobalTransform.Basis.Y.Normalized() : Vector3.Up;

        if (DraggingDragger == ZDragger)
            return IsInLocalSpace ? GlobalTransform.Basis.Z.Normalized() : Vector3.Back;

        return Vector3.Zero;
    }

    private Vector3? GetPlaneIntersectionPoint()
    {
        Camera3D camera = EditorRoot.Instance.EDITOR_CAMERA;

        Vector2 mousePosition = EditorRoot.Instance.VIEWPORT.GetMousePosition();

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);

        Vector3 axis = GetDraggingAxis();
        Vector3 planeNormal = axis.Cross(Vector3.Forward); // axis.Cross(camera.GlobalTransform.Basis.Z);

        if (planeNormal.LengthSquared() < 0.000001f)
        {
            planeNormal = axis.Cross(Vector3.Right);
        }

        planeNormal = planeNormal.Normalized();

        Plane plane = new(planeNormal, GlobalPosition);

        return plane.IntersectsRay(rayOrigin, rayDirection);
    }
    
    private Vector3 DragStartPoint;
    private Vector3 DragStartGizmoPosition;

    private readonly Dictionary<PointInstance, Vector3> DragStartPositions = [];

    private void SetStartPositions(Instance[] instances)
    {
        foreach (Instance target in instances)
        {
            if (target is PointInstance pointInstance)
                DragStartPositions[pointInstance] = pointInstance.GlobalPosition;
            else
                SetStartPositions([.. target.Children]);
        }
    }

    private void BeginDrag()
    {
        Vector3? intersection = GetPlaneIntersectionPoint();

        if (intersection == null) return;

        DragStartPoint = intersection.Value;
        DragStartGizmoPosition = GlobalPosition;

        DragStartPositions.Clear();
        SetStartPositions([.. Targets]);
    }

    private void Drag()
    {
        Vector3? currentIntersection = GetPlaneIntersectionPoint();

        if (currentIntersection == null) return;

        Vector3 axis = GetDraggingAxis();

        if (axis == Vector3.Zero) return;

        Vector3 movement = currentIntersection.Value - DragStartPoint;

        float distance = movement.Dot(axis);

        if (Snap > 0)
            distance = Mathf.Snapped(distance, Snap);
        
        foreach (var pair in DragStartPositions)
        {
            pair.Key.GlobalPosition = pair.Value + axis * distance;
        }
    }

    private StaticBody3D DraggingDragger;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Apply();

        if (DraggingDragger != null)
        {
            Drag();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // if ((Tools.HoveredOverGizmoDragger == XDragger || Tools.HoveredOverGizmoDragger == YDragger || Tools.HoveredOverGizmoDragger == ZDragger) && Input.IsMouseButtonPressed(MouseButton.Left))
        // {
        //     DraggingDragger = Tools.HoveredOverGizmoDragger;
        //     BeginDrag();
        // }

        // if (!Input.IsMouseButtonPressed(MouseButton.Left))
        // {
        //     DraggingDragger = null;
        //     DragStartPositions.Clear();
        // }

        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
        {
            if (mouseButton.Pressed)
            {
                if (Tools.HoveredOverGizmoDragger == XDragger || Tools.HoveredOverGizmoDragger == YDragger || Tools.HoveredOverGizmoDragger == ZDragger)
                {
                    DraggingDragger = Tools.HoveredOverGizmoDragger;
                    BeginDrag();
                }
            } else
            {
                DraggingDragger = null;
                DragStartPositions.Clear();
            }
        }
    }

    public override void Enable()
    {
        base.Enable();
        SetDraggerEnabled(XDragger, true);
        SetDraggerEnabled(YDragger, true);
        SetDraggerEnabled(ZDragger, true);
    }

    public override void Disable()
    {
        base.Disable();
        SetDraggerEnabled(XDragger, false);
        SetDraggerEnabled(YDragger, false);
        SetDraggerEnabled(ZDragger, false);
    }
}