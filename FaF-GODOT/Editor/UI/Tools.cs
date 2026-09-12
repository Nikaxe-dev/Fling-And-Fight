using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FaF.Editor.DataModel;
using FaF.Editor.DataModel.Physical;
using FaF.Editor.Gizmos;
using Gizmo3DPlugin;
using Godot;
using Godot.Collections;

namespace FaF.Editor.UI;

public partial class Tools : HBoxContainer
{
    [Export] public Button SelectButton;
    [Export] public Button MoveButton;
    [Export] public Button ScaleButton;
    [Export] public Button RotateButton;

    [Export] public SubViewportContainer ViewportContainer;

    public enum ToolType
    {
        None,
        Select,
        Move,
        Scale,
        Rotate
    }

    private void RefreshVisuals()
    {
        foreach (var item in GetChildren())
        {
            if (item is Button button)
            {
                button.Flat = true;
            }
        }

        if (CurrentTool == ToolType.Select) SelectButton.Flat = false;
        else if (CurrentTool == ToolType.Move) MoveButton.Flat = false;
        else if (CurrentTool == ToolType.Scale) ScaleButton.Flat = false;
        else if (CurrentTool == ToolType.Rotate) RotateButton.Flat = false;

        // if (ExplorerTree.SelectedInstances.Count > 0) MoveToolGizmo3D.Enable();
        // else MoveToolGizmo3D.Disable();

        ToolGizmo3D.Visible = true;
        ToolGizmo3D.Targets = ExplorerTree.SelectedInstances;
        if (CurrentTool == ToolType.Select) ToolGizmo3D.Targets = [];
        else if (CurrentTool == ToolType.Move) ToolGizmo3D.Mode = Gizmo3D.ToolMode.Move;
        else if (CurrentTool == ToolType.Rotate) ToolGizmo3D.Mode = Gizmo3D.ToolMode.Rotate;
        else if (CurrentTool == ToolType.Scale) ToolGizmo3D.Mode = Gizmo3D.ToolMode.Scale;
    }

    private void SwitchToTool(ToolType tool)
    {
        if (CurrentTool == tool) CurrentTool = ToolType.None;
        else CurrentTool = tool;
        RefreshVisuals();
    }

    public static ToolType CurrentTool {get; private set;} = ToolType.None;

    public static Tools Instance {get; private set;}

