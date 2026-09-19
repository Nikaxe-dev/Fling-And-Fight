using Godot;
using System;

namespace FaF.UI.MainMenu;

public partial class AnimatedHoverTextureRect : TextureRect
{
	[Export] public double EnterTweenDuration = 0.1;
	[Export] public double ExitTweenDuration = 0.05;
	[Export] public Vector2 HoveredOffset = Vector2.Right * 40;
	[Export] public Vector2 HoveredScale = Vector2.One * 1.05f;
	[Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;
	[Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Quart;

	private Tween EnterTween;
	private Tween ExitTween;

	public override void _Ready()
	{
		base._Ready();

		OffsetTransformEnabled = true;

		MouseEntered += () =>
		{
			ExitTween?.Kill();

			EnterTween = GetTree().CreateTween();
			EnterTween.TweenProperty(this, "offset_transform_position", HoveredOffset, EnterTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
			EnterTween.Parallel().TweenProperty(this, "offset_transform_scale", HoveredScale, EnterTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
		};

		MouseExited += () =>
		{
			EnterTween?.Kill();

			ExitTween = GetTree().CreateTween();
			ExitTween.TweenProperty(this, "offset_transform_position", Vector2.Zero, ExitTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
			ExitTween.Parallel().TweenProperty(this, "offset_transform_scale", Vector2.One, ExitTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
		};
	}
}
