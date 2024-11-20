using Godot;
using System;
using System.Collections.Generic;

public partial class HUDManager : Control
{
	public static HUDManager Instance { get; private set; }

	[Export]
	public Godot.Collections.Array<TextureRect> HealthIconList;
	private Node2D player;

	
	private double _fpsUpdateTimer = 1;
	[Export] private Label _fpsLabel;

	[Export] private Label _ammoCount;
	public override void _Ready()
	{
		 Instance = this;

		 player = GetTree().GetNodesInGroup("Player")[0] as Node2D;
	}

	public override void _Process(double delta)
	{
		_fpsUpdateTimer -= delta;
		if (_fpsUpdateTimer < 0) {
			_fpsLabel.Text = "FPS: " + Engine.GetFramesPerSecond();
			_fpsUpdateTimer = 0.75;
		}

		_updateAmmo();
	}

	public void IncreasePlayerHp() {
		foreach (TextureRect image in HealthIconList) {
			if (image.Texture.ResourcePath == "res://Assets/Sprites/Menu/heart-empty.png") {
				image.Texture = GD.Load<Texture2D>("res://Assets/Sprites/Menu/heart.png");
				return;
			}
		}
	}
	public void DecreasePlayerHp()
	{
		//HealthIconList[(player as Player).GetPlayerHealth()].Visible = false;
		HealthIconList[(player as Player).GetPlayerHealth()].Texture = GD.Load<Texture2D>("res://Assets/Sprites/Menu/heart-empty.png");
	}

	private void _updateAmmo()
	{
		Player temp = (Player)player;
		_ammoCount.Text = $"{temp.Ammo}/{temp.MaxAmmo}";
	}
}
