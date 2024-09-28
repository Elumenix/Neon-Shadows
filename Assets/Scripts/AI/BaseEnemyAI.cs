using Godot;
using System;

public partial class BaseEnemyAI : CharacterBody2D
{
    [ExportCategory("Health")]
    [Export] public int MaxHealth = 100;
    private int currentHealth;
    [Export] private float flashTime = 0.1f;

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
        FlashOnDamge();
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
        Player temp = (Player)player;
        // Moving the screen shake before damage means the screen shakes on the last damage
        // But not if the slimes collide after the player has 0 health
        if (!temp.IsDead)
        {
            Camera.Instance.StartShakeCamera(0.1f, 25);
        }
        temp.takeDamage(1);
        HUDManager.Instance.DecreasePlayerHp();
        
    }


    public void FlashOnDamge() {
        GetNode<AnimationPlayer>("EnemySprite/FlashAnimation").Play("Flash");
    }
}
