using Godot;
using System;

public partial class Projectile : RigidBody2D
{
	private int _damage;
	private float _speed;
	public Vector2 Heading;
	private Area2D _area;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_damage = 50;
		_speed = 200.0f;
		Heading = Vector2.Zero;
		_area = GetNode<Area2D>("Area2D");
	}

    public override void _PhysicsProcess(double delta)
    {
		// Moves the projectile to the "Right" but rotated so it will actually move where the player aimed.
		var collision = MoveAndCollide((Vector2.Right* _speed).Rotated(this.Rotation) * (float)delta);
		// Checks for collisions and handles them
		if(collision != null)
		{
			QueueFree();
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

	public void OnCollision(Node2D collider)
	{
        // Deals damage if the collision is with an enemy
        if (collider is BaseEnemyAI temp)
        {
            temp.TakeDamage(_damage);
            QueueFree();
        }
        else if (collider is DroneAI drone)
        {
            drone.TakeDamage(_damage);
            QueueFree();
        }
		else if(collider is OozeAi Ooze)
		{
			Ooze.TakeDamage(_damage);
            QueueFree();
        }
        // If it collides with anything other than the player delete itself
        // Line could be changed since it no longer has a collision layer in common with player
        if (collider is Barrel barrel)
        {
            barrel._onHit();
            QueueFree();
        }
        
    }
}
