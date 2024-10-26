using Godot;
using System;

public partial class Barrel : AnimatedSprite2D
{

	private Explosion _explosion;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Play("default");
	}
	public void _onHit()
	{
		// Spawn explosion

		// play explosion animation
		Play("default");

	}

	public void ExplosionEnd()
	{
		QueueFree();
	}
}
