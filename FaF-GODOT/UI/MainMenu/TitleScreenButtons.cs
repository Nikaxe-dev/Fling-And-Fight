using Godot;
using System;

namespace FaF.UI.MainMenu;

public partial class TitleScreenButtons : VBoxContainer
{
    [Export] private Button ContinueButton;
    [Export] private Button PlayButton;
    [Export] private Button JoinButton;
    [Export] private Button SettingsButton;

    private void OnContinuePressed()
    {
        
    }

    private void OnPlayPressed()
    {
        MainMenuManager.Instance.SwitchToPanel(MainMenuManager.Instance.GameSelectionPanel);
    }

    private void OnJoinPressed()
    {
        
    }

    private void OnSettingsPressed()
    {
        
    }

    public override void _Ready()
    {
        base._Ready();

        ContinueButton.Pressed += OnContinuePressed;
        PlayButton.Pressed += OnPlayPressed;
        JoinButton.Pressed += OnJoinPressed;
        SettingsButton.Pressed += OnSettingsPressed;
    }
}
