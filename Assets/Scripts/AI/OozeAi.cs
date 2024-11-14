using Godot;
using System;

public partial class OozeAi : BaseEnemyAI
{
	 [Export] private float lungeSpeed = 200.0f;
	[Export] private float lungeRange = 100.0f;
	[Export] private float lungeDuration = 0.5f;
	private Timer lungeTimer;
	private bool isLunging = false;

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

		if (_speed <= 0) _speed = 50.0f;
		if (_maxSpeed <= 0) _maxSpeed = 100.0f;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player != null && !isLunging && Position.DistanceTo(_player.Position) < lungeRange)
		{
			StartLunge();
		}
		else if (!isLunging)
		{
			base._PhysicsProcess(delta);
		}

		MoveAndSlide();
	}

	private void StartLunge()
	{
		isLunging = true;
		_usePathFinding = false;
		_shouldMove = true;
		
		Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
		Velocity = direction * lungeSpeed;
		
		_iFrames = lungeDuration;
		lungeTimer.Start();
	}

	private void EndLunge()
	{
		isLunging = false;
		_usePathFinding = true;
		_iFrames = 0;
		Velocity = Vector2.Zero;
	}

	public override void TakeDamage(int damageAmount)
	{
		if (!isLunging)
		{
			base.TakeDamage(damageAmount);
		}
	}
}
