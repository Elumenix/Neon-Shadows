using Godot;
using System;
using static Godot.HttpClient;

public partial class BaseCollisionHandler : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("hi");
		AreaEntered += OnCollisionEntered;
		GetNode<Area2D>("Area2D").Connect("area_entered", new Callable(this, "OnCollisionEntered"));
    }

    public override void _Process(double delta)
    {
    }
	private void OnCollisionEntered(Node2D body) {
		GD.Print("hi");
	}
}
