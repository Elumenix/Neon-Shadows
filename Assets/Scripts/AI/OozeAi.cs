using Godot;
using System;

public partial class OozeAi : BaseEnemyAI
{
    [Export] private float lungeSpeed = 200.0f;
    [Export] private float lungeRange = 100.0f;
    [Export] private float lungeDuration = 0.5f;
    private Timer lungeTimer;

    public override void _Ready()
    {
        base._Ready();

        float randomScale = (float)GD.RandRange(0.9, 1.1);
        Scale = new Vector2(randomScale, randomScale);
        Modulate = new Color(0, 0, (float)GD.RandRange(0.3, 0.7));

        lungeTimer = new Timer();
        AddChild(lungeTimer);
        lungeTimer.WaitTime = lungeDuration;
        lungeTimer.OneShot = true;
        lungeTimer.Timeout += EndLunge;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player != null && new BetterMath().DistanceBetweenTwoVector(GlobalPosition,_player.GlobalPosition) < lungeRange && !lungeTimer.IsStopped())
        {
            LungeAttack();
        }
        else
        {
            base._PhysicsProcess(delta);
        }
    }

    private void LungeAttack()
    {
        _usePathFinding = false;
        _shouldMove = true;
        Velocity = (GlobalPosition.DirectionTo(_player.GlobalPosition) * lungeSpeed);
        _iFrames = lungeDuration;
        lungeTimer.Start();
    }

    private void EndLunge()
    {
        _usePathFinding = true;
        _iFrames = 0;
        Velocity = Vector2.Zero;
    }
}

