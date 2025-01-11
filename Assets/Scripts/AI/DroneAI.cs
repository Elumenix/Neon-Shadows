using Godot;
using System;
using System.Threading.Tasks;

public partial class DroneAI : BaseEnemyAI
{
    public enum DroneFSM
    {
        Stop,
        Detecting,
        TrackPlayer,
        AttackCharging,
        Attacking
    }

    [Export] private float _moveStopRange = 25f;
    [Export] private float _shootOffset = 0.1f;
    [Export] private PackedScene _bullet;
    [Export] private CpuParticles2D _particles;
    [Export] private AudioStream deathSound;
    [Export] private Vector2 targetMaxOffset = new Vector2(50, 50);

    private DroneFSM _currentState = DroneFSM.Stop;

    [Export] private float shootCooldown = 0.5f;
    private bool _movementCompleted;
    private Vector2 _targetPosition;
    private AudioStreamPlayer2D _droneHitAudio;
    private double _attackCooldownTimer = 0.0;
    private Timer _stateTimer;
    private bool _hasFired = false;

    public override void _Ready()
    {
        base._Ready();

        _droneHitAudio = GetNode<AudioStreamPlayer2D>("%DroneHit");

        _oneSecTimer = GetNode<Timer>("OneSecTimer");
        _oneSecTimer.Timeout += OnOneSecTimerFinished;

        _stateTimer = GetNodeOrNull<Timer>("StateTimer");
        if (_stateTimer == null)
        {
            _stateTimer = new Timer();
            AddChild(_stateTimer);
        }
        _movementCompleted = true;
        Spawn();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (isDead) return;

        if (_attackCooldownTimer > 0)
        {
            _attackCooldownTimer -= delta;
        }

        HandleState(delta);
    }
    public override void _PhysicsProcess(double delta) { 
        
    }
    private void OnOneSecTimerFinished()
    {
        if (_animatedSprite.Animation == "Spawn")
        {
            _shouldMove = true;
            _animatedSprite.Play("Forward");
            SetState(DroneFSM.Detecting);
        }
        else if (_animatedSprite.Animation.ToString().Substring(0,5) == "Death")
        {
            QueueFree();
        }
    }

    private void HandleState(double delta)
    {
        switch (_currentState)
        {
            case DroneFSM.Stop:
                if (_attackCooldownTimer <= 0)
                {
                    SetState(DroneFSM.Detecting);
                }
                break;

            case DroneFSM.Detecting:
                if (IsPlayerDetected())
                {
                    SetState(DroneFSM.TrackPlayer);
                    GenerateNewTarget();
                }
                break;

            case DroneFSM.TrackPlayer:
                MoveTowardsTarget(delta);
                break;

            case DroneFSM.AttackCharging:
                PlayAttackAnimation(GlobalPosition.DirectionTo(_player.GlobalPosition));
                if (!_hasFired)
                {
                    _hasFired = true;

                    var callable = new Callable(this, nameof(OnAttackChargingComplete));
                    if (_stateTimer.IsConnected("timeout", callable))
                    {
                        _stateTimer.Disconnect("timeout", callable);
                    }

                    _stateTimer.Timeout += OnAttackChargingComplete;
                    _stateTimer.Start(0.5f);
                }
                break;

            case DroneFSM.Attacking:
                Attack();
                break;
        }
    }

    private void SetState(DroneFSM newState)
    {
        if (_currentState == newState) return;

        GD.Print($"Drone transitioning from {_currentState} to {newState}");
        _currentState = newState;

        if (newState == DroneFSM.AttackCharging)
        {
            _hasFired = false;
        }
    }
    private void OnAttackChargingComplete()
    {
        if (_currentState == DroneFSM.AttackCharging)
        {
            SetState(DroneFSM.Attacking);
        }

        _stateTimer.Disconnect("timeout", new Callable(this, nameof(OnAttackChargingComplete)));
    }
    private async void Attack()
    {
        if (isDead) return;

        _attackCooldownTimer = shootCooldown;

        Vector2 bulletDirection = GlobalPosition.DirectionTo(_player.GlobalPosition);
        float shootAngle = Mathf.Atan2(bulletDirection.Y, bulletDirection.X) +
                           (float)GD.RandRange(-_shootOffset, _shootOffset);

        var bullet = _bullet.Instantiate<Bullet>();
        bullet.Position = GlobalPosition;
        bullet.Rotation = shootAngle;
        bullet.player = _player;
        GetParent().AddChild(bullet);

        PlayBulletSound();

        SetState(DroneFSM.Stop);
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        SetState(DroneFSM.Detecting);
    }

