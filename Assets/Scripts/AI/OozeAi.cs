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

        float randomScale = (float)GD.RandRange(0.8, 2);
		Scale = new Vector2(randomScale, randomScale);
		Modulate = new Color(0, 0, (float)GD.RandRange(0.3, 0.7));

		_lungeCooldownTimer = GetNode<Timer>("LungeCooldownTimer");
		_lungeCooldownTimer.Timeout += EndCooldown;
		_lungeTimer = new Timer();
		AddChild(_lungeTimer);
		_lungeTimer.WaitTime = _lungeDuration;
		_lungeTimer.OneShot = true;
		_lungeTimer.Timeout += EndLunge;
		
		_ZIndexTimer = GetNode<Timer>("ZIndexTimer");
		_ZIndexTimer.Timeout += ChangeZIndex;

        _oneSecTimer = GetNode<Timer>("OneSecTimer");
        _oneSecTimer.Timeout += OnOneSecTimerFinished;
        _shouldMove = false;
        Spawn();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsOnPlatform() && _shouldMove)
        {
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
                PlayWalkAnimation(GlobalPosition.DirectionTo(nextPathPosition));
            }

            MoveAndSlide();
        }


    }

    public void OnOneSecTimerFinished()
    {
        if (_animatedSprite.Animation == "Spawn")
        {
            _shouldMove = true;
            _animatedSprite.Play("Walk_Down");
            _lungeCooldownTimer.Start();
        }
        else if (_animatedSprite.Animation.ToString().Substring(0, 5) == "Death") {
            QueueFree();
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
        PlayLungeAnimation(direction);
    }

	private void EndLunge()
	{
		_usePathFinding = true;
		_iFrames = 0;
		Velocity = Vector2.Zero;
		_lungeCooldownTimer.Start();

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
        if (isDead)
            return;
        
        if (!_isLunging)
		{
			base.TakeDamage(damageAmount);
		}
	}


	private void PlayLungeAnimation(Vector2 direction) {
        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Lunge_UpRight");
        }
        else if (direction.Y < -0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Lunge_UpLeft");
        }
        else if (direction.Y > 0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Lunge_DownRight");
        }
        else if (direction.Y > 0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Lunge_DownLeft");
        }
        else if (direction.Y < -0.5f)
        {
            _animatedSprite.Play("Lunge_Up");
        }
        else if (direction.Y > 0.5f)
        {
            _animatedSprite.Play("Lunge_Down");
        }
        else if (direction.X < -0.5f)
        {
            _animatedSprite.Play("Lunge_Left");
        }
        else if (direction.X > 0.5f)
        {
            _animatedSprite.Play("Lunge_Right");
        }
    }

    private void PlayWalkAnimation(Vector2 direction)
    {
        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Walk_UpRight");
        }
        else if (direction.Y < -0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Walk_UpLeft");
        }
        else if (direction.Y > 0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Walk_DownRight");
        }
        else if (direction.Y > 0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Walk_DownLeft");
        }
        else if (direction.Y < -0.5f)
        {
            _animatedSprite.Play("Walk_Up");
        }
        else if (direction.Y > 0.5f)
        {
            _animatedSprite.Play("Walk_Down");
        }
        else if (direction.X < -0.5f)
        {
            _animatedSprite.Play("Walk_Left");
        }
        else if (direction.X > 0.5f)
        {
            _animatedSprite.Play("Walk_Right");
        }
    }

    private void PlayDeathAnimation(Vector2 direction)
    {
        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Death_UpRight");
        }
        else if (direction.Y < -0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Death_UpLeft");
        }
        else if (direction.Y > 0.5f && direction.X > 0.5f)
        {
            _animatedSprite.Play("Death_DownRight");
        }
        else if (direction.Y > 0.5f && direction.X < -0.5f)
        {
            _animatedSprite.Play("Death_DownLeft");
        }
        else if (direction.Y < -0.5f)
        {
            _animatedSprite.Play("Death_Up");
        }
        else if (direction.Y > 0.5f)
        {
            _animatedSprite.Play("Death_Down");
        }
        else if (direction.X < -0.5f)
        {
            _animatedSprite.Play("Death_Left");
        }
        else if (direction.X > 0.5f)
        {
            _animatedSprite.Play("Death_Right");
        }
    }

    protected override void HandleDeath() {
        isDead = true;
        GameManager.Instance.EnemyDefeated();
        Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
        PlayDeathAnimation(direction);
        _shouldMove = false;
        _oneSecTimer.Start();
    }

    protected void Spawn()
    {
        if (_animatedSprite == null)
            _animatedSprite = GetNode<AnimatedSprite2D>("EnemySprite");

        _oneSecTimer.Start();
        _animatedSprite.Play("Spawn");
    }
}

