using Godot;
using System;

namespace FaF.Debug.Console;

public partial class ConsoleUI : Control
{
    public static ConsoleUI Instance;

    [Export] public required LineEdit UserInput;
    [Export] public required VBoxContainer OutputContainer;

    [Export] public required int OUTPUT_FONT_SIZE = 8;

    public void OutputRichString(string text)
    {
        var label = new RichTextLabel()
        {
            Text = text,
            FitContent = true,
            BbcodeEnabled = true,
        };

        label.AddThemeFontSizeOverride("normal_font_size", OUTPUT_FONT_SIZE);

        OutputContainer.AddChild(label);
    }

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("debug_console"))
        {
            Visible = !Visible;
        }
    }
}
