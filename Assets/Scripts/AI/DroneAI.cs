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

    [Export]
    private PackedScene _bullet;
    public override void _Ready()
	{
		base._Ready();
        _usePathFinding = false;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
        DetermineDrongState();
        DroneMovement(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
		base._PhysicsProcess(delta);
	}

    public void DroneMovement(double delta) {
        if (_droneState == DroneFSM.Attacking) {
            //new movement logic
            Attack();
        }
        else if (_droneState == DroneFSM.TrackPlayer) {
            Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * (float)(_speed * delta);
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
        return new BetterMath().DistanceBetweenTwoVector(_player.GlobalPosition, GlobalPosition) < _moveStopRange;
    }

    private void DetectPlayer() {
        _isPlayerInRange = new BetterMath().DistanceBetweenTwoVector(_player.GlobalPosition, GlobalPosition) < _playerDetectRange;
    }

    private void Attack() {
        //calculate angle
        Vector2 bulletDirection = _player.GlobalPosition - GlobalPosition;

        float shootAngle = new BetterMath().VectorToAngle(bulletDirection);
        //shootAngle = Mathf.RadToDeg(shootAngle);

        GD.Print(shootAngle);
        //shoot bullet

        Node node = _bullet.Instantiate();
        (node as Node2D).GlobalPosition = GlobalPosition;
        (node as Node2D).Rotation = shootAngle;
        GetParent().AddChild(node);
    }

}
