using Godot;
using System;
using System.Runtime.CompilerServices;

enum FACING_DIRECTION { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }

public partial class Player : CharacterBody2D
{
	// Fields
	private Vector2 _heading;
	private float _maxSpeed = 50.0f;
	private float _speed = 50.0f;
	private AnimatedSprite2D _animatedSprite;
	private FACING_DIRECTION _facing;

	// Health related fields
	private int _health;
	private bool _dead;
	private float _damageFrames;
	public bool IsDead { get { return _dead; } }

	// attack realted fields
	private bool _hasMoved;
	private int _attackCount; // Where the player is in the combo attacks. 0 means no attacks yet.
	private Area2D _attackHitBox;
	private bool _isAttacking;
	private Timer _attackTimer;
	//private float attackOffset;  // Maybe Get ride of this?

	// Dash Stuff
	private Dash _dash;
	private float _dashSpeed = 250.0f;
	private const float _DashDuration = 0.2f;

	// Ranged Stuff
	private int _ammo;
	private PackedScene _projectile = GD.Load<PackedScene>("res://Assets/Entities/Objects/PlayerProjectile.tscn");
	private Marker2D _marker;



	public int GetPlayerHealth() {
		return _health;
	}
	public bool GetEnemyCollisionMask {  get { return GetCollisionMaskValue(1); } }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_heading = new Vector2();
		_facing = FACING_DIRECTION.Right;
		_health = 3;
		_dead = false;
		_damageFrames = 0.0f;


