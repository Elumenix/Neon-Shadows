using Godot;
using System;

public partial class FirstLedge : Area2D
{
	// Fields
	// Walk Text
	[Export] private Label _walkText;
	// Enemy Rich Text
	[Export] private RichTextLabel _enemyWarningText;
	// Enemy (have disabled at start?)
	[Export] private BaseEnemyAI _enemy;
	// Dash Rich Text
	[Export] private RichTextLabel _dashText;

	private bool _enemyDead = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEnter(Node collide)
	{
		if (collide == null) return;
		if (collide is Player player)
		{
			if (!_enemyDead)
			{
				// Display Enemy Warning Text
			}
			else
			{
				// Display Dash Text
			}
		}
	}

	public void OnExit(Node collide)
	{
		if (collide == null) return;
		if(collide is Player player)
		{
			// if Warning text is up hide that

			// if Dash Text is up hide that
		}
	}
}
