using Godot;
using System;

public partial class OozeAi : BaseEnemyAI
{
	[Export] private float _lungeSpeed = 200.0f;
	[Export] private float _lungeRange = 100.0f;
	[Export] private float _lungeDuration = 0.5f;
	private Timer _lungeTimer;
	private Timer _lungeCooldownTimer;
	private bool _isLunging = false;
	private bool _isLungeCooldown = false;
	[Export] private AnimationPlayer _animationPlayer;
	private Timer _ZIndexTimer;

	public override void _Ready()
	{
		base._Ready();

        float randomScale = (float)GD.RandRange(0.9, 1.1);
		Scale = new Vector2(randomScale, randomScale);
		Modulate = new Color(0, 0, (float)GD.RandRange(0.3, 0.7));

		_lungeCooldownTimer = GetNode<Timer>("LungeCooldownTimer");
		_lungeCooldownTimer.Start();
		_lungeCooldownTimer.Timeout += EndCooldown;
		_lungeTimer = new Timer();
		AddChild(_lungeTimer);
		_lungeTimer.WaitTime = _lungeDuration;
		_lungeTimer.OneShot = true;
		_lungeTimer.Timeout += EndLunge;
		_ZIndexTimer.Timeout += ChangeZIndex;

		GD.Print("sd: "+_animationPlayer);

        _animatedSprite.Play("Walk");
    }

	public override void _PhysicsProcess(double delta)
	{
		if (IsOnPlatform()) {
            if (_player != null && !_isLungeCooldown && GlobalPosition.DistanceTo(_player.GlobalPosition) < _lungeRange)
            {
                StartLunge();
            }
            else
            {
                UpdateNavigationTarget();
                Vector2 nextPathPosition = nextPathPosition = _navigationAgent.GetNextPathPosition();
                if (_navigationAgent.DistanceToTarget() < _detectionRange)
                {
                    _shouldMove = true;
                }
                else
                {
                    _shouldMove = false;
                }

                if (_navigationAgent.IsNavigationFinished())
                {
                    nextPathPosition = GlobalPosition;
                }

                MoveTowardsTarget(nextPathPosition, delta);
            }

            MoveAndSlide();
        }
		
        
	}

	
	private bool IsOnPlatform()
	{
		return GetTree().GetNodesInGroup("PlatformArea")[0].GetNode<Area2D>("PlatformArea").OverlapsArea(GetNode<Area2D>("EdgeDetect"));
	}
	
	private void StartLunge()
	{
		_isLunging = true;
		_isLungeCooldown = true;
        _usePathFinding = false;
		_shouldMove = true;
		
		Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
		Velocity = direction * _lungeSpeed;
		
		_iFrames = _lungeDuration;
		_lungeTimer.Start();
        _animatedSprite.Play("Lunge");
    }

	private void EndLunge()
	{
		_usePathFinding = true;
		_iFrames = 0;
		Velocity = Vector2.Zero;
		_lungeCooldownTimer.Start();
		_animatedSprite.Play("Walk");

        _isLunging = false;

		if (!IsOnPlatform())
		{
            _animationPlayer.Play("Fall");
            GetNode<Timer>("ZIndexTimer").Start();
        }
    
    }

	private void ChangeZIndex() {
		ZIndex = -1;
	}


    private void EndCooldown() {
        _isLungeCooldown = false;

    }

	public override void TakeDamage(int damageAmount)
	{
		if (!_isLunging)
		{
			base.TakeDamage(damageAmount);
		}
	}
}

