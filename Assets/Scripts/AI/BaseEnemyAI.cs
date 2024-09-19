using Godot;
using System;

public partial class BaseEnemyAI : CharacterBody2D
{
    [ExportCategory("Health")]
    [Export] public int MaxHealth = 100;
    private int currentHealth;

    [ExportCategory("Movement")]
    [Export] private float _speed;
    [Export] private NavigationAgent2D _navigationAgent;

    private Node2D player;

    public override async void _Ready()
    {
        currentHealth = MaxHealth;

        // Small delay for initialization
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

        // Get player node (assuming player is in the "Player" group)
        player = GetTree().GetNodesInGroup("Player")[0] as Node2D;

        // Connect the Area2D signal for collision detection
        GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;

        // Set the initial movement target
        CallDeferred("SetMovementTarget");
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
        Vector2 targetPosition;
        if (player != null)
        {
            targetPosition = player.Position;
        }
        else
        {
            targetPosition = new Vector2(0, 0);
        }

    }

    private void UpdateNavigationTarget()
    {
        if (player != null)
        {
            _navigationAgent.TargetPosition = player.GlobalPosition; 
        }
    }

    private void MoveTowardsTarget(Vector2 targetPosition, double delta)
    {
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * (float)(_speed * delta);
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
        GD.Print("Enemy died.");
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
        GD.Print("Enemy collided with the player!");
        Camera.Instance.StartShakeCamera(0.1f, 50);
    }
}
