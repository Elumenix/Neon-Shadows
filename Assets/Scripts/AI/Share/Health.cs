using Godot;
using System;

public partial class Health : Node
{
	[Export] public int MaxHealth = 100;
	private int currentHealth;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentHealth = MaxHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
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
}
