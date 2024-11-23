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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_walkText.Visible = true;
		_enemyWarningText.Visible = false;
		_enemy.Visible = false;
		_enemy.ShouldMove = false;
		_enemy.ProcessMode = ProcessModeEnum.Disabled;
		_dashText.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void OnEnter(Node2D collide)
	{
		if (collide == null) return;
		if (collide is Player)
		{
			if (GameManager.Instance.DefeatedEnemies < 1)
			{
				// Move the text over the player's head
				//_enemyWarningText.GlobalPosition = tempPosition;
				
				// Display Enemy Warning Text
				_enemyWarningText.Show();
				_enemy.ProcessMode = ProcessModeEnum.Inherit;
				_enemy.Show();
				_enemy.ShouldMove = true;
			}
			else
			{
				// Display Dash Text
				_dashText.Show();
			}
		}
	}

	public void OnExit(Node2D collide)
	{
		if (collide == null) return;
		if(collide is Player player)
		{
			// if Warning text is up hide that
			_enemyWarningText.Hide();
			// if Dash Text is up hide that
			_dashText.Hide();
		}
	}
}
