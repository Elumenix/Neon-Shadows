using Godot;
using System;
using System.Threading.Tasks;
using static Godot.TextServer;

public partial class DroneAI : BaseEnemyAI
{
	public enum DroneFSM {
		Stop,
		TrackPlayer,
		Attacking,
		AttackCharging
	}

	[Export]
	private float _moveStopRange;
	[Export]
	private float _playerDetectRange;
	private float _aggroDetectRange;

	private DroneFSM _droneState;
	private bool _isPlayerInRange = false;
	private bool detectedPlayer = false;

	public double shootCooldown = 1;
	public double currentShootCooldown;

	private bool _movementCompleted = false;
	[Export]
	private float _shootOffset;

	private double _totalDistance;
	private bool _positiveDirection;
	private Vector2 _targetPosition;
	private Vector2 _originalPosition;
	[Export]public Vector2 targetMaxOffset;

	[Export]
	private PackedScene _bullet;
	[Export]
	private CpuParticles2D _particles;

	private AudioStreamPlayer2D droneHit;
	[Export] private AudioStream deathSound;

	public override void _Ready()
	{
		base._Ready();
		_usePathFinding = false;
		currentShootCooldown = shootCooldown;
		_aggroDetectRange = _playerDetectRange * 3;

		droneHit = GetNode<AudioStreamPlayer2D>("%DroneHit");

		_oneSecTimer = GetNode<Timer>("OneSecTimer");
		_oneSecTimer.Timeout += OnOneSecTimerFinished;
		_shouldMove = false;
		Spawn();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		DetermineDroneState();

		if(_shouldMove)
			DroneLogic(delta);

		currentShootCooldown -= delta;
		/*
		if (_droneState == DroneFSM.Attacking) {

			Vector2 bulletDirection = _player.Position - Position;
			UpdateAnimation(bulletDirection);
		}
		*/
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
		{
			return;
		}
		//base._PhysicsProcess(delta);

		//check collision with player short range attack
		var collision = MoveAndCollide(new Vector2(0,0));
		if (collision != null)
		{
			// Enemy collided with a slash
			if (collision.GetCollider() is PlayerSlash)
			{
				// take damage from the slash
				PlayerSlash temp = (PlayerSlash)collision.GetCollider();
				if (_iFrames <= 0)
				{
					this.TakeDamage(temp.DealDamage());
				}
			}
			else if (collision.GetCollider() is Player)
			{
				// Collided with the player
				HandlePlayerCollision();
			}
		}
	}

	public void OnOneSecTimerFinished()
	{
		if (_animatedSprite.Animation == "Spawn")
		{
			_shouldMove = true;
			_animatedSprite.Play("Forward");
            _playerDetectRange = 275;
        }
		else if (_animatedSprite.Animation.ToString().Substring(0, 5) == "Death")
		{
			QueueFree();
		}
	}

	public void DroneLogic(double delta) {
		if (_droneState == DroneFSM.Attacking) {
			//new movement logic
			//Attack();
			//
			//Vector2 direction = (_player.Position - Position).Normalized();
			//Vector2 newDirection = new Vector2(-direction.Y, direction.X);
			//if (_positiveDirection)
			//{
			//    Position += newDirection * (float)(_speed * delta);
			//}
			//else {
			//    Position += newDirection * (float)(-_speed * delta);
			//}
			//DroneMovement(delta);


		}
		else if (_droneState == DroneFSM.TrackPlayer) {
			DroneMovement(delta);

		}
	}

	/// <summary>
	/// update the target position of the enemy pathfinding
	/// </summary>
	private void UpdateNavigationTarget()
	{
		if (GameManager.Instance.player != null)
		{
			_navigationAgent.TargetPosition = _targetPosition;
		}
	}

