using Godot;
using System;

public partial class BaseCollisionHandler : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 Connect("body_entered", this, nameof(OnBodyEntered));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (body is Player) 
		{
			GD.Print("player collided");
		}
		else
		{
			GD.Print("collided with other gameobject");
		}
	}
}
