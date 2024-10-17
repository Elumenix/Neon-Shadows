using Godot;
using System;

public partial class Projectile : RigidBody2D
{
	private int _damage;
	private float _speed;
	public Vector2 Heading;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_damage = 50;
		_speed = 165.0f;
		Heading = Vector2.Zero; 
	}

    public override void _PhysicsProcess(double delta)
    {
		// Moves the projectile to the "Right" but rotated so it will actually move where the player aimed.
		var collision = MoveAndCollide((Vector2.Right* _speed).Rotated(this.Rotation) * (float)delta);
		// Checks for collisions and handles them
		if(collision != null)
		{
			// Deals damage if the collision is with an enemy
			if (collision.GetCollider() is BaseEnemyAI)
			{
				BaseEnemyAI temp = (BaseEnemyAI)collision.GetCollider();
				temp.TakeDamage(_damage);
            }
			else if(collision.GetCollider() is DroneAI)
			{
				DroneAI temp = (DroneAI)collision.GetCollider();
				temp.TakeDamage(_damage);
			}
			// If it collides with anything other than the player delete itself
			// Line could be changed since it no longer has a collision layer in common with player
			if (!(collision.GetCollider() is Player))
			{
                QueueFree();
            }
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
	
	/// <summary>
	/// Simple method that's called when the projectile leaves the screen to delete itself
	/// </summary>
	public void _on_visible_on_screen_enabler_2d_screen_exited()
	{
		QueueFree();
	}
}