	/// <summary>
	/// Move the drone towards the target position near the player
	/// </summary>
	/// <param name="delta"></param>
	private void DroneMovement(double delta) {
		//move the drone towards the target position if the current target position is not reached
		if (!_movementCompleted)
		{
			//init the variable needed for movement
			Vector2 nextPosition = _navigationAgent.GetNextPathPosition();
			double currentDistance = new BetterMath().DistanceBetweenTwoVector(_targetPosition, GlobalPosition);
			double process = (_totalDistance - currentDistance) / _totalDistance;
			double speed = _speed * new BetterMath().EasingCalculation(process) + _speed/5;
			Vector2 direction = (nextPosition - GlobalPosition).Normalized();
			UpdateAnimation(direction);
			//mark current movement as complete when it's close enough to the target position
			if (_navigationAgent.IsNavigationFinished())
			{
				_movementCompleted = true;
				_droneState = DroneFSM.AttackCharging;
				Attack();
			}

			//increase the speed to make sure drone will always move
			if(process < 0.1) {
				speed += 10;	
			}
			// move the drone and handle any collisions
			Velocity += direction * (float)speed;
			Velocity = Velocity.LimitLength(_maxSpeed);
			var collision = MoveAndCollide(Velocity * (float)(delta));
			if (collision != null)
			{

				if (collision.GetCollider() is PlayerSlash)
				{
					PlayerSlash temp = (PlayerSlash)collision.GetCollider();
					//this.TakeDamage(temp.DealDamage());
				}
			}
		}
		else
		{
			//create a new target position if the previouse movement is completed
			_targetPosition = new Vector2(_player.GlobalPosition.X + (float)new BetterMath().GetRandomWithNegative(targetMaxOffset.X),
				(float)new BetterMath().GetRandomWithNegative(targetMaxOffset.Y) + _player.GlobalPosition.Y);
			_totalDistance = new BetterMath().DistanceBetweenTwoVector(_targetPosition, GlobalPosition);
			UpdateNavigationTarget();
			_movementCompleted = false;

			// Check if the drone is colliding
			var collision = MoveAndCollide(Vector2.Zero, true);
			if (collision != null)
			{
				if(collision.GetCollider() is PlayerSlash)
				{
					PlayerSlash temp = (PlayerSlash)collision.GetCollider();
					this.TakeDamage(temp.DealDamage());
				}
			}
		}
	}

	/// <summary>
	/// determine which state the drone will currently be
	/// </summary>
	private void DetermineDroneState()
	{
		DetectPlayer();
		if (_droneState != DroneFSM.AttackCharging) {
			if (IsPlayerTooClose())
			{
				_playerDetectRange = _aggroDetectRange;
			}
			else if (_isPlayerInRange && _shouldMove)
			{
				_droneState = DroneFSM.TrackPlayer;
			}
			else
			{
				_droneState = DroneFSM.Stop;
				_movementCompleted = true;
			}
		}
	}

	/// <summary>
	/// return whether or not drone is too close to the player
	/// </summary>
	/// <returns></returns>
	private bool IsPlayerTooClose() {
		if (GameManager.Instance.player == null) {
			return false;
		}
		return new BetterMath().DistanceBetweenTwoVector(GameManager.Instance.player.Position, Position) < _moveStopRange;
	}

	/// <summary>
	/// detect if player is in range of the drone's detection range
	/// </summary>
	private void DetectPlayer() {
		if (GameManager.Instance.player == null) {
			_isPlayerInRange = false;
			return;
		}
		_isPlayerInRange = new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _playerDetectRange;

		if (_isPlayerInRange && !detectedPlayer)
		{
			detectedPlayer = true;
			_playerDetectRange = _aggroDetectRange;
			_detectionRange = _aggroDetectRange;
			GetNode<AudioStreamPlayer2D>("%NoticeSound").Play();
		}
	}

	/// <summary>
	/// Shoot bullet toward a position near the player
	/// </summary>
	private async void Attack() {

		//_droneState = DroneFSM.Attacking;
		//show the charging attack particle effect before shoot the bullet
		PlayAttackAnimation(GlobalPosition.DirectionTo(GameManager.Instance.player.GlobalPosition));

		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

		//calculate angle
		Vector2 bulletDirection = GlobalPosition.DirectionTo(GameManager.Instance.player.GlobalPosition);
		float shootAngle = new BetterMath().VectorToAngle(bulletDirection);
		shootAngle += (float)(new Random().NextDouble() * (_shootOffset*2) - _shootOffset);

		UpdateAnimation(new BetterMath().AngleToVector(shootAngle));

		// Don't let drone fire if it's in it's death animations
		if (isDead) return;
		
		//shoot bullet
		Node node = _bullet.Instantiate();
		(node as Node2D).Position = Position;
		(node as Node2D).Rotation = shootAngle;
		(node as Bullet).player = GameManager.Instance.player;
		GetParent().AddChild(node);
		PlayBulletSound();

		currentShootCooldown = shootCooldown;
		_positiveDirection = !_positiveDirection;

		//stop for a period of time before next state
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		_droneState = DroneFSM.TrackPlayer;

	}

