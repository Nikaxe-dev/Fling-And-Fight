using Godot;

namespace FaF.Editor.UI.Properties.Selectors;

public partial class Vector3PropertySelector : PropertySelector<Vector3>
{
    [Export] public LineEdit XLabel;
    [Export] public LineEdit YLabel;
    [Export] public LineEdit ZLabel;

    protected override Vector3 GetInputValue()
    {
        return new(float.Parse(XLabel.Text),float.Parse(YLabel.Text),float.Parse(ZLabel.Text));
    }

    protected override bool IsInputValid() => float.TryParse(XLabel.Text, out _) && float.TryParse(YLabel.Text, out _) && float.TryParse(ZLabel.Text, out _);

    protected override void SetVisualTo(Vector3 value)
    {
        XLabel.Text = value.X.ToString();
        YLabel.Text = value.Y.ToString();
        ZLabel.Text = value.Z.ToString();
    }

    public override void _Ready()
    {
        base._Ready();

        XLabel.TextSubmitted += (value) => OnInputValueChanged();
        YLabel.TextSubmitted += (value) => OnInputValueChanged();
        ZLabel.TextSubmitted += (value) => OnInputValueChanged();
    }

    protected override void SetVisualToDifferent()
    {
        XLabel.Text = "";
        YLabel.Text = "";
        ZLabel.Text = "";
    }
}