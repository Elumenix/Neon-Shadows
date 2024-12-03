using Godot;
using System;

public partial class GameManager : Node
{

	private CanvasLayer _pauseMenu;
	public Node2D player;
	public Resource cursor = ResourceLoader.Load("res://Assets/Sprites/Menu/cursor.png");
    public Resource reticle = ResourceLoader.Load("res://Assets/Sprites/Menu/cursor-reticle.png");
    public AudioStream bulletSoundEffect = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/laser-gun-81720.mp3");


    //game states
    public bool gamePaused;
	public int currentLevel;

	public int TotalEnemies { get; set; }
	private int _defeatedEnemies;
	public int DefeatedEnemies { get { return _defeatedEnemies; } }
	public int currentGate;

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
		currentGate = 1;
		_defeatedEnemies= 0;
		TotalEnemies = 0;
		Input.SetCustomMouseCursor(reticle);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause")) {
            SoundFx.PlayButtonClicked();
            PauseGame();
		}
	}

	public void EnemyDefeated()
	{
		_defeatedEnemies++;
		GD.Print("Enemy defeated. Total defeated: ", _defeatedEnemies);
		CheckIfAllEnemiesDefeated();
	}

	private void CheckIfAllEnemiesDefeated()
	{
		GD.Print("_defeatedEnemies: " + _defeatedEnemies + "  TotalEnemies: " + TotalEnemies);
		if (_defeatedEnemies >= TotalEnemies)
		{
			GD.Print("All enemies defeated!");

            foreach (var gate in GetTree().GetNodesInGroup("Gate")){

				if ((gate as Gate).GateNum == currentGate)
                {
                    (gate as Gate).OpenGate();
                    TotalEnemies = 0;
                    _defeatedEnemies = 0;
					return;
                }
			}

            currentGate++;

            foreach (var gate in GetTree().GetNodesInGroup("Gate"))
            {
                if ((gate as Gate).GateNum == currentGate)
                {
                    (gate as Gate).StartGate();
                    return;
                }
            }
        }
	}


	/// <summary>
	/// pause the game and open the pause menu
	/// </summary>
	public void PauseGame() {
        //get player and pause menu panel
        _pauseMenu = GetTree().GetNodesInGroup("PauseMenu")[0] as CanvasLayer;

		if (_pauseMenu == null) return;
		//toggle pause menu and disable player controls
		_pauseMenu.Visible = !_pauseMenu.Visible;
		gamePaused = _pauseMenu.Visible;

		//pause or active the game
		if (_pauseMenu.Visible)
		{
			Engine.TimeScale = 0;
			Input.SetCustomMouseCursor(cursor);
        }
        else
		{
			Engine.TimeScale = 1;
            Input.SetCustomMouseCursor(reticle);
        }
    }
}
