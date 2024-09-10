using Godot;
using System;

public partial class Player : Node
{
	// Fields
	private Vector2 heading;
	private Vector2 velocity;
	private float maxSpeed = 5.0f;
	private float speed;
	public Sprite2D sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		heading = new Vector2();
		velocity = new Vector2();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		velocity = heading * maxSpeed;
		GD.Print($"The Velocity is X{velocity.X}, Y{velocity.Y}");
	}
}
