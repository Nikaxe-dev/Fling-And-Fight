using Godot;
using System;

public partial class AnimatedHover : Control
{
	[Export] public double TweenDuration = 0.1;
	[Export] public Vector2 HoveredOffset = Vector2.Right * 40;
	[Export] public Vector2 HoveredScale = Vector2.One * 1.1f;

	public override void _Ready()
	{
		base._Ready();

		OffsetTransformEnabled = true;

		

		MouseEntered += () =>
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "offset_transform_position", HoveredOffset, TweenDuration);
			tween.Parallel();
			tween.TweenProperty(this, "offset_transform_scale", HoveredScale, TweenDuration);
		};

		MouseExited += () =>
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "offset_transform_position", Vector2.Zero, TweenDuration);
			tween.Parallel();
			tween.TweenProperty(this, "offset_transform_scale", Vector2.One, TweenDuration);
		};
	}
}
