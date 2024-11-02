using Godot;
using System;

public partial class Barrel : RigidBody2D
{
	private AnimatedSprite2D _sprite;

	private PackedScene _explosion = GD.Load<PackedScene>("res://Assets/Entities/Objects/Explosion.tscn");
	private bool _wasHit = false;
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
		if (_wasHit)
		{
			return;
		}
		_wasHit = true;
		// play explosion animation
		_sprite.Play("default");

        // Spawn explosion
		Explosion temp = (Explosion)_explosion.Instantiate();
		temp.Position = this.Position * this.Transform;
		AddChild(temp);
    }

    public void ExplosionEnd()
	{
		QueueFree();
	}
}
