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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnCollisionEnter(Node body)
	{
		if (body is Player player)
		{
			//player take damage method
		}
	}
}
