using Godot;
using System;

enum FACING_DIRECTION { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }

public partial class Player : Area2D
{
	// Fields
	private Vector2 heading;
	private Vector2 velocity;
	private float maxSpeed = 50.0f;
	private float speed;
	public Sprite2D sprite;
	private FACING_DIRECTION facing;

	private int health;
	private bool dead;
	public bool isDead { get { return dead; } }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		heading = new Vector2();
		velocity = new Vector2();
		facing = FACING_DIRECTION.Right;
		health = 3;
		dead = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// "Friction"
		heading = Vector2.Zero;
		// Recieve inputs
		if (Input.IsActionPressed("move_up"))
		{
			heading += Vector2.Up;
		}
		if (Input.IsActionPressed("move_down"))
		{
			heading += Vector2.Down;
		}
		if (Input.IsActionPressed("move_left"))
		{
			heading += Vector2.Left;
		}
		if (Input.IsActionPressed("move_right"))
		{
			heading += Vector2.Right;
		}

		// Noralize heading
		heading = heading.Normalized();
		velocity = heading * maxSpeed * (float)delta;
		GD.Print($"The Velocity is X{velocity.X}, Y{velocity.Y}");
		Translate(velocity);

		// Update facing and apply change sprite accordingly
		updateFacing();

		GD.Print($"Facing {facing}");
	}
	private void updateFacing()
	{
		if (heading.X > 0 && heading.Y == 0)
		{
			facing = FACING_DIRECTION.Right;
		}
		else if (heading.X < 0 && heading.Y == 0)
		{
			facing = FACING_DIRECTION.Left;
		}
		else if (heading.X == 0 && heading.Y > 0)
		{
			facing = FACING_DIRECTION.Up;
		}
		else if (heading.X == 0 && heading.Y < 0)
		{
			facing = FACING_DIRECTION.Down;
		}
		else if(heading.X > 0 && heading.Y > 0)
		{
			facing = FACING_DIRECTION.UpRight;
		}
		else if(heading.X > 0 && heading.Y < 0)
		{
			facing = FACING_DIRECTION.DownRight;
		}
		else if(heading.X < 0 && heading.Y > 0)
		{
			facing = FACING_DIRECTION.UpLeft;
		}
		else if(heading.X < 0 && heading.Y < 0)
		{
			facing = FACING_DIRECTION.DownLeft;
		}
	}
	private void takeDamage(int damage)
	{
		if (health > 0)
		{
			health -= damage;
			if (health < 0)
			{
				health = 0;
				dead = true;
			}
		}

	}
}
