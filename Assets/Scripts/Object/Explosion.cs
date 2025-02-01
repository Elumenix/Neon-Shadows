using Godot;
using System;
using System.Collections.Generic;

public partial class Explosion : Area2D
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
		// Object is kept here until sound effect ends
		if(!_player.PlaybackActive) ExplosionEnd();
		
		// Timer signifies how long the hit box is active
		if (_timer.TimeLeft == 0) return;

		foreach (var body in GetOverlappingBodies())
		{
			if (body is Player player)
			{
				player.takeDamage(1);
				HUDManager.Instance.DecreasePlayerHp();
			}
			else if (body is BaseEnemyAI enemyAI)
			{
				enemyAI.TakeDamage(300);
			}
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
