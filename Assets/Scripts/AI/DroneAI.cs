using Godot;
using System;
using System.Threading.Tasks;

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

	private DroneFSM _droneState;
	private bool _isPlayerInRange = false;

	public double shootCooldown = 1;
	public double currentShootCooldown;

	private bool _movementCompleted = false;
	[Export]
	private float _shootOffset;

	private bool _positiveDirection;
	private Vector2 _targetPosition;
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

	private void DroneMovement(double delta) {
        if (!_movementCompleted)
        {
            Vector2 direction = (_targetPosition - Position).Normalized();
            //Position += direction * (float)(_speed * delta);
			var collision = MoveAndCollide(direction * (float)(_speed * delta));
			if (collision != null)
			{
				if(collision.GetCollider() is PlayerSlash)
				{
					PlayerSlash temp = (PlayerSlash)collision.GetCollider();
					this.TakeDamage(temp.Damage);
				}
			}

            if (new BetterMath().DistanceBetweenTwoVector(_targetPosition,Position) < 1)
            {
                _movementCompleted = true;
				_droneState = DroneFSM.AttackCharging;
                Attack();
            }
        }
        else
        {
            _targetPosition = new Vector2(_player.Position.X + (float)new BetterMath().GetRandomWithNegative(targetMaxOffset.X),
                (float)new BetterMath().GetRandomWithNegative(targetMaxOffset.Y) + _player.Position.Y);
            _movementCompleted = false;
        }
    }

	private void DetermineDrongState()
	{
		DetectPlayer();
		if (_droneState != DroneFSM.AttackCharging) {
            if (IsPlayerTooClose())
            {
                _droneState = DroneFSM.Attacking;
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

	private bool IsPlayerTooClose() {
		if (_player == null) {
			return false;
		}
		return new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _moveStopRange;
	}

	private void DetectPlayer() {
		if (_player == null) {
			_isPlayerInRange = false;
			return;
		}
		_isPlayerInRange = new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _playerDetectRange;
	}

	private async void Attack() {
		_particles.Show();
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        _particles.Hide();
        //if (currentShootCooldown <= 0) {
        //calculate angle
        Vector2 bulletDirection = _player.Position - Position;

			float shootAngle = new BetterMath().VectorToAngle(bulletDirection);

			shootAngle += (float)(new Random().NextDouble() * (_shootOffset*2) - _shootOffset);
			//shootAngle = Mathf.RadToDeg(shootAngle);

			//shoot bullet

			Node node = _bullet.Instantiate();
			(node as Node2D).Position = Position;
			(node as Node2D).Rotation = shootAngle;
			(node as Bullet).player = _player;
			GetParent().AddChild(node);

			currentShootCooldown = shootCooldown;
			_positiveDirection = !_positiveDirection;

        //}

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        _droneState = DroneFSM.Stop;

	}

}
