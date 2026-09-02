using Godot;
using System;

namespace FaF.Editor.UI;

public partial class ViewportTexture : TextureRect
{
    [Export] public SubViewport viewport;

    private bool mouseOver = false;

    public override void _Ready()
    {
        MouseEntered += () => mouseOver = true;
        MouseExited += () => mouseOver = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouse mouseEvent && mouseOver)
        {
            viewport?.PushInput(mouseEvent, true);
        }
    }
}