    private bool IsPlayerDetected()
    {
        if (_player == null) return false;

        return GlobalPosition.DistanceTo(_player.GlobalPosition) < _detectionRange;
    }

    private void MoveTowardsTarget(double delta)
    {
        if (!_movementCompleted)
        {
            Vector2 direction = (_targetPosition - GlobalPosition).Normalized();
            Velocity = direction * _speed;
            Velocity = Velocity.LimitLength(_maxSpeed);

            UpdateAnimation(direction);

            if (GlobalPosition.DistanceTo(_targetPosition) < 1)
            {
                _movementCompleted = true;
                SetState(DroneFSM.AttackCharging);
            }
        }

        var collision = MoveAndCollide(Velocity * (float)delta);
        if (collision != null)
        {

        }
    }

    private void GenerateNewTarget()
    {
        if (_player == null) return;

        _targetPosition = _player.GlobalPosition +
            new Vector2(
                (float)GD.RandRange((double)-targetMaxOffset.X, (double)targetMaxOffset.X),
                (float)GD.RandRange((double)-targetMaxOffset.Y, (double)targetMaxOffset.Y)
            );

        _movementCompleted = false;
        GD.Print(_targetPosition +":" + _player.GlobalPosition);
    }

    private void PlayBulletSound()
    {
        _droneHitAudio.PitchScale = (float)GD.RandRange(1.0, 1.5);
        _droneHitAudio.Play();
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (isDead) return;

        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
            _animatedSprite.Play("Backward-Rightward");
        else if (direction.Y < -0.5f && direction.X < -0.5f)
            _animatedSprite.Play("Backward-Leftward");
        else if (direction.Y > 0.5f && direction.X > 0.5f)
            _animatedSprite.Play("Forward-Rightward");
        else if (direction.Y > 0.5f && direction.X < -0.5f)
            _animatedSprite.Play("Forward-Leftward");
        else if (direction.Y < -0.5f)
            _animatedSprite.Play("Backward");
        else if (direction.Y > 0.5f)
            _animatedSprite.Play("Forward");
        else if (direction.X < -0.5f)
            _animatedSprite.Play("Leftward");
        else if (direction.X > 0.5f)
            _animatedSprite.Play("Rightward");
    }

    private void PlayAttackAnimation(Vector2 direction)
    {
        if (isDead) return;

        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
            _animatedSprite.Play("Attack_UpRight");
        else if (direction.Y < -0.5f && direction.X < -0.5f)
            _animatedSprite.Play("Attack_UpLeft");
        else if (direction.Y > 0.5f && direction.X > 0.5f)
            _animatedSprite.Play("Attack_DownRight");
        else if (direction.Y > 0.5f && direction.X < -0.5f)
            _animatedSprite.Play("Attack_DownLeft");
        else if (direction.Y < -0.5f)
            _animatedSprite.Play("Attack_Up");
        else if (direction.Y > 0.5f)
            _animatedSprite.Play("Attack_Down");
        else if (direction.X < -0.5f)
            _animatedSprite.Play("Attack_Left");
        else if (direction.X > 0.5f)
            _animatedSprite.Play("Attack_Right");
    }

    private void PlayDeathAnimation(Vector2 direction)
    {
        if (isDead) return;

        direction = direction.Normalized();
        if (direction.Y < -0.5f && direction.X > 0.5f)
            _animatedSprite.Play("Death_UpRight");
        else if (direction.Y < -0.5f && direction.X < -0.5f)
            _animatedSprite.Play("Death_UpLeft");
        else if (direction.Y > 0.5f && direction.X > 0.5f)
            _animatedSprite.Play("Death_DownRight");
        else if (direction.Y > 0.5f && direction.X < -0.5f)
            _animatedSprite.Play("Death_DownLeft");
        else if (direction.Y < -0.5f)
            _animatedSprite.Play("Death_Up");
        else if (direction.Y > 0.5f)
            _animatedSprite.Play("Death_Down");
        else if (direction.X < -0.5f)
            _animatedSprite.Play("Death_Left");
        else if (direction.X > 0.5f)
            _animatedSprite.Play("Death_Right");
    }

    private void Spawn()
    {
        if (_animatedSprite == null)
            _animatedSprite = GetNode<AnimatedSprite2D>("EnemySprite");

        _animatedSprite.Play("Spawn");
        _oneSecTimer.Start();
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();

        PlayDeathAnimation(GlobalPosition.DirectionTo(_player.GlobalPosition));
        _animatedSprite.Play("Death");
        _oneSecTimer.Start();
    }
}
