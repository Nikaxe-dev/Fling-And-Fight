using Godot;

namespace FaF.Editor.UI.Properties.Selectors;

public partial class StringPropertySelector : PropertySelector<string>
{
    [Export] public required LineEdit Value;

    protected override string GetInputValue() => Value.Text;
    protected override bool IsInputValid() => true;
    protected override void SetVisualTo(string value) => Value.Text = value;

    public override void _Ready()
    {
        base._Ready();
        Value.TextSubmitted += (newValue) => OnInputValueChanged();
    }
}
