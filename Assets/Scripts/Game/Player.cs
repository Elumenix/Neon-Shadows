using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

public enum FACING_DIRECTION { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }

public partial class Player : CharacterBody2D
{
	// Fields
	private Vector2 _heading;
	private float _maxSpeed = 160.0f;
	private float _speed = 80.0f;
	private float _friction = 700.0f;

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
	private bool _lastMoved; // if you moved in the previous frame
	private int _attackCount; // Where the player is in the combo attacks. 0 means no attacks yet.
	private Area2D _attackHitBox;
	private bool _isAttacking;
	private Timer _attackTimer;
	private PackedScene _slashScene = GD.Load<PackedScene>("res://Assets/Entities/Objects/Slash.tscn");

	// Dash Stuff
	private Dash _dash;
	private float _dashSpeed = 550.0f;
	private const float _DashDuration = 0.15f;

	// Ranged Stuff
	private int _ammo;
	public int Ammo { get { return _ammo; } }
	private const int _MaxAmmo = 5;
	private PackedScene _projectile = GD.Load<PackedScene>("res://Assets/Entities/Objects/PlayerProjectile.tscn");
	private Marker2D _marker;
	private Timer _rangedTimer;
	private bool _isShooting;

	//falling off edges
	private bool _isFalling = false;
	private Vector2 _safePosition; 
	private Timer _safePositionTimer;
	private Vector2 _direction;
	private bool _fallCooldown;
	private AnimationPlayer _animationPlayer;
	[Export] private float _fallSpeed;
	private Timer _zIndexTimer;
	// Coyote time stuff
	private Timer _coyoteTimer;
	private float _coyoteWait = 0.2f; // Wait time added to Coyote Timer
	private bool _coyoteEnd;

	// Funky movement thing
	private bool _moveNSlide = false;

	//Foot step effect
	[Export] public PackedScene footstepParticle;
	[Export] public float stepDistance = 50.0f;
	private Vector2 _lastStepPosition;
	private Vector2 _footStepEffectSpawnPosition;
	private Vector2 _playerSpriteSize;

	[Export] PackedScene test;

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
		_lastMoved = false;
		_attackCount = 0;
		_attackTimer = GetNode<Timer>("%Timer");
		_isAttacking = false;
		_attackTimer.OneShot = true;
		

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_dash = GetNode<Dash>("Dash");
		_marker = GetNode<Marker2D>("Marker2D");

		// Ranged Stuff
		_ammo = _MaxAmmo;
		_isShooting = false;
		_rangedTimer = GetNode<Timer>("ShootTimer");


		//init the variables needed for falling off edges mechanic

		// Connect the frame_changed signal to track animation progress
		_safePositionTimer = GetNode<Timer>("SafePositionTimer");
		_safePositionTimer.Timeout += UpdateSafePosition;
		_zIndexTimer = GetNode<Timer>("ZIndexTimer");
		_zIndexTimer.Timeout += ChangeZIndex;
		_safePosition = Position;
		_direction = new Vector2(0,-1);
		_isFalling = true;
		RespawnPlayer();
		UpdateSafePosition();
		ZIndex = 0;
		_fallCooldown = false;
		_animationPlayer.AnimationFinished += ResetAnimation;
		_coyoteTimer = GetNode<Timer>("CoyoteTimer");
		_coyoteEnd = true;

		//foot step
		_lastStepPosition = GlobalPosition;
		_playerSpriteSize = _animatedSprite.SpriteFrames.GetFrameTexture("default", 0).GetSize();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GameManager.Instance.gamePaused)
			return;

		// Do stuff as long as the player isn't dead and is not falling
		if (!_isFalling && !_dead)
		{
			if (Input.IsActionJustPressed("dash") && _dash.CanDash && !_dash.IsDashing)
			{
				// Starts the dash if the player has pressed the dash button, is able to dash, and isn't currently dashing
				_dash.StartDash(_facing, _DashDuration);
			}
			if(Input.IsActionJustReleased("dash") && _dash.IsDashing)
			{
				// ends the dash early if the spacebar is released
				_dash.EndDash();
			}
			GetInput((float)delta);
			// We don't currently use the returned KinematicCollision since the enemy will take care of dealing damage to the player
			//var collision = MoveAndCollide(Velocity * (float)delta);
			MoveAndCollide(Velocity * (float)delta);
			if (!_isAttacking)
			{
				walkAnimation();
			}

		}

		// Fall stuff
		if (!IsOnSafePlatform())
		{
			if (!_coyoteEnd && _coyoteTimer.WaitTime == 0)
			{
				_coyoteTimer.Start(_coyoteWait);
			}

			if (!_isFalling && _coyoteEnd)
			{
				TriggerFall();
			}
		}

		//spawn foot step effect when player traveraled long enough
		if (GlobalPosition.DistanceTo(_lastStepPosition) >= stepDistance)
		{
			SpawnFootstep();
			_lastStepPosition = GlobalPosition;
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Temp code for toggling two movement types
		if (Input.IsActionJustPressed("moveToggle"))
		{
			_moveNSlide = !_moveNSlide;
		}

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
		if (_isFalling)
		{

			if (_direction.Y < 0)
			{
				GlobalPosition += new Vector2(1, 1).Normalized() * _fallSpeed * (float)delta;
			}
			else
			{
				GlobalPosition += (new Vector2(1, 1).Normalized() + _direction).Normalized() * _fallSpeed * (float)delta;
			}
		}

	}
	/// <summary>
	/// Checks for player relevant input actions and handles them. (Called once per Physics Frame)
	/// </summary>
	public void GetInput(float delta)
	{
		if (_isFalling) {
			Velocity = new Vector2(0,0);
			return;
		}

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

		if (Input.IsActionJustPressed("heal")) {
			takeDamage(-1);
		}

		if (_dash.IsDashing)
		{
			if (_heading.IsZeroApprox())
			{
				_heading = _directionFromFacing();
			}
			Velocity = _heading * _dashSpeed;
			return;
		}


		// Get Vector returns a vector based off the inputs, with a length of 1 (normalized)
		//_heading.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		//_heading.Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
		_heading = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		// Use isometrics on diagonals
		if(_heading.X > 0 && _heading.Y > 0)
		{
			_heading = _cartesianToIsometric(_heading, true);
			_heading.X = _heading.X + 0.5f * 64.0f;
			_heading.Y = _heading.Y + 0.5f * 32.0f;
		}
		else if(_heading.X < 0 && _heading.Y < 0)
		{
			_heading = _cartesianToIsometric(_heading, true);
			_heading.X = _heading.X + -0.5f * 64.0f;
			_heading.Y = _heading.Y + -0.5f * 32.0f;
		}
		else if (_heading.X > 0 && _heading.Y < 0)
		{
			_heading = _cartesianToIsometric(_heading, false);
			_heading.X = _heading.X + 0.5f * 64.0f;
			_heading.Y = _heading.Y + -0.5f * 32.0f;
		}
		else if (_heading.X < 0 && _heading.Y > 0)
		{
			_heading = _cartesianToIsometric(_heading, false);
			_heading.X = _heading.X + -0.5f * 64.0f;
			_heading.Y = _heading.Y + 0.5f * 32.0f;
		}
		_heading = _heading.Normalized();

		// Set last move to what it was in the previous frame
		_lastMoved = _hasMoved;

		// Update _hasMoved
		if(_heading.IsZeroApprox())
		{
			_hasMoved = false;
			//GD.Print("Hasn't moved");
		}
		else
		{
			_hasMoved = true;
		}

		if (_heading.IsEqualApprox(Vector2.Zero) && !_dash.IsDashing)
		{
			if (Velocity.Length() > (_friction * delta))
			{
				Velocity -= Velocity.Normalized() * (_friction * delta);
			}
			else
			{
				Velocity = Vector2.Zero;
			}
		}

		
		Velocity += _heading * _speed;
		Velocity = Velocity.LimitLength(_maxSpeed);
		

	}
	/// <summary>
	/// Updates the FACING_DIRECTION Enum which is used for animation stuff
	/// </summary>
	private void updateFacing()
	{
		if (_heading.X > 0 && _heading.Y == 0)
		{
			_facing = FACING_DIRECTION.Right;
			_direction = new Vector2(1, 0);
			_footStepEffectSpawnPosition = new Vector2(Position.X - _playerSpriteSize.X / 2, Position.Y + _playerSpriteSize.Y / 2);
		}
		else if (_heading.X < 0 && _heading.Y == 0)
		{
			_facing = FACING_DIRECTION.Left;
			_direction = new Vector2(-1, 0);
			_footStepEffectSpawnPosition = new Vector2(Position.X + _playerSpriteSize.X / 2,Position.Y + _playerSpriteSize.Y/2);
		}
		else if (_heading.X == 0 && _heading.Y < 0)
		{
			_facing = FACING_DIRECTION.Up;
			_direction = new Vector2(0, -1);
			_footStepEffectSpawnPosition = new Vector2(Position.X, Position.Y + _playerSpriteSize.Y / 2);
		}
		else if (_heading.X == 0 && _heading.Y > 0)
		{
			_facing = FACING_DIRECTION.Down;
			_direction = new Vector2(0, 1);
			_footStepEffectSpawnPosition = new Vector2(Position.X, Position.Y - _playerSpriteSize.Y / 2);
		}
		else if (_heading.X > 0 && _heading.Y < 0)
		{
			_facing = FACING_DIRECTION.UpRight;
			_direction = new Vector2(1, -1).Normalized();
			_footStepEffectSpawnPosition = new Vector2(Position.X, Position.Y + _playerSpriteSize.Y / 2);
		}
		else if (_heading.X > 0 && _heading.Y > 0)
		{
			_facing = FACING_DIRECTION.DownRight;
			_direction = new Vector2(1, 1).Normalized();
			_footStepEffectSpawnPosition = new Vector2(Position.X, Position.Y + _playerSpriteSize.Y / 2);
		}
		else if (_heading.X < 0 && _heading.Y < 0)
		{
			_facing = FACING_DIRECTION.UpLeft;
			_direction = new Vector2(-1, -1).Normalized();
			_footStepEffectSpawnPosition = new Vector2(Position.X, Position.Y + _playerSpriteSize.Y / 2);
		}
		else if (_heading.X < 0 && _heading.Y > 0)
		{
			_facing = FACING_DIRECTION.DownLeft;
			_direction = new Vector2(-1, 1).Normalized();
			_footStepEffectSpawnPosition = new Vector2(Position.X, Position.Y + _playerSpriteSize.Y / 2);
		}
	}
	
	/// <summary>
	/// Returns a normalized vector based on the direction the player is facing
	/// </summary>
	/// <returns>normalized vector in the direction the player is facing</returns>
	private Vector2 _directionFromFacing()
	{
		Vector2 direction = Vector2.Zero;
		switch(_facing)
		{
			case (FACING_DIRECTION.Up):
			{
				direction = Vector2.Up;
				break;
			}
			case (FACING_DIRECTION.UpRight):
				{
					direction =new Vector2(1,-1).Normalized();
					break;
				}
			case(FACING_DIRECTION.UpLeft):
				{
					direction = new Vector2(-1,-1).Normalized();
					break;
				}
			case (FACING_DIRECTION.Down):
				{
					direction = Vector2.Down;
					break;
				}
			case (FACING_DIRECTION.DownRight):
				{
					direction = new Vector2(1,1).Normalized();
					break;
				}
			case (FACING_DIRECTION.DownLeft):
				{
					direction = new Vector2(-1,1).Normalized();
					break;
				}
			case(FACING_DIRECTION.Right):
				{
					direction = Vector2.Right;
					break;
				}
			case (FACING_DIRECTION.Left):
				{
					direction = Vector2.Left;
					break;
				}
		}
		return direction;
	}

	/// <summary>
	/// Takes damage if the player doesn't have any invulenrable frames, if the player takes damage the gain i frames, and has the sprite flash
	/// </summary>
	/// <param name="damage">The amount of damage the player will take (usually only 1 damage)</param>
	public void takeDamage(int damage)
	{
		if (damage < 0)
		{
			_health -= damage;
			HUDManager.Instance.IncreasePlayerHp();
			return;
		}

		if (_isFalling) {
			return;
		}

		if(_damageFrames > 0.0f)
		{
			return;
		}
		FlashOnDamge();

		if (_health > 0)
		{
			_health -= damage;
			HUDManager.Instance.DecreasePlayerHp();
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
		if (!_hasMoved)// && !_lastMoved)
		{
			_animatedSprite.Stop();
			_animatedSprite.Frame = 1;
		}
	}
	/// <summary>
	/// Changes the attack animation after updateFacing
	/// </summary>
	private void _attackAnimation()
	{
		updateFacing();
		switch (_facing)
		{
			case (FACING_DIRECTION.Up):
				{
					_animatedSprite.Play("attack_up");
					break;
				}
			case (FACING_DIRECTION.Left):
				{
					_animatedSprite.Play("attack_left");
					break;
				}
			case (FACING_DIRECTION.Right):
				{
					_animatedSprite.Play("attack_right");
					break;
				}
			case (FACING_DIRECTION.Down):
				{
					_animatedSprite.Play("attack_down");
					break;
				}
			case (FACING_DIRECTION.UpLeft):
				{
					_animatedSprite.Play("attack_upleft");
					break;
				}
			case (FACING_DIRECTION.DownLeft):
				{
					_animatedSprite.Play("attack_downleft");
					break;
				}
			case (FACING_DIRECTION.UpRight):
				{
					_animatedSprite.Play("attack_upright");
					break;
				}
			case (FACING_DIRECTION.DownRight):
				{
					_animatedSprite.Play("attack_downright");
					break;
				}
			default:
				{
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

		_attackAnimation();

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
		slash.Player = this;
		if(_attackCount == 0) { slash.Modulate = Colors.White; }
		else if (_attackCount == 1) { slash.Modulate = Colors.Blue; slash.GetChild<Sprite2D>(0).FlipV = true; }
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
		_animationPlayer.Play("Flash");
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

	/// <summary>
	/// call this function when the player leaves the platform
	/// </summary>
	private void TriggerFall()
	{

		if (!_isFalling && _fallCooldown && !_dash.IsDashing)
		{
			_isFalling = true;
			Velocity = Vector2.Zero;

			//play the fall animation
			_animatedSprite.Play("fall");
			_animationPlayer.Play("Fall");

			_fallCooldown = false;
			//_coyoteEnd = false;

			if (_direction.Y < 0)
			{
				_zIndexTimer.Start();
			}

			if (GlobalPosition.Y < GetViewport().GetCamera2D().GetScreenCenterPosition().Y && _direction.X < 0)
			{
				_zIndexTimer.Start();
			}
		}
	}

	/// <summary>
	/// store the player's position as safe if they are on a platform
	/// </summary>
	public void UpdateSafePosition()
	{
		if (IsOnSafePlatform()) {
			_safePosition = Position;
			_fallCooldown = true;
		}
	}

	private void ChangeZIndex() {
		ZIndex = -1;
	}

	/// <summary>
	/// Respawn player at the last safe position
	/// </summary>
	private void RespawnPlayer()
	{
		_zIndexTimer.Stop();
		_isFalling = false;
		ZIndex = 0;
		Position = _safePosition;
		_animationPlayer.Stop();
		_animatedSprite.Scale = new Vector2(1,1);
		_animatedSprite.Play("default");
		GetViewport().GetCamera2D().GlobalPosition = GlobalPosition;
		//takeDamage(1);
		//HUDManager.Instance.DecreasePlayerHp();
	}

	/// <summary>
	/// Check if the player is on a valid platform
	/// </summary>
	/// <returns></returns>
	private bool IsOnSafePlatform()
	{
		return GetTree().GetNodesInGroup("PlatformArea")[0].GetNode<Area2D>("PlatformArea").OverlapsArea(GetNode<Area2D>("EdgeDetect"));
	}

	/// <summary>
	/// detect if the fall animation is done
	/// </summary>
	private void ResetAnimation(StringName animName)
	{
		if(animName == "Fall")
			RespawnPlayer();
		if (animName == "Flash") {
			_animationPlayer.Stop();
		}
	}


	/// <summary>
	/// Converts coordinates from a cartesian system to the isometric system
	/// </summary>
	/// <param name="cartesian">The Vector in the cartesian system being modified</param>
	/// <param name="same"> Whether the heading components are in the same direction of 0 (both greater than or both lesser than)</param>
	/// <returns>The new vector in the isometric system</returns>
	private Vector2 _cartesianToIsometric(Vector2 cartesian, bool same = true)
	{
		Vector2 isometric = new Vector2();
		isometric.X = cartesian.X - cartesian.Y;
		isometric.Y = (cartesian.X + cartesian.Y) / 2.0f;
		isometric = isometric.Rotated(Mathf.Pi * 5.0f / 3.0f);
		if (!same)
		{
			float temp = isometric.X * -1.0f;
			isometric.X = isometric.Y * -1.0f;
			isometric.Y = temp;
		}
		return isometric;
	}
	/// <summary>
	/// Called when the coyote Timer goes off
	/// </summary>
	private void _endCoyoteTime()
	{
		_coyoteEnd = true;
	}

	/// <summary>
	/// spawn foot step effect
	/// </summary>
	private void SpawnFootstep()
	{
		//multiply by 0.8 to make the color a little darker
		Color pixelColor = GetScreenColorUnderPlayer() *0.8f;

		//spawn the effect
		var particleInstance = (CpuParticles2D)footstepParticle.Instantiate();
		particleInstance.ZIndex = 1;
		particleInstance.GlobalPosition = _footStepEffectSpawnPosition;



		particleInstance.Color = pixelColor;
		particleInstance.Emitting = true;
		particleInstance.Gravity = -_direction * 100;
		GetTree().CurrentScene.AddChild(particleInstance);

		//delete effect after it's done
		particleInstance.OneShot = true;
		GetTree().CreateTimer((float)particleInstance.Lifetime).Timeout += () =>
		{
			particleInstance.QueueFree();
		};
	}



	private Color GetScreenColorUnderPlayer()
	{
		Viewport viewport = GetViewport();
		Image image = viewport.GetTexture().GetImage();

		Camera2D camera = viewport.GetCamera2D();

		Vector2 screenPos = GlobalPosition - (camera.GlobalPosition - (viewport.GetVisibleRect().Size / 2));

		int x = Mathf.Clamp((int)screenPos.X, 0, image.GetWidth() - 1);
		int y = Mathf.Clamp((int)screenPos.Y, 0, image.GetHeight() - 1);

		return image.GetPixel(x, y);
	}

}
