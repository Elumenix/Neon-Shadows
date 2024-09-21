using Godot;
using System;

public partial class BaseEnemyAI : CharacterBody2D
{
    [ExportCategory("Health")]
    [Export] private int _maxHealth = 100;
    private int currentHealth;

    [ExportCategory("Movement")]
    [Export] protected float _speed;
    [Export] private NavigationAgent2D _navigationAgent;

    protected Node2D _player;
    protected bool _usePathFinding = true;
    protected bool _isPlayerInRange = true;
    protected bool _shouldMove = true;

    [Export]
    public float playerDetectRange;

    public override async void _Ready()
    {
        currentHealth = _maxHealth;

        // Small delay for initialization
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

        // Get player node (assuming player is in the "Player" group)
        _player = GetTree().GetNodesInGroup("Player")[0] as Node2D;

        // Connect the Area2D signal for collision detection
        GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
        UpdateNavigationTarget();
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateNavigationTarget();

        if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }

        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

        MoveTowardsTarget(nextPathPosition, delta);
    }

    public override void _Process(double delta)
    {
    }


    public void CheckIsPlayerInRange() {
        float distanceToPlayer = new BetterMath().DistanceBetweenTwoVector(_player.GlobalPosition, GlobalPosition);
    }

    private void UpdateNavigationTarget()
    {
        if (_player != null)
        {
            _navigationAgent.TargetPosition = _player.GlobalPosition; 
        }
    }

    private void MoveTowardsTarget(Vector2 targetPosition, double delta)
    {
        if (_usePathFinding && _shouldMove) {
            Vector2 direction = (targetPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * (float)(_speed * delta);
        }
    }

    /// <summary>
    /// Manages enemy's health when taking damage
    /// </summary>
    /// <param name="damageAmount"></param>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleDeath();
        }
    }

    /// <summary>
    /// Handle enemy death
    /// </summary>
    private void HandleDeath()
    {
        QueueFree();
    }

    // This function will be called when a collision with the player happens
    public void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            HandlePlayerCollision();
        }
    }

    public void HandlePlayerCollision()
    {
        (_player as Player).takeDamage(1);
        HUDManager.Instance.DecreasePlayerHp();
        Camera.Instance.StartShakeCamera(0.1f, 25);
    }
}
