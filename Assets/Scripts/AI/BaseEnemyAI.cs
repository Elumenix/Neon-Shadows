using Godot;
using System;

public partial class BaseEnemyAI : Node2D
{
    [ExportCategory("Health")]
    [Export] public int MaxHealth = 100;
    private int currentHealth;

    [ExportCategory("Movement")]
    // Speed of the enemy
    [Export] public float Speed = 100f;


    private Node2D player;
    public override void _Ready()
	{
        currentHealth = MaxHealth;
        player = GetNodeOrNull<Node2D>("");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Movement(delta);
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
    private void OnCollisionEnter(Node body)
    {
        if (body is Player player)
        {
            //player take damage method
        }
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
}
