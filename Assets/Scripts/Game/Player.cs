using Godot;
using System;
using System.Runtime.CompilerServices;

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
	private Area2D attackHitBox;
	private bool isAttacking;
	private Timer attackTimer;
	//private float attackOffset;  // Maybe Get ride of this?


	public int GetPlayerHealth() {
		return health;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		heading = new Vector2();
		facing = FACING_DIRECTION.Right;
		health = 3;
		dead = false;


		hasMoved = false;
		attackCount = 0;
		attackHitBox = GetNode<Area2D>("Area2D");
		attackTimer = GetNode<Timer>("%Timer");
		isAttacking = false;
		attackTimer.OneShot = true;
		

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

    public override void _PhysicsProcess(double delta)
    {
		if (!dead)
		{
            GetInput();
            MoveAndCollide(Velocity * (float)delta);
            walkAnimation();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		// Check if we're attacking
		if (Input.IsActionJustPressed("attack")) //&& !isAttacking)
		{
			isAttacking = true;
			// Rotate the hitbox to face the mouse
			//Vector2 playerToMouse = this.Position.DirectionTo(this.GetGlobalMousePosition());
			//float radians = playerToMouse.AngleTo(heading);
			//attackHitBox.Rotate(radians);
			Vector2 mousePOSinPlayer = this.GetLocalMousePosition() * this.Transform;

			attackHitBox.Monitoring = true;
			attackTimer.Start(1.0f);
			GD.Print("Attacked!");
			GD.Print($"Player Position: {this.Position}");
			GD.Print($"MousePosition: {mousePOSinPlayer}");
		}
		if (attackTimer.TimeLeft == 0)
		{
			isAttacking = false;
			attackHitBox.Monitoring = false;
		}
	}
	public void GetInput()
	{
		// Get Vector returns a vector based off the inputs, with a length of 1 (normalized)
		heading = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if(heading == Vector2.Zero)
		{
			hasMoved = false;
		}
		else
		{
			hasMoved = true;
		}
		Velocity = heading * speed;

		
	}
	private void updateFacing()
	{
		if (heading.X > 0 && heading.Y == 0)
		{
			facing = FACING_DIRECTION.Right;
			if(attackHitBox.Position.X < 0)
			{
				attackHitBox.Position = attackHitBox.Position.Reflect(Vector2.Up);
			}
		}
		else if (heading.X < 0 && heading.Y == 0)
		{
			facing = FACING_DIRECTION.Left;
			if(attackHitBox.Position.X > 0)
			{
                attackHitBox.Position = attackHitBox.Position.Reflect(Vector2.Up);
            }
			
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
	public void takeDamage(int damage)
	{
		if (health > 0)
		{
			health -= damage;
			if (health <= 0)
			{
				health = 0;
				on_Death();
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
		// Currently removed for testing
		/*if(!hasMoved)
		{
			return;
		}*/
		// If we have any overlapping bodies in the attack hit box
        if (attackHitBox.GetOverlappingBodies().Count > 0 && isAttacking)
        {
			Godot.Collections.Array<Node2D> overlapList = attackHitBox.GetOverlappingBodies();
            for(int i = 0; i > overlapList.Count; i++)
			{
				// Check for enemies and Deal damage
				if (overlapList[i].HasMethod("TakeDamage"))
				{
					BaseEnemyAI temp = (BaseEnemyAI)overlapList[i];
					temp.TakeDamage(1);
				}
			}
        }
		attackHitBox.GetChild<CollisionShape2D>(0).DebugColor = Colors.Red;
    }
	public void on_area_2d_area_entered(Area2D collision)
	{
		// Vector math to check if the mouse is facing towards 
		//Vector2 mousePOSinWorld = (GetViewport().GetScreenTransform() * this.GetCanvasTransform()).AffineInverse() * cursor.Position;
		Vector2 playerToEnemy = this.Position.DirectionTo(collision.Position);
		Vector2 playerToMouse = this.Position.DirectionTo(this.GetGlobalMousePosition() * this.Transform);
        GD.Print($"Dot Product Result: {playerToEnemy.Dot(playerToMouse)}");
		//attackHitBox.Rotate(playerToEnemy.Dot(playerToMouse));
        if (playerToEnemy.Dot(playerToMouse) <= 0.0f)
		{
            if (collision.Owner.HasMethod("TakeDamage") && isAttacking)
            {
                BaseEnemyAI temp = (BaseEnemyAI)collision.Owner;
                temp.TakeDamage(100);
            }
        }
		
	}
	public void on_Death()
	{
		dead = true;
		animatedSprite.Visible = false;
	}
}