	private void UpdateAnimation(Vector2 direction)
	{
		if (isDead) return;

		direction = direction.Normalized();
		if (direction.Y < -0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Backward-Rightward");
		}
		else if (direction.Y < -0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Backward-Leftward");
		}
		else if (direction.Y > 0.5f && direction.X > 0.5f)
		{
			_animatedSprite.Play("Forward-Rightward");
		}
		else if (direction.Y > 0.5f && direction.X < -0.5f)
		{
			_animatedSprite.Play("Forward-Leftward");
		}
		else if (direction.Y < -0.5f)
		{
			_animatedSprite.Play("Backward");
		}
		else if (direction.Y > 0.5f)
		{
			 _animatedSprite.Play("Forward");
		}
		else if (direction.X < -0.5f)
		{
			_animatedSprite.Play("Leftward");
		}
		else if (direction.X > 0.5f)
		{
			_animatedSprite.Play("Rightward");
		}
	}

	private void PlayDeathAnimation(Vector2 direction)
	{
		if (isDead) return;
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

    private void PlayAttackAnimation(Vector2 direction)
    {
        if (isDead) return;
        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Attack_UpRight");
        }
        else if (direction.Y < -0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Attack_UpLeft");
        }
        else if (direction.Y > 0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Attack_DownRight");
        }
        else if (direction.Y > 0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Attack_DownLeft");
        }
        else if (direction.Y < -0.5f)
        {
            _animatedSprite.Play("Attack_Up");
        }
        else if (direction.Y > 0.5f)
        {
            _animatedSprite.Play("Attack_Down");
        }
        else if (direction.X < -0.5f)
        {
            _animatedSprite.Play("Attack_Left");
        }
        else if (direction.X > 0.5f)
        {
            _animatedSprite.Play("Attack_Right");
        }
    }

    protected void Spawn()
	{
		if (_animatedSprite == null)
			_animatedSprite = GetNode<AnimatedSprite2D>("EnemySprite");

		_oneSecTimer.Start();
		_animatedSprite.Play("Spawn");
	}

	protected override void HandleDeath()
	{
		Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
		if (!isDead) {
			GameManager.Instance.EnemyDefeated();
		}

		PlayDeathAnimation(direction);
		isDead = true;
		_shouldMove = false;

		if (_oneSecTimer.TimeLeft == 0)
			_oneSecTimer.Start();
	}

	public override void TakeDamage(int damageAmount) {
		if (isDead)
			return;
		base.TakeDamage(damageAmount);
		_playerDetectRange = _aggroDetectRange;
	}

	public override void PlayDamageSound()
	{
		droneHit.Play();
	}

	public override void PlayDeathSound()
	{
		// Quickly switching any playing tracks and switching to play death sound
		droneHit.Stop();
		droneHit.Stream = deathSound;
		droneHit.Play();
		
		base.PlayDeathSound();
	}
	
	public override void OnBodyEntered(Node2D body) {
		return;
	}
	
	private void PlayBulletSound()
	{
		var audioPlayer = new AudioStreamPlayer2D
		{
			Stream = GameManager.Instance.bulletSoundEffect,
			Position = GlobalPosition,
			Autoplay = true,
			PitchScale = (float)GD.RandRange(1.0, 3.0)
		};
		
		// TODO: AudioPlayer doesn't seem to free itself correctly
		audioPlayer.Connect("finished", new Callable(audioPlayer, nameof(audioPlayer.QueueFree)));
		GetTree().Root.AddChild(audioPlayer);
	}
}
