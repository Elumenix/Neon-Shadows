using Godot;
using System;

public partial class PlayButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_pressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Scenes/TileMapTest.tscn");
	}
	
	public void _on_leave()
	{
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
	}
}
