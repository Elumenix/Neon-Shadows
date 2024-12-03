using Godot;
using System;

public partial class SecondLedge : Area2D
{
	private Label shootText;
	private RichTextLabel fireText;
	private bool showOnce = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shootText = GetNode<Label>("%ShootText");
		fireText = GetNode<RichTextLabel>("%FireText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnExit(Node2D body)
	{
		if(body is Player)
		{
			fireText.Hide();
			if (!showOnce)
			{
				shootText.Show();
				showOnce = true;
			}
		}
	}
}
