using Godot;
using System;

public partial class WinArea : Area2D
{
	private Label _winText;
	// Button?

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_winText = GetNode<Label>("%WinText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEnter(Node2D body)
	{
		if(body is Player)
		{
			_winText.Show();
		}
	}
	public void OnExit(Node2D body)
	{
		if(body is Player)
		{
            (GetTree().GetNodesInGroup("LevelCompleteMenu")[0] as CanvasLayer).Visible = true;
			GameManager.Instance.gamePaused = true;
        }
	}
}
