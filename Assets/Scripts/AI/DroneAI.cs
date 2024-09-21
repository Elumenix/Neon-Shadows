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

        
    }

    


    public override void _PhysicsProcess(double delta)
    {
		base._PhysicsProcess(delta);
	}
}
