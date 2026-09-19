using FaF.UI.MainMenu;
using Godot;
using System;

public partial class CloseButton : AnimatedHoverButton
{
    public override void _Ready()
    {
        base._Ready();

        Pressed += () =>
        {
            MainMenuManager.Instance.SwitchToPanel(MainMenuManager.Instance.TitleScreenPanel);
        };
    }
}
