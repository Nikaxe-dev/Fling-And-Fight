using System.Collections.Generic;
using FaF.Editor.DataModel;
using FaF.Editor.DataModel.Physical;
using FaF.Editor.UI;
using Godot;

namespace FaF.Editor.Gizmos;

[GlobalClass]
public partial class SelectionGizmo3D : Gizmo3DPlugin.Gizmo3D
{
    public List<Instance> Targets = [];

    private void Apply(List<Instance> targets)
    {
        ClearSelection();
        
        foreach (Instance target in Targets)
        {
            if (target is PointInstance pointInstance)
            {
                Select(pointInstance.Node3DRepresentation);
            }
        }

        Visible = true;
        Targets = ExplorerTree.SelectedInstances;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Apply(Targets);
    }

    public override void _Ready()
    {
        base._Ready();
        TransformBegin += (mode) => GD.Print($"Begin {(TransformMode) mode}");
        TransformChanged += (mode, value) => GD.Print($"Change {(TransformMode) mode}: {value}");
        TransformEnd += (mode) => GD.Print($"End {(TransformMode) mode}");
    }
}