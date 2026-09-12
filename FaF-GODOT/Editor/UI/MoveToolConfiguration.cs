using FaF.Editor.Gizmos;
using Godot;
using System;

public partial class MoveToolConfiguration : VBoxContainer
{
    [Export] public CheckBox LocalSpaceCheckbox;
    [Export] public SpinBox SnapSpinBox;

    public override void _Ready()
    {
        LocalSpaceCheckbox.ButtonPressed = TransformGizmo3D.IsInLocalSpace;
        SnapSpinBox.Value = TransformGizmo3D.Snap;

        LocalSpaceCheckbox.Toggled += (isOn) =>
        {
            TransformGizmo3D.IsInLocalSpace = isOn;
        };

        SnapSpinBox.ValueChanged += (newValue) =>
        {
            TransformGizmo3D.Snap = (float)newValue;
        };
    }
}
