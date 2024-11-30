using Godot;
using System;
using System.Collections.Generic;

public partial class Explosion : RigidBody2D
{
	private Timer _timer;
	private AnimationPlayer _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("%Timer");
		_player = GetNode<AnimationPlayer>("%AnimationPlayer");
		_player.Play("Explode");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public override void _PhysicsProcess(double delta)
	{
		// Object is kept here until sound effect ends
		if(!_player.PlaybackActive) ExplosionEnd();
		
		// Timer signifies how long the hit box is active
		if (_timer.TimeLeft == 0) return;
		var collision = MoveAndCollide(Vector2.Zero, true);
		if (collision != null)
		{
			if (collision.GetCollider() is Player)
			{
				Player player = (Player)collision.GetCollider();
				player.takeDamage(1);
				HUDManager.Instance.DecreasePlayerHp();
			}
			else if (collision.GetCollider() is BaseEnemyAI)
			{
				BaseEnemyAI enemyAI = (BaseEnemyAI)collision.GetCollider();
				enemyAI.TakeDamage(300);
			}
			else if (collision.GetCollider() is DroneAI)
			{
				DroneAI enemy = (DroneAI)collision.GetCollider();
				enemy.TakeDamage(300);
			}
		}
	}
	public void InExplosion(Node collider)
	{
		GD.Print("Inside Explosion");
		if(collider is Player)
		{
			Player player = (Player)collider;
			player.takeDamage(1);
			HUDManager.Instance.DecreasePlayerHp();
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
	/// Called when the sound effect ends. Deletes the Explosion from the scene
	/// </summary>
	public void ExplosionEnd()
	{
		QueueFree();
	}
}