    public override void _Ready()
    {
        SwitchToTool(ToolType.Select);

        SelectButton.Pressed += () => SwitchToTool(ToolType.Select);
        MoveButton.Pressed += () => SwitchToTool(ToolType.Move);
        ScaleButton.Pressed += () => SwitchToTool(ToolType.Scale);
        RotateButton.Pressed += () => SwitchToTool(ToolType.Rotate);

        ViewportContainer.MouseEntered += () => MouseInViewport = true;
        ViewportContainer.MouseExited += () => MouseInViewport = false;

        EditorRoot.Instance.EDITOR_GIZMOS.AddChild(SelectRaycastGizmo);

        ExplorerTree.SelectionChanged += OnSelectionChanged;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("editor_tool_select")) SwitchToTool(ToolType.Select);
        if (@event.IsActionPressed("editor_tool_move")) SwitchToTool(ToolType.Move);
        if (@event.IsActionPressed("editor_tool_scale")) SwitchToTool(ToolType.Scale);
        if (@event.IsActionPressed("editor_tool_rotate")) SwitchToTool(ToolType.Rotate);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("editor_select")) OnClick();
        if (@event is InputEventMouseMotion) OnMouseMove();
    }

    private static Dictionary CastRayFromMouse(Viewport viewport)
    {
        PhysicsDirectSpaceState3D space = viewport.World3D.DirectSpaceState;
        Vector2 mousePos = viewport.GetMousePosition();

        Vector3 rayOrigin = EditorRoot.Instance.EDITOR_CAMERA.ProjectRayOrigin(mousePos);
        Vector3 rayEnd = rayOrigin + EditorRoot.Instance.EDITOR_CAMERA.ProjectRayNormal(mousePos) * 10000;

        PhysicsRayQueryParameters3D rayParams = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        rayParams.CollideWithAreas = true;

        return space.IntersectRay(rayParams);
    }

    private bool MouseInViewport = false;

    private SelectRaycastGizmo3D SelectRaycastGizmo = new() {Name = "SelectRaycastResultGizmo"};

    [Export] private SelectionBoxGizmo3D SelectingSelectionBoxGizmo3D;
    [Export] private SelectionBoxGizmo3D SelectedSelectionBoxGizmo3D;

    [Export] private SelectionGizmo3D ToolGizmo3D;

    private void OnClick()
    {
        if (CurrentTool == ToolType.None || !MouseInViewport || ToolGizmo3D.Hovering || ToolGizmo3D.Editing) return;

        if (HoveredOverInstance != null)
        {
            if (Input.IsActionPressed("editor_select_component")) ExplorerTree.Instance.SelectInstance(HoveredOverInstance);
            else ExplorerTree.Instance.SelectInstance(ResolveInstanceSelection(HoveredOverInstance));
        }
        else ExplorerTree.Instance.DeselectAll();
    }

    private void OnSelectionChanged(Instance[] selected)
    {
        SelectedSelectionBoxGizmo3D.Targets = [];
        foreach (var item in selected)
        {
            if (item.NodeRepresentation != null) SelectedSelectionBoxGizmo3D.Targets.Add(item.NodeRepresentation);
        }
        SelectedSelectionBoxGizmo3D.Apply();

        RefreshVisuals();

        ToolGizmo3D.Targets.RemoveAll(_ => true);
        ToolGizmo3D.Targets.AddRange(selected);
    }

    private static Instance ResolveInstanceSelection(Instance instance)
    {
        Instance topModel = null;

        foreach (var ancestor in instance.Ancestry)
        {
            if (ancestor.ClassName == "Model") topModel = ancestor;
        }

        if (topModel != null)
        {
            return topModel;
        }

        return instance;
    }

    private Instance HoveredOverInstance;
    public static StaticBody3D HoveredOverGizmoDragger {get; private set;} = null;

    private void OnMouseMove()
    {
        if (CurrentTool == ToolType.None || !MouseInViewport) return;

        var raycastResult = CastRayFromMouse(EditorRoot.Instance.VIEWPORT);
        var gizmoRaycastResult = CastRayFromMouse(EditorRoot.Instance.GIZMO_VIEWPORT);

        if (raycastResult.Count > 0 && gizmoRaycastResult.Count == 0)
        {
            SelectRaycastGizmo.GlobalPosition = (Vector3)raycastResult["position"];
            
            StaticBody3D bodyNode = (StaticBody3D)(GodotObject)raycastResult["collider"];
            HoveredOverGizmoDragger = null;

            Instance bodyInstance = DataModel.Instance.GetInstanceFromNode(bodyNode.GetParent());
            HoveredOverInstance = bodyInstance;

            if (Input.IsActionPressed("editor_select_component")) SelectingSelectionBoxGizmo3D.Targets = [bodyInstance.NodeRepresentation];
            else SelectingSelectionBoxGizmo3D.Targets = [ResolveInstanceSelection(bodyInstance).NodeRepresentation];

            if ((Input.IsActionPressed("editor_select_component") && ExplorerTree.SelectedInstances.Contains(bodyInstance)) || (!Input.IsActionPressed("editor_select_component") && ExplorerTree.SelectedInstances.Contains(ResolveInstanceSelection(bodyInstance))))
            {
                SelectingSelectionBoxGizmo3D.Targets = [];
            }

            SelectingSelectionBoxGizmo3D.Apply();
        } else
        {
            HoveredOverInstance = null;

            if (gizmoRaycastResult.Count > 0) HoveredOverGizmoDragger = (StaticBody3D)(GodotObject) gizmoRaycastResult["collider"];
            else HoveredOverGizmoDragger = null;
            
            SelectingSelectionBoxGizmo3D.Targets = [];
            SelectingSelectionBoxGizmo3D.Apply();
        }
    }
}
