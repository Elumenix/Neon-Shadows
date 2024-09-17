using Godot;
using System;

enum FACING_DIRECTION { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }

public partial class Player : CharacterBody2D
{
	// Fields
	private Vector2 heading;
	private float maxSpeed = 50.0f;
	private float speed = 50.0f;
	private AnimatedSprite2D animatedSprite;
	private FACING_DIRECTION facing;

	// Health related fields
	private int health;
	private bool dead;
	public bool isDead { get { return dead; } }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		heading = new Vector2();
		facing = FACING_DIRECTION.Right;
		health = 3;
		dead = false;

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

    

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{


		// "Friction"
		//heading = Vector2.Zero;
		//// Recieve inputs
		//if (Input.IsActionPressed("move_up"))
		//{
		//	heading += Vector2.Up;
		//}
		//if (Input.IsActionPressed("move_down"))
		//{
		//	heading += Vector2.Down;
		//}
		//if (Input.IsActionPressed("move_left"))
		//{
		//	heading += Vector2.Left;
		//}
		//if (Input.IsActionPressed("move_right"))
		//{
		//	heading += Vector2.Right;
		//}
		//
		//// Noralize heading
		//heading = heading.Normalized();
		//velocity = heading * maxSpeed * (float)delta;
		//GD.Print($"The Velocity is X{velocity.X}, Y{velocity.Y}");
		//Translate(velocity);

		GetInput();
		MoveAndSlide();
		//walkAnimation();

		//GD.Print($"Facing {facing}");
	}
	public void GetInput()
	{
		// Get Vector returns a vector based off the inputs, with a length of 1 (normalized)
		heading = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = heading * speed;
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
		else if (heading.X == 0 && heading.Y < 0)
		{
			facing = FACING_DIRECTION.Up;
		}
		else if (heading.X == 0 && heading.Y > 0)
		{
			facing = FACING_DIRECTION.Down;
		}
		else if(heading.X > 0 && heading.Y < 0)
		{
			facing = FACING_DIRECTION.UpRight;
		}
		else if(heading.X > 0 && heading.Y > 0)
		{
			facing = FACING_DIRECTION.DownRight;
		}
		else if(heading.X < 0 && heading.Y < 0)
		{
			facing = FACING_DIRECTION.UpLeft;
		}
		else if(heading.X < 0 && heading.Y > 0)
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
	private void walkAnimation()
	{
		updateFacing();
		switch (facing)
		{
			case (FACING_DIRECTION.Up): { 
					animatedSprite.Play("walk_up");
					break;
				}
			case (FACING_DIRECTION.Left):
				{
					animatedSprite.Play("walk_left");
					break;
				}
			case (FACING_DIRECTION.Right):
				{
					animatedSprite.Play("walk_right");
					break;
				}
			case (FACING_DIRECTION.Down):
				{
					animatedSprite.Play("walk_down");
					break;
				}
			case (FACING_DIRECTION.UpLeft):
				{
					animatedSprite.Play("walk_upleft");
					break;
				}
			case (FACING_DIRECTION.DownLeft):
				{
					animatedSprite.Play("walk_downleft");
					break;
				}
			case (FACING_DIRECTION.UpRight):
				{
					animatedSprite.Play("walk_upright");
					break;
				}
			case (FACING_DIRECTION.DownRight):
				{
					animatedSprite.Play("walk_downright");
					break;
				}
			default: { 
					animatedSprite.Play("default");
					break;
				}
		}
	}
}
