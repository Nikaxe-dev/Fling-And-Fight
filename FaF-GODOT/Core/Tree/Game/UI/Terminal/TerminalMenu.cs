using System;
using FaF.Core.Extensions;
using Godot;

namespace FaF.Core.Tree.Game.UI.Menu;

public partial class TerminalMenu : VBoxContainer
{
    [Export] public VBoxContainer OutputContainer;
    [Export] public LineEdit InputTextBox;

    public override void _Ready()
    {
        base._Ready();
        Visible = false;

        InputTextBox.TextSubmitted += text =>
        {
            this.GetGame().CommandService.ExecuteCommand(text);
            InputTextBox.Text = "";
        };

        this.GetGame().CommandService.Logged += message =>
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