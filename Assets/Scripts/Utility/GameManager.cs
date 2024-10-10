using Godot;
using System;

public partial class GameManager : Node
{

    private CanvasLayer _pauseMenu;
	private Node2D _player;


    //game states
	public bool gamePaused;
    public int currentLevel;


    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public override void _Ready()
    {
        _instance = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause")) {
			PauseGame();
		}
	}

    public void PauseGame() {

        _pauseMenu = GetTree().GetNodesInGroup("PauseMenu")[0] as CanvasLayer;
        _player = GetTree().GetNodesInGroup("Player")[0] as Node2D;
        

        //toggle pause menu and disable player controls
        _pauseMenu.Visible = !_pauseMenu.Visible;
        gamePaused = _pauseMenu.Visible;

        if (_pauseMenu.Visible)
        {
            Engine.TimeScale = 0;
        }
        else
        {
            Engine.TimeScale = 1;
        }
    }
}
