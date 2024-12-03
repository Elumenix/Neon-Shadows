using Godot;
using System;

public partial class DashGhost : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Color Invisible = this.Modulate;
		// Same color, but with an alpha of 0
		Invisible.A = 0;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", Invisible, 0.3).SetTrans(Tween.TransitionType.Quart);
		tween.TweenCallback(Callable.From(this.QueueFree));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
