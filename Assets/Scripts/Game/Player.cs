using Godot;
using System;
using System.IO;
using System.Runtime.CompilerServices;

enum FACING_DIRECTION { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }

public partial class Player : CharacterBody2D
{
	// Fields
	private Vector2 _heading;
	private float _maxSpeed = 50.0f;
	private float _speed = 130.0f;
	private AnimatedSprite2D _animatedSprite;
	private FACING_DIRECTION _facing;

	// Health related fields
	private int _health;
	private bool _dead;
	private float _damageFrames;
	public bool IsInvulenerable { get { return _damageFrames > 0; } }
	public bool IsDead { get { return _dead; } }

	// attack realted fields
	private bool _hasMoved;
	private int _attackCount; // Where the player is in the combo attacks. 0 means no attacks yet.
	private Area2D _attackHitBox;
	private bool _isAttacking;
	private Timer _attackTimer;
	private PackedScene _slashScene = GD.Load<PackedScene>("res://Assets/Entities/Objects/Slash.tscn");

	// Dash Stuff
	private Dash _dash;
	private float _dashSpeed = 550.0f;
	private const float _DashDuration = 0.2f;

	// Ranged Stuff
	private int _ammo;
	private const int _MaxAmmo = 5;
	private PackedScene _projectile = GD.Load<PackedScene>("res://Assets/Entities/Objects/PlayerProjectile.tscn");
	private Marker2D _marker;
	private Timer _rangedTimer;
	private bool _isShooting;


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
		_attackTimer = GetNode<Timer>("%Timer");
		_isAttacking = false;
		_attackTimer.OneShot = true;
		

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		_dash = GetNode<Dash>("Dash");
		_marker = GetNode<Marker2D>("Marker2D");

