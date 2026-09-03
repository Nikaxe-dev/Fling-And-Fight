using Godot;

namespace FaF.Editor.UI;

public partial class SubmitRemoveFocus : LineEdit
{
    public override void _Ready()
    {
        TextSubmitted += (_) =>
        {
            ReleaseFocus();
        };
    }
}