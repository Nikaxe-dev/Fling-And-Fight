using FaF.Game.Networking;
using Godot;
using System;

namespace FaF.Debug.Console;

/// <summary>
/// A simple in-game console UI for viewing all logs created by the FaFLogger static class.
/// </summary>
public partial class ConsoleUI : Control
{
    public static ConsoleUI Instance {get; private set;}

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
        label.AddThemeFontSizeOverride("bold_font_size", OUTPUT_FONT_SIZE);

        OutputContainer.AddChild(label);
    }

    public override void _Ready()
    {
        Instance = this;

        Visible = false;

        UserInput.GuiInput += _UserInput_Input;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_console"))
        {
            Visible = !Visible;
        }
    }

    private void _UserInput_Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_escape"))
        {
            UserInput.ReleaseFocus();
        }

        if (@event.IsActionPressed("textbox_accept"))
        {
            UserInput.Clear();
            UserInput.ReleaseFocus();
        }
    }
}
