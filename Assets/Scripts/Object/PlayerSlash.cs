using Godot;
using System;

public partial class PlayerSlash : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_area_entered(Area2D collision)
	{
		if(collision.Owner is BaseEnemyAI)
		{
			BaseEnemyAI temp = (BaseEnemyAI)collision.Owner;
			temp.TakeDamage(50);
		}
	}
}
