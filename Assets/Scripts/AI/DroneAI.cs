using Godot;
using System;

public partial class DroneAI : BaseEnemyAI
{
    public enum DroneFSM {
        Stop,
        TrackPlayer,
        Attacking
    }

    [Export]
    private float _moveStopRange;
    [Export]
    private float _playerDetectRange;

    private DroneFSM _droneState;
    private bool _isPlayerInRange = false;

    public double shootCooldown = 1;
    public double currentShootCooldown;
    [Export]
    private float _shootOffset;

    [Export]
    private PackedScene _bullet;
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
        DroneMovement(delta);

        currentShootCooldown -= delta;
    }

    public override void _PhysicsProcess(double delta)
    {
		//base._PhysicsProcess(delta);
	}

    public void DroneMovement(double delta) {
        if (_droneState == DroneFSM.Attacking) {
            //new movement logic
            Attack();
        }
        else if (_droneState == DroneFSM.TrackPlayer) {
            Vector2 direction = (_player.Position -Position).Normalized();
            Position += direction * (float)(_speed * delta);
        }
    }

    private void DetermineDrongState()
    {
        DetectPlayer();
        if (IsPlayerTooClose())
        {
            _droneState = DroneFSM.Attacking;
        }
        else if (_isPlayerInRange && _shouldMove)
        {
            _droneState = DroneFSM.TrackPlayer;
        }
        else {
            _droneState = DroneFSM.Stop;
        }
    }

    private bool IsPlayerTooClose() {
        return new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _moveStopRange;
    }

    private void DetectPlayer() {
        if (_player == null) {
            _isPlayerInRange = false;
        }
        _isPlayerInRange = new BetterMath().DistanceBetweenTwoVector(_player.Position, Position) < _playerDetectRange;
    }

    private void Attack() {
        if (currentShootCooldown <= 0) {
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
        }
    }

}
