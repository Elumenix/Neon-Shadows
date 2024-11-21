using Godot;
using System;
using System.Collections.Generic;

public partial class HUDManager : Control
{
	public static HUDManager Instance { get; private set; }

	[Export]
	public Godot.Collections.Array<TextureRect> HealthIconList;
	private Node2D player;

	[Export]
	public Godot.Collections.Array<TextureRect> AmmoIconList;
	
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
		/*for(int i = 1; i < temp.MaxAmmo+1; i++)
		{
			if (i > temp.Ammo)
			{
				AmmoIconList[i].Texture = GD.Load<Texture2D>("res://Assets/Sprites/Menu/ammo-empty.png");
			}
			else
			{
				AmmoIconList[i].Texture = GD.Load<Texture2D>("res://Assets/Sprites/Menu/ammo-full.png");
			}
		}*/
	}

	public void DecreaseAmmo()
	{
		AmmoIconList[(player as Player).Ammo].Texture = GD.Load<Texture2D>("res://Assets/Sprites/Menu/ammo-empty.png");
	}
	public void IncreaseAmmo()
	{
		AmmoIconList[(player as Player).Ammo -1].Texture = GD.Load<Texture2D>("res://Assets/Sprites/Menu/ammo-full.png");
	}
}
