using Godot;
using System;

namespace FaF.UI.MainMenu;

public partial class MainMenuManager : Control
{
    public static MainMenuManager Instance {get; private set;}

    [Export] public Control GameSelectionPanel;
    [Export] public Control TitleScreenPanel;

    [Export] public double TweenDuration = 0.25;

    [Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;
	[Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Quart;

    private void TweenOpen(Control panel) {
        panel.OffsetTransformEnabled = true;
        panel.Visible = true;

        Tween tween = panel.CreateTween();
        tween.TweenProperty(panel, "offset_transform_position", Vector2.Zero, TweenDuration).SetTrans(TransitionType).SetEase(EaseType);
    }

    private void TweenClosed(Control panel) {
        panel.OffsetTransformEnabled = true;

        Tween tween = panel.CreateTween();
        tween.TweenProperty(panel, "offset_transform_position", panel.GetViewportRect().Size*Vector2.Left, TweenDuration).SetTrans(TransitionType).SetEase(EaseType);

        tween.Finished += () => panel.Visible = false;
    }

    public void SwitchToPanel(Control panel)
    {
        foreach (var child in GetChildren())
        {
            if (child != panel && child is Control control)
            {
                TweenClosed(control);
            }
        }

        TweenOpen(panel);
    }

    public override void _Ready()
    {
        base._Ready();

        SwitchToPanel(TitleScreenPanel);

        Instance = this;
    }
}
