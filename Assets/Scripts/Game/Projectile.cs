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
		_speed = 85.0f;
		Heading = Vector2.Zero; 
	}

    public override void _PhysicsProcess(double delta)
    {
		//ApplyForce((Vector2.Right * _speed).Rotated(this.Rotation));
        //Position += (Vector2.Right * _speed).Rotated(this.Rotation) * (float)delta;
		var collision = MoveAndCollide((Vector2.Right* _speed).Rotated(this.Rotation) * (float)delta);
		if(collision != null)
		{
			if (collision.GetCollider() is BaseEnemyAI)
			{
				BaseEnemyAI temp = (BaseEnemyAI)collision.GetCollider();
				temp.TakeDamage(_damage);
            }
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

	public void _on_body_entered(Node collision)
	{
		if (collision is BaseEnemyAI)
		{
			BaseEnemyAI temp = collision.Owner as BaseEnemyAI;
			temp.TakeDamage(_damage);
            QueueFree();
        }
        else
        {
			GD.Print("Non-Enemy Collision");
        }
		
	}

	public void _on_visible_on_screen_enabler_2d_screen_exited()
	{
		QueueFree();
	}
}
