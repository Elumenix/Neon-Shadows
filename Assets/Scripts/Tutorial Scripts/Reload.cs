using Godot;
using System;

public partial class Reload : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnEnter(Node2D body)
	{
		if(body is Player)
		{
			Player temp = (Player)body;
			temp.Reload();
		}
	}
}
