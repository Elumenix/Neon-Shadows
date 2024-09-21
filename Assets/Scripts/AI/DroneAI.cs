using Godot;
using System;

public partial class DroneAI : BaseEnemyAI
{
	public override void _Ready()
	{
		base._Ready();
        _usePathFinding = false;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
        DroneMovement(delta);
    }

    public void DroneMovement(double delta) {
        if (_isPlayerInRange && _shouldMove) {
            Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * (float)(_speed * delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
		base._PhysicsProcess(delta);
	}
}
