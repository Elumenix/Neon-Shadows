using Godot;
using System;

public partial class HeartPickUp : Area2D
{
	[Export] 
	private double lifetime = 10;
	private bool startedIdle;
	private bool fullyTransitioned;
	private bool noCollision;
	
	[Export]
	private AnimationPlayer animator;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Play spawn animation immediately
		animator.Play("Spawn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		lifetime -= delta;
		
		// destroy after animation finishes playing
		if(lifetime <= 0 && fullyTransitioned && !animator.IsPlaying())
		{
			QueueFree();
		}

		// Condition to start the despawn animation
		if (lifetime <= 1 && !animator.IsPlaying())
		{
			animator.Play("Despawn");
			fullyTransitioned = true;
		}
		else if (!animator.IsPlaying() && startedIdle)
		{
			// Idle animation if nothing else is playing
			animator.Play("Idle");
		}
		else if (!animator.IsPlaying() && !startedIdle)
		{
			animator.Play("BeginIdle");
			startedIdle = true;
		}
	}

	public void _onEnter(Node2D body)
	{
		// Early out if final animations are playing
		if (noCollision)
		{
			return;
		}
		
		if(body is Player player)
		{
			if(player.GetPlayerHealth() < 3)
			{
				player.takeDamage(-1);
				
				// Switch to grab animation no matter what
				animator.Stop();
				animator.Play("Grabbed");
				
				// Allows for deque
				lifetime = 0;
				noCollision = true;
			}
		}
	}
}
