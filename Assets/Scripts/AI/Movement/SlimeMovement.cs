using Godot;
using System;

public partial class SlimeMovement : Node2D
{
	// Speed of the enemy
	[Export] public float Speed = 100f;
	
	private Node2D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 player = GetNodeOrNull<Node2D>("");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Movement(delta);
	}
	
	private void Movement(double delta){
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
		Position += direction * (float) (Speed * delta);
	}
}
