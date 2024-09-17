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

	// attack realted fields
	private bool hasMoved;
	private int attackCount; // Where the player is in the combo attacks. 0 means no attacks yet.


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		heading = new Vector2();
		facing = FACING_DIRECTION.Right;
		health = 3;
		dead = false;


		hasMoved = false;
		attackCount = 0;
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndCollide(Velocity * (float)delta);
        walkAnimation();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}
	public void GetInput()
	{
		// Get Vector returns a vector based off the inputs, with a length of 1 (normalized)
		heading = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if(heading != Vector2.Zero)
		{
			hasMoved = true;
		}
		else
		{
			hasMoved = false;
		}
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
	private void attack()
	{
		// Can't attack and move?
		if(!hasMoved)
		{
			return;
		}
	}
}
