using Godot;
using System;

namespace FaF.UI.Common.AnimatedHover;

public partial class AnimatedHoverButton : Button
{
	[Export] public double EnterTweenDuration = 0.1;
	[Export] public double ExitTweenDuration = 0.05;
	[Export] public Vector2 HoveredOffset = Vector2.Right * 40;
	[Export] public Vector2 HoveredScale = Vector2.One * 1.05f;
	[Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;
	[Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Quart;
	[Export] public Control EffectNode;

	private Tween EnterTween;
	private Tween ExitTween;

	public override void _Ready()
	{
		base._Ready();

		EffectNode ??= this;

		EffectNode.OffsetTransformEnabled = true;

		MouseEntered += () =>
		{
			ExitTween?.Kill();

			EnterTween = EffectNode.CreateTween();
			EnterTween.TweenProperty(EffectNode, "offset_transform_position", HoveredOffset, EnterTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
			EnterTween.Parallel().TweenProperty(EffectNode, "offset_transform_scale", HoveredScale, EnterTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
		};

		MouseExited += () =>
		{
			EnterTween?.Kill();

			ExitTween = EffectNode.CreateTween();
			ExitTween.TweenProperty(EffectNode, "offset_transform_position", Vector2.Zero, ExitTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
			ExitTween.Parallel().TweenProperty(EffectNode, "offset_transform_scale", Vector2.One, ExitTweenDuration).SetTrans(TransitionType).SetEase(EaseType);
		};
	}
}