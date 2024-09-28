using Godot;
using System;

public partial class DroneAI : BaseEnemyAI
{
    public enum DroneFSM {
        TrackPlayer,
        Attacking
    }

    [Export]
    private float _moveStopRange;

    private DroneFSM _droneState;
	public override void _Ready()
	{
		base._Ready();
        n_usePathFinding = false;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
        DroneMovement(delta);
        GD.Print(_droneState.ToString());
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
        if (IsPlayerTooClose())
        {
            _droneState = DroneFSM.Attacking;
        }
        else if (_isPlayerInRange && _shouldMove)
        {
            _droneState = DroneFSM.TrackPlayer;
        }
    }

    private bool IsPlayerTooClose() {
        return new BetterMath().DistanceBetweenTwoVector(_player.GlobalPosition, GlobalPosition) < _moveStopRange;
    }

    private void Attack() {
        Vector2 bulletDirection = _player.GlobalPosition - GlobalPosition;

        float shootAngle = new BetterMath().VectorToAngle(bulletDirection);
        GD.Print(shootAngle);
        //shoot bullet
    }

}
