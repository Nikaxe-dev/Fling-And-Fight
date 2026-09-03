using Godot;

namespace FaF.Editor.UI.Properties.Selectors;

public partial class FloatPropertySelector : PropertySelector<float>
{
    [Export] public required SpinBox Value;

    protected override float GetInputValue() => (float)Value.Value;
    protected override bool IsInputValid() => true;
    protected override void SetVisualTo(float value) => Value.Value = value;

    public override void _Ready()
    {
        base._Ready();
        Value.ValueChanged += (value) => OnInputValueChanged();
    }

    protected override void SetVisualToDifferent() => Value.Value = 0;
}