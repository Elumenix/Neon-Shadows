using Godot;
using System;
using static Godot.HttpClient;

public partial class BaseCollisionHandler : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnCollisionEnter;
	}

	private void OnCollisionEnter(Node body)
	{
		GD.Print("hit!");
		if (body is Player player)
		{
			//player take damage method

		}
	}
}
