using System;
using Godot;
using GodotShell.Runtime;

namespace GodotShell.UI;

public partial class Terminal : VBoxContainer
{
    [Export] public VBoxContainer OutputContainer;
    [Export] public LineEdit InputTextBox;

    public GDSRuntime RUNTIME = new GDSRuntime().SetupLib();

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        
        InputTextBox.TextSubmitted += text =>
        {
            try
            {
                RUNTIME.Execute(text);
            } catch (Exception exception)
            {
                RUNTIME.Log($"[color=red]{exception.Message}");
            }

            InputTextBox.Text = "";
        };

        RUNTIME.Logged += message =>
            OutputContainer.AddChild(new RichTextLabel()
            {
                Text = message,
                FitContent = true,
                BbcodeEnabled = true,
                AutowrapMode = TextServer.AutowrapMode.Off
            });
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("debug_terminal"))
            Visible = !Visible;
    }
}