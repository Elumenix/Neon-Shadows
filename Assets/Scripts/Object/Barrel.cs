using Godot;
using System;

public partial class Barrel : RigidBody2D
{

	private Explosion _explosion;
	private AnimatedSprite2D _sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Play("default");
	}
    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(Vector2.Zero, true);
		if(collision != null)
		{
			if(collision.GetCollider() is PlayerSlash || collision.GetCollider() is Projectile)
			{
				_onHit();
			}
		}
    }
    public void _onHit()
	{
		// Spawn explosion
		GD.Print("EXPLOSION!");

		// play explosion animation
		_sprite.Play("default");

	}

	public void ExplosionEnd()
	{
		QueueFree();
	}
}
