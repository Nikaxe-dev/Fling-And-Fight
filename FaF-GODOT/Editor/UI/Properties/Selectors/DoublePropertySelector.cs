using Godot;

namespace FaF.Editor.UI.Properties.Selectors;

public partial class DoublePropertySelector : PropertySelector<double>
{
    [Export] public required SpinBox Value;

    protected override double GetInputValue() => Value.Value;
    protected override bool IsInputValid() => true;
    protected override void SetVisualTo(double value) => Value.Value = value;

    public override void _Ready()
    {
        base._Ready();
        Value.ValueChanged += (value) => OnInputValueChanged();
    }
}