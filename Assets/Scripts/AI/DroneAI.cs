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

	public override void _Ready()
	{
		base._Ready();
		_usePathFinding = false;
		currentShootCooldown = shootCooldown;
		_aggroDetectRange = _playerDetectRange * 3;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		DetermineDrongState();
		DroneLogic(delta);

		currentShootCooldown -= delta;
	}

	public override void _PhysicsProcess(double delta)
	{
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
			DroneMovement(delta);


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
        if (_player != null)
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
			var collision = MoveAndCollide(direction * (float)(_speed * delta));
			if (collision != null)
			{

				if (collision.GetCollider() is PlayerSlash)
				{
					PlayerSlash temp = (PlayerSlash)collision.GetCollider();
					this.TakeDamage(temp.DealDamage());
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
	private void DetermineDrongState()
	{
		DetectPlayer();
		if (_droneState != DroneFSM.AttackCharging) {
			if (IsPlayerTooClose())
			{
				_droneState = DroneFSM.Attacking;
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
		if (_player == null) {
			return false;
		}
		return new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _moveStopRange;
	}

	/// <summary>
	/// detect if player is in range of the drone's detection range
	/// </summary>
	private void DetectPlayer() {
		if (_player == null) {
			_isPlayerInRange = false;
			return;
		}
		_isPlayerInRange = new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _playerDetectRange;
	}

	/// <summary>
	/// Shoot bullet toward a position near the player
	/// </summary>
	private async void Attack() {
		//show the charging attack particle effect before shoot the bullet
		_particles.Show();
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		_particles.Hide();

		//calculate angle
		Vector2 bulletDirection = _player.Position - Position;
		float shootAngle = new BetterMath().VectorToAngle(bulletDirection);
		shootAngle += (float)(new Random().NextDouble() * (_shootOffset*2) - _shootOffset);

		//shoot bullet
		Node node = _bullet.Instantiate();
		(node as Node2D).Position = Position;
		(node as Node2D).Rotation = shootAngle;
		(node as Bullet).player = _player;
		GetParent().AddChild(node);

		currentShootCooldown = shootCooldown;
		_positiveDirection = !_positiveDirection;

		//stop for a period of time before next state
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		_droneState = DroneFSM.Stop;

	}
	public override void TakeDamage(int damageAmount) { 
		base.TakeDamage(damageAmount);
		_playerDetectRange = _aggroDetectRange;
	}
}