		_hasMoved = false;
		_attackCount = 0;
		_attackHitBox = GetNode<Area2D>("Area2D");
		_attackTimer = GetNode<Timer>("%Timer");
		_isAttacking = false;
		_attackTimer.OneShot = true;
		

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		_dash = GetNode<Dash>("Dash");
		_marker = GetNode<Marker2D>("Marker2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_dead)
		{
			if(Input.IsActionJustPressed("dash") && _dash.CanDash && !_dash.IsDashing)
			{
				_dash.StartDash(_heading, _DashDuration);
			}
			GetInput();
			MoveAndCollide(Velocity * (float)delta);
			walkAnimation();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// reduce damage frames
		if(_damageFrames > 0.0f)
		{
			_damageFrames -= (float)delta;
			if(_damageFrames < 0.0f) { _damageFrames = 0.0f; }
		}

		// Check if we're attacking
		if (Input.IsActionJustPressed("attack_melee")) //&& !isAttacking)
		{
			_isAttacking = true;
			// Rotate the hitbox to face the mouse
			//Vector2 playerToMouse = this.Position.DirectionTo(this.GetGlobalMousePosition());
			//float radians = playerToMouse.AngleTo(heading);
			//attackHitBox.Rotate(radians);
			Vector2 mousePOSinPlayer = this.GetGlobalMousePosition();

			_attackHitBox.Monitoring = true;
			_attackTimer.Start(0.25f);

			// Change the hitbox color while attacking
			Color attackColor = new Color(Colors.Red, 0.4f);
			_attackHitBox.GetChild<CollisionShape2D>(0).DebugColor = attackColor;
			GD.Print("Attacked!");
			//GD.Print($"Player Position: {this.Position}");
			//GD.Print($"MousePosition: {mousePOSinPlayer}");

			
		}
		if (_attackTimer.TimeLeft == 0)
		{
			Color attackColor = new Color(Colors.Blue, 0.4f);
			_attackHitBox.GetChild< CollisionShape2D>(0).DebugColor= attackColor;
			_isAttacking = false;
			_attackHitBox.Monitoring = false;
		}

		if (Input.IsActionJustPressed("attack_ranged"))
		{
            Vector2 mousePOSinPlayer = this.GetGlobalMousePosition();
            // Create a Ranged Attack
            _marker.LookAt(mousePOSinPlayer);
            CreateProjectile();
		} 
	}
	public void GetInput()
	{
		// Get Vector returns a vector based off the inputs, with a length of 1 (normalized)
		_heading = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if(_heading == Vector2.Zero)
		{
			_hasMoved = false;
		}
		else
		{
			_hasMoved = true;
		}
		if (_dash.IsDashing) { Velocity = _heading * _dashSpeed; }
		else { Velocity = _heading * _speed; }

		
	}
	private void updateFacing()
	{
		if (_heading.X > 0 && _heading.Y == 0)
		{
			_facing = FACING_DIRECTION.Right;
		}
		else if (_heading.X < 0 && _heading.Y == 0)
		{
			_facing = FACING_DIRECTION.Left;
		}
		else if (_heading.X == 0 && _heading.Y < 0)
		{
			_facing = FACING_DIRECTION.Up;
		}
		else if (_heading.X == 0 && _heading.Y > 0)
		{
			_facing = FACING_DIRECTION.Down;
		}
		else if(_heading.X > 0 && _heading.Y < 0)
		{
			_facing = FACING_DIRECTION.UpRight;
		}
		else if(_heading.X > 0 && _heading.Y > 0)
		{
			_facing = FACING_DIRECTION.DownRight;
		}
		else if(_heading.X < 0 && _heading.Y < 0)
		{
			_facing = FACING_DIRECTION.UpLeft;
		}
		else if(_heading.X < 0 && _heading.Y > 0)
		{
			_facing = FACING_DIRECTION.DownLeft;
		}
	}
	public void takeDamage(int damage)
	{
		if(_damageFrames > 0.0f)
		{
			return;
		}
		FlashOnDamge();


		if (_health > 0)
		{
			_health -= damage;
			_damageFrames = 0.25f;
			if (_health <= 0)
			{
				_health = 0;
				on_Death();
			}
		}

	}
	private void walkAnimation()
	{
		updateFacing();
		switch (_facing)
		{
			case (FACING_DIRECTION.Up): { 
					_animatedSprite.Play("walk_up");
					break;
				}
			case (FACING_DIRECTION.Left):
				{
					_animatedSprite.Play("walk_left");
					break;
				}
			case (FACING_DIRECTION.Right):
				{
					_animatedSprite.Play("walk_right");
					break;
				}
			case (FACING_DIRECTION.Down):
				{
					_animatedSprite.Play("walk_down");
					break;
				}
			case (FACING_DIRECTION.UpLeft):
				{
					_animatedSprite.Play("walk_upleft");
					break;
				}
			case (FACING_DIRECTION.DownLeft):
				{
					_animatedSprite.Play("walk_downleft");
					break;
				}
			case (FACING_DIRECTION.UpRight):
				{
					_animatedSprite.Play("walk_upright");
					break;
				}
			case (FACING_DIRECTION.DownRight):
				{
					_animatedSprite.Play("walk_downright");
					break;
				}
			default: { 
					_animatedSprite.Play("default");
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
		if (_attackHitBox.GetOverlappingBodies().Count > 0 && _isAttacking)
		{
			Godot.Collections.Array<Node2D> overlapList = _attackHitBox.GetOverlappingBodies();
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
		_attackHitBox.GetChild<CollisionShape2D>(0).DebugColor = Colors.Red;
	}
	public void on_area_2d_area_entered(Area2D collision)
	{
		// Vector math to check if the mouse is facing towards 
		//Vector2 mousePOSinWorld = (GetViewport().GetScreenTransform() * this.GetCanvasTransform()).AffineInverse() * cursor.Position;
		Vector2 playerToEnemy = this.Position.DirectionTo(collision.Position);
		Vector2 playerToMouse = this.Position.DirectionTo(this.GetGlobalMousePosition() * this.Transform);
		//GD.Print($"Dot Product Result: {playerToEnemy.Dot(playerToMouse)}");
		//attackHitBox.Rotate(playerToEnemy.Dot(playerToMouse));
		if (playerToEnemy.Dot(playerToMouse) <= 0.0f)
		{
			if (collision.Owner.HasMethod("TakeDamage") && _isAttacking)
			{
				BaseEnemyAI temp = (BaseEnemyAI)collision.Owner;
				temp.TakeDamage(50);
			}
		}
		
	}
	public void on_Death()
	{
		_dead = true;
		_animatedSprite.Visible = false;
	}

	public void ChangeEnemyCollision(bool hit)
	{
		SetCollisionLayerValue(1, hit);
		SetCollisionMaskValue(1, hit);
	}

	public void FlashOnDamge()
	{
		GetNode<AnimationPlayer>("FlashAnimation").Play("Flash");
	}

	
	private void CreateProjectile()
	{
		var projectile = (Projectile)_projectile.Instantiate();
		//projectile.GlobalPosition = this.GlobalPosition;
		projectile.Position = this.Position;
		projectile.Rotation = _marker.Rotation;

		GetParent().AddChild(projectile);

	}
}
