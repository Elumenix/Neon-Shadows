using Godot;
using System;

public partial class Reload : Area2D
{
	private Label text;
	private Label shootText;
	private bool showOnce = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		text = GetNode<Label>("%ReloadText");
		shootText = GetNode<Label>("%ShootText");
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
			shootText.Hide();
			if (!showOnce)
			{
				showOnce = true;
				text.Show();
			}
		}
	}
	public void OnExit(Node2D body)
	{
		if(body is Player)
		{
			Player temp = (Player)body;
			temp.Reload();
			if (text.Visible)
			{
				text.Hide();
			}
		}
	}
}
