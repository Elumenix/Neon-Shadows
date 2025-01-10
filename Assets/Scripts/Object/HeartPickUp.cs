using Godot;
using System;

public partial class HeartPickUp : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _onEnter(Node2D body)
	{
		if(body is Player player)
		{
			if(player.GetPlayerHealth() < 3)
			{
				player.takeDamage(-1);
				QueueFree();
			}
		}
	}
}
