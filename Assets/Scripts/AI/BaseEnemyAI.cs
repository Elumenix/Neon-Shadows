using Godot;
using System;

public partial class BaseEnemyAI : CharacterBody2D
{
    [ExportCategory("Health")]
    [Export] public int MaxHealth = 100;
    private int currentHealth;

    [ExportCategory("Movement")]
    // Speed of the enemy
    [Export] public float Speed;

    private Node2D player;

    public override async void _Ready()
    {
        currentHealth = MaxHealth;
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        player = GetTree().GetNodesInGroup("Player")[0] as Node2D;
        GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
    }


    public override void _Process(double delta)
    {
        Movement(delta);
    }

    public override void _PhysicsProcess(double delta)
    {

    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        GD.Print("Enemy died.");
        QueueFree();
    }

    private void Movement(double delta)
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

        MoveTowardsTarget(targetPosition, delta);
    }

    private void MoveTowardsTarget(Vector2 targetPosition, double delta)
    {
        Vector2 direction = (targetPosition - Position).Normalized();
        Position += direction * (float)(Speed * delta);
    }

    // This function will be called when a collision with the player happens
    public void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            HandlePlayerCollison();
        }
    }
    public void HandlePlayerCollison()
    {
        GD.Print("Enemy collided with the player!");
    }
}