		// Ranged Stuff
		_ammo = _MaxAmmo;
		_isShooting = false;
		_rangedTimer = GetNode<Timer>("ShootTimer");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Do stuff as long as the player isn't dead
		if (!_dead)
		{
			if(Input.IsActionJustPressed("dash") && _dash.CanDash && !_dash.IsDashing)
			{
				// Starts the dash if the player has pressed the dash button, is able to dash, and isn't currently dashing
				_dash.StartDash(_heading, _DashDuration);
			}
			GetInput();
			// We don't currently use the returned KinematicCollision since the enemy will take care of dealing damage to the player
			MoveAndCollide(Velocity * (float)delta);
			walkAnimation();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Used mostly to check for timer like objects.
		// Most player logic should be handled by _PhysicsProcess
		
		if(_damageFrames > 0.0f)
		{
            // reduce damage frames
            _damageFrames -= (float)delta;
			if(_damageFrames < 0.0f) { _damageFrames = 0.0f; }
		}
		// Attack timer stuff
		if (_attackTimer.TimeLeft == 0)
		{
			// Stop attacking
			_isAttacking = false;
			_attackCount = 0;
		}
		if(_rangedTimer.TimeLeft == 0)
		{
			// Stop shooting
			_isShooting = false;
		}
	}
	/// <summary>
	/// Checks for player relevant input actions and handles them. (Called once per Physics Frame)
	/// </summary>
	public void GetInput()
	{
		// Get Vector returns a vector based off the inputs, with a length of 1 (normalized)
		//_heading = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		// Isometric movement (I think)
		_heading.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		_heading.Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
		_heading = _heading.Normalized();

		if(_heading == Vector2.Zero)
		{
			_hasMoved = false;
			GD.Print("Hasn't moved");
		}
		else
		{
			_hasMoved = true;
		}
		if (_dash.IsDashing) { Velocity = _heading * _dashSpeed; }
		else { Velocity = _heading * _speed; }

		// Attacking Stuff
		// Check if we're attacking with melee
		if (Input.IsActionJustPressed("attack_melee") && (!_isAttacking || (_attackCount > 0 && _attackCount < 3)))
        {
			_meleeAttack();
        }

		// Check if we are attacking with ranged
        if (Input.IsActionJustPressed("attack_ranged") && !_isShooting)
        {
            Vector2 mousePOSinPlayer = this.GetGlobalMousePosition();
            // Create a Ranged Attack
            _marker.LookAt(mousePOSinPlayer);
            if (_ammo > 0)
            {
                _rangedTimer.WaitTime = 0.25f;
                _isShooting = false;
                CreateProjectile();
            }

        }

    }
	/// <summary>
	/// Updates the FACING_DIRECTION Enum which is used for animation stuff
	/// </summary>
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
	/// <summary>
	/// Takes damage if the player doesn't have any invulenrable frames, if the player takes damage the gain i frames, and has the sprite flash
	/// </summary>
	/// <param name="damage">The amount of damage the player will take (usually only 1 damage)</param>
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
			_damageFrames = 1.0f;
			if (_health <= 0)
			{
				_health = 0;
				on_Death();
			}
		}

	}
	/// <summary>
	/// Changes the walk animations of the player after calling updateFacing
	/// </summary>
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
	/// <summary>
	/// Update attack variables and instantiate a Slash object check PlayerSlash for more info
	/// </summary>
	private void _meleeAttack()
	{
        _isAttacking = true;

        // Rotate Marker to follow the mouse
        Vector2 mousePOSinPlayer = this.GetGlobalMousePosition();
        _marker.LookAt(mousePOSinPlayer);
		// Start attackTimer
        _attackTimer.Start(0.25f);

		// Instantiate a player slash
        PlayerSlash slash = (PlayerSlash)_slashScene.Instantiate();
        slash.Position = this.Position * this.Transform;
        slash.Rotation = _marker.Rotation;
        slash.AttackTime = 0.25f;
        slash.Damage = 50;
		if(_attackCount == 0) { slash.Modulate = Colors.White; }
		else if (_attackCount == 1) { slash.Modulate = Colors.Blue; }
		else if (_attackCount == 2) { 
			// End of combo should deal more knockback then normal
			slash.Modulate = Colors.Red;
		}
        AddChild(slash);

		// increase attack combo
		if(_attackCount < 3) { _attackCount++;}
    }
	/* Old way we handled attacks, kept for code references
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

				if(_ammo < _MaxAmmo)
				{
					_ammo++;
				}
			}
		}
		
	}*/
	/// <summary>
	/// Resolves the player death, currently (10/08) only changes the dead bool and hides the Player Sprite
	/// </summary>
	public void on_Death()
	{
		_dead = true;
		_animatedSprite.Visible = false;
	}

	/// <summary>
	/// Enables/Disables the Player's Collision with enemies, used exclusively for dash as of (10/08)
	/// </summary>
	/// <param name="hit">Whether the player collides with enemies or not, TRUE = can get hit by enemy FALSE = can't get hit by enemy</param>
	public void ChangeEnemyCollision(bool hit)
	{
		SetCollisionLayerValue(1, hit);
		SetCollisionMaskValue(1, hit);
	}
	/// <summary>
	/// Plays a flash animation on the player's sprite. Only called when the player takes damage
	/// </summary>
	public void FlashOnDamge()
	{
		GetNode<AnimationPlayer>("FlashAnimation").Play("Flash");
	}

	/// <summary>
	/// Instantiates a projectile object in the world, and decreases the player's ammo, check Projectile script for more information
	/// </summary>
	private void CreateProjectile()
	{
		var projectile = (Projectile)_projectile.Instantiate();
		//projectile.GlobalPosition = this.GlobalPosition;
		//projectile.Position = this.Position;
		projectile.Position = new Vector2(this.Position.X + 0.5f, this.Position.Y + 0.5f);
		//Vector2 futurePosition = this.Position;
		//Vector2 rotation = new Vector2((float)Math.Sin(_marker.RotationDegrees), (float)Math.Cos(_marker.RotationDegrees));
		//projectile.Position = futurePosition + (rotation * 2.0f);
		
		projectile.Rotation = _marker.Rotation;

		GetParent().AddChild(projectile);
		_ammo--;
	}
	/// <summary>
	/// "Reloads" the player's ammo
	/// Increases the player's ammo if they aren't at max. Only called by the PlayerSlash (as of 10/08)
	/// </summary>
	public void Reload()
	{
		if(_ammo < _MaxAmmo)
		{
			_ammo++;
		}
	}
}
