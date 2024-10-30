using Godot;
using System;

public partial class Explosion : RigidBody2D
{
	private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void InExplosion(Node collider)
	{
		if(collider is Player)
		{
			Player player = (Player)collider;
			player.takeDamage(1);
		}
		else if(collider is BaseEnemyAI)
		{
			BaseEnemyAI enemyAI = (BaseEnemyAI)collider;
			enemyAI.TakeDamage(100);
		}
		else if(collider is DroneAI)
		{
			DroneAI enemy = (DroneAI)collider;
			enemy.TakeDamage(100);
		}
	}
	/// <summary>
	/// Called when the timer ends. Deletes the Explosion from the scene
	/// </summary>
	public void ExplosionEnd()
	{
		QueueFree();
	}
}
