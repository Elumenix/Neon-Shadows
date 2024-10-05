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
	public override void _Ready()
	{
		 Instance = this;

		 player = GetTree().GetNodesInGroup("Player")[0] as Node2D;
	}

    public override void _Process(double delta)
    {
		_fpsUpdateTimer -= delta;

		if (_fpsUpdateTimer < 0) {
            int fps = (int)(1 / delta);
            _fpsLabel.Text = "FPS: " + fps.ToString();
			_fpsUpdateTimer = 1;
        }
    }

    public void IncreasePlayerHp() {
		foreach (TextureRect image in HealthIconList) {
			if (!image.Visible) { 
				image.Visible = true;
				return;
			}
		}
	}
	public void DecreasePlayerHp()
	{
		HealthIconList[(player as Player).GetPlayerHealth()].Visible = false;
	}
}
