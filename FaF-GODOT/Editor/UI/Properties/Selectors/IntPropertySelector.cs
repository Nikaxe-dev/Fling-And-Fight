using Godot;

namespace FaF.Editor.UI.Properties.Selectors;

public partial class IntPropertySelector : PropertySelector<int>
{
    [Export] public required SpinBox Value;

    protected override int GetInputValue() => (int)Value.Value;
    protected override bool IsInputValid() => true;
    protected override void SetVisualTo(int value) => Value.Value = value;

    public override void _Ready()
    {
        base._Ready();
        Value.ValueChanged += (value) => OnInputValueChanged();
    }

    protected override void SetVisualToDifferent() => Value.Value = 0;

    protected override void MakeReadonly()
    {
        Value.Editable = false;
    }
}