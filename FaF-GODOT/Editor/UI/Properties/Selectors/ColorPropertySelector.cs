using Godot;

namespace FaF.Editor.UI.Properties.Selectors;

public partial class ColorPropertySelector : PropertySelector<Color>
{
    [Export] public required ColorPickerButton Value;

    protected override Color GetInputValue() => Value.Color;
    protected override bool IsInputValid() => true;
    protected override void SetVisualTo(Color value) => Value.Color = value;

    public override void _Ready()
    {
        base._Ready();
        Value.ColorChanged += (value) => OnInputValueChanged();
    }

    protected override void SetVisualToDifferent() => Value.Color = new Color(1,1,1);

    protected override void MakeReadonly()
    {
        Value.Disabled = true;
    }
}