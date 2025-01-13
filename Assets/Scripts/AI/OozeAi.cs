using Godot;


public partial class OozeAi : BaseEnemyAI
{
	[Export] private float _lungeSpeed = 200.0f;
	[Export] private float _lungeRange = 100.0f;
	[Export] private float _lungeDuration = 0.5f;
	private Timer _lungeTimer;
	private Timer _lungeChargeTimer;
	private Timer _lungeCooldownTimer;
	private bool _isLunging = false;
	private bool _isLungeCooldown = false;
	private bool _isCharging = false;
	[Export] private AnimationPlayer _animationPlayer;
	private Timer _ZIndexTimer;
	private bool _isSpawning;
	private AudioStreamPlayer2D slimeAudio;
	private AudioStreamPlayer2D lungeAudio;
	[Export] private AudioStream deathSound;
	private bool deathSoundPlayed = false;

	public override void _Ready()
	{
		base._Ready();

		float randomScale = (float)GD.RandRange(1, 1.8);
		Scale = new Vector2(randomScale, randomScale);
		Modulate = new Color(0, 0, (float)GD.RandRange(0.3, 0.7));

		_lungeCooldownTimer = GetNode<Timer>("LungeCooldownTimer");
		_lungeCooldownTimer.Timeout += EndCooldown;
		_lungeTimer = new Timer();
		AddChild(_lungeTimer);
		_lungeTimer.WaitTime = _lungeDuration;
		_lungeTimer.OneShot = true;
		_lungeTimer.Timeout += EndLunge;

		_ZIndexTimer = GetNode<Timer>("ZIndexTimer");
		_ZIndexTimer.Timeout += ChangeZIndex;

		_oneSecTimer = GetNode<Timer>("OneSecTimer");
		_oneSecTimer.Timeout += OnOneSecTimerFinished;
		_shouldMove = false;
		_isSpawning = true;

		_lungeChargeTimer = GetNode<Timer>("LungeChargeTimer");
		_lungeChargeTimer.Timeout += OnChargeFinished;

		slimeAudio = GetNode<AudioStreamPlayer2D>("%SlimeAudio");
		lungeAudio = GetNode<AudioStreamPlayer2D>("%LungeAudio");

		Spawn();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
		{
			return;
		}


        if (_isCharging)
        {
            PlayWalkAnimation(GlobalPosition.DirectionTo(_player.GlobalPosition));
			return;
        }

        if (IsOnPlatform() && !_isSpawning)
		{
			if (!_isCharging && _player != null && !_isLungeCooldown && GlobalPosition.DistanceTo(_player.GlobalPosition) < _lungeRange)
			{
				StartChargeLunge();
				Velocity = new Vector2(0,0);
			}
			else if (!_isCharging && !_isLunging)
			{
				UpdateNavigationTarget();
				Vector2 nextPathPosition = nextPathPosition = _navigationAgent.GetNextPathPosition();
				if (_navigationAgent.DistanceToTarget() < _detectionRange)
				{
					_shouldMove = true;
				}
				else
				{
					_shouldMove = false;
				}

				if (_navigationAgent.IsNavigationFinished())
				{
					nextPathPosition = GlobalPosition;
				}

				PlayWalkAnimation(GlobalPosition.DirectionTo(nextPathPosition));
				MoveTowardsTarget(nextPathPosition, delta);
			}

			MoveAndSlide();

            if (!IsOnPlatform())
            {
                _animationPlayer.Play("Fall");
                GetNode<Timer>("ZIndexTimer").Start();
            }
        }


	}

	private void OnChargeFinished()
	{
		_isCharging = false;

		StartLunge();
	}

	public void OnOneSecTimerFinished()
	{
		if (_animatedSprite.Animation == "Spawn")
		{
			_shouldMove = true;
			_animatedSprite.Play("Walk_Down");
			_lungeCooldownTimer.Start();
			_isSpawning = false;
		}
		else if (_animatedSprite.Animation.ToString().Substring(0, 5) == "Death") {
			QueueFree();
		} 
	}


	private bool IsOnPlatform()
	{
		return GetTree().GetNodesInGroup("PlatformArea")[0].GetNode<Area2D>("PlatformArea").OverlapsArea(GetNode<Area2D>("EdgeDetect"));
	}

	public void StartChargeLunge() {
		_isCharging = true;

		_usePathFinding = false;
		_shouldMove = false;
		_lungeChargeTimer.Start();
	}
	
	private void StartLunge()
	{
		if (isDead) return;
		lungeAudio.Play();
		_shouldMove = true;
		_isLunging = true;
		_isLungeCooldown = true;
		Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
		Velocity = direction * _lungeSpeed;
		
		_lungeTimer.Start();
		PlayLungeAnimation(direction);
	}

	private void EndLunge()
	{
		_usePathFinding = true;
		Velocity = Vector2.Zero;
		_lungeCooldownTimer.Start();

		_isLunging = false;

		if (!IsOnPlatform())
		{
			_animationPlayer.Play("Fall");
			GetNode<Timer>("ZIndexTimer").Start();
		}
	
	}

	private void ChangeZIndex() {
		ZIndex = -1;
		HandleDeath();
	}


	private void EndCooldown() {
		_isLungeCooldown = false;

	}


	private void PlayLungeAnimation(Vector2 direction) {
		if (isDead)
		{
			return;
		}
		direction = direction.Normalized();
		if (direction.Y < -0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Lunge_UpRight");
		}
		else if (direction.Y < -0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Lunge_UpLeft");
		}
		else if (direction.Y > 0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Lunge_DownRight");
		}
		else if (direction.Y > 0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Lunge_DownLeft");
		}
		else if (direction.Y < -0.5f)
		{
			_animatedSprite.Play("Lunge_Up");
		}
		else if (direction.Y > 0.5f)
		{
			_animatedSprite.Play("Lunge_Down");
		}
		else if (direction.X < -0.5f)
		{
			_animatedSprite.Play("Lunge_Left");
		}
		else if (direction.X > 0.5f)
		{
			_animatedSprite.Play("Lunge_Right");
		}
	}

	private void PlayWalkAnimation(Vector2 direction)
	{
		if (isDead)
		{
			return;
		}
		direction = direction.Normalized();
		if (direction.Y < -0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Walk_UpRight");
		}
		else if (direction.Y < -0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Walk_UpLeft");
		}
		else if (direction.Y > 0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Walk_DownRight");
		}
		else if (direction.Y > 0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Walk_DownLeft");
		}
		else if (direction.Y < -0.5f)
		{
			_animatedSprite.Play("Walk_Up");
		}
		else if (direction.Y > 0.5f)
		{
			_animatedSprite.Play("Walk_Down");
		}
		else if (direction.X < -0.5f)
		{
			_animatedSprite.Play("Walk_Left");
		}
		else if (direction.X > 0.5f)
		{
			_animatedSprite.Play("Walk_Right");
		}
	}

	private void PlayDeathAnimation(Vector2 direction)
	{
		if (isDead)
		{
			return;
		}
		direction = direction.Normalized();
		if (direction.Y < -0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Death_UpRight");
		}
		else if (direction.Y < -0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Death_UpLeft");
		}
		else if (direction.Y > 0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Death_DownRight");
		}
		else if (direction.Y > 0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Death_DownLeft");
		}
		else if (direction.Y < -0.5f)
		{
			_animatedSprite.Play("Death_Up");
		}
		else if (direction.Y > 0.5f)
		{
			_animatedSprite.Play("Death_Down");
		}
		else if (direction.X < -0.5f)
		{
			_animatedSprite.Play("Death_Left");
		}
		else if (direction.X > 0.5f)
		{
			_animatedSprite.Play("Death_Right");
		}
	}

	protected override void HandleDeath()
	{
		if(rng.Randf() <= _heartDropChance)
		{
            CallDeferred("_spawnHealthPickUp");
        }
		else
		{
			spawnedHeart = true;
		}
		Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
		if (!isDead)
		{
			GameManager.Instance.EnemyDefeated();
		}
		PlayDeathAnimation(direction);
		isDead = true;
		_shouldMove = false;
		_isSpawning = true;
		if(_oneSecTimer.TimeLeft==0)
			_oneSecTimer.Start();
	}


	public override void TakeDamage(int damageAmount)
	{
		if (isDead)
			return;

		_shouldMove = false;
		base.TakeDamage(damageAmount);
	}

	public override void PlayDamageSound()
	{
		if (isDead) return;
		lungeAudio.Stop();
		slimeAudio.Play();
	}

	public override void PlayDeathSound()
	{
		if (deathSoundPlayed) return;
		deathSoundPlayed = true;
		lungeAudio.Stop();
		slimeAudio.Stop();

		// Switching the lunge audio to carry the sound of death
		lungeAudio.Stream = deathSound;
		lungeAudio.PitchScale = 1;
		
		lungeAudio.Play();
	}

	protected void Spawn()
	{
		if (_animatedSprite == null)
			_animatedSprite = GetNode<AnimatedSprite2D>("EnemySprite");

		_oneSecTimer.Start();
		_animatedSprite.Play("Spawn");
	}

}
