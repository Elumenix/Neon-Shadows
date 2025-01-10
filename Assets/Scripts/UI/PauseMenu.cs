using Godot;
using System;
using System.Security.Cryptography;

public partial class PauseMenu : CanvasLayer
{
	[Export]
	private Panel _quitConfirmPanel;
	[Export]
	private Panel _restartConfirmPanel;
	[Export]
	private Panel _OptionsPanel;
	
	// Audio Channel
	private int masterBusIndex;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//don't remove these two line, it will break the game(don't ask me why)
		GameManager.Instance.PauseGame();
		GameManager.Instance.PauseGame();
		masterBusIndex = AudioServer.GetBusIndex("Master");
	}

	/// <summary>
	/// open the restart confirm panel
	/// </summary>
	public void OnRestartPressed() {
		GD.Print("Restart Pressed");
        SoundFx.PlayButtonClicked();
        _restartConfirmPanel.Show();
		_OptionsPanel.Hide();
		_quitConfirmPanel.Hide();
	}

	/// <summary>
	/// open the options panel
	/// </summary>
	public void OnOptionPressed() {
		GD.Print("Option Pressed");
        SoundFx.PlayButtonClicked();
        _OptionsPanel.Show();
		_quitConfirmPanel.Hide();
		_restartConfirmPanel.Hide();
	}

	/// <summary>
	/// open the quit panel
	/// </summary>
	public void OnQuitPressed()
	{
		GD.Print("Quit Pressed");
        SoundFx.PlayButtonClicked();
        _quitConfirmPanel.Show();
		_OptionsPanel.Hide();
		_restartConfirmPanel.Hide();
	}

	/// <summary>
	/// resume to the game
	/// </summary>
	public void OnResumePressed()
	{
		GD.Print("Resume Pressed");
        SoundFx.PlayButtonClicked();
        GameManager.Instance.PauseGame();
	}

	/// <summary>
	/// quit to the main menu 
	/// </summary>
	public void OnQuitYesPressed() {
        SoundFx.PlayButtonClicked();
        GameManager.Instance._Ready();
		(GameManager.Instance.player as Player).RespawnPlayer();
		GD.Print("quit yes pressed");
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
	}

	/// <summary>
	/// close the quit panel
	/// </summary>
	public void OnQuitNoPressed() {
		GD.Print("on quit no pressed");
        SoundFx.PlayButtonClicked();
        _quitConfirmPanel.Hide();
	}

	/// <summary>
	/// restart the current level
	/// </summary>
	public void OnRestartYesPressed()
	{
        SoundFx.PlayButtonClicked();
        GameManager.Instance._Ready();
		(GameManager.Instance.player as Player).RespawnPlayer();
		GD.Print("restart yes pressed");
		GetTree().ReloadCurrentScene();

	}

	/// <summary>
	/// close the restart panel
	/// </summary>
	public void OnRestartNoPressed()
	{
		GD.Print("on restart no pressed");
        SoundFx.PlayButtonClicked();
        _restartConfirmPanel.Hide();
	}

	public void OnGameOverRestart()
	{
        SoundFx.PlayButtonClicked();
        GameManager.Instance._Ready();
		(GameManager.Instance.player as Player).RespawnPlayer();
		GetTree().ReloadCurrentScene();

	}

	public void OnGameOverQuit() {
        SoundFx.PlayButtonClicked();
        GameManager.Instance._Ready();
		(GameManager.Instance.player as Player).RespawnPlayer();
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
	}

	public void OnVolumeChanged(float value)
	{
		// logarithmically converts value to be between -40db (essentially mute) to 0db (normal volume)
		float normalized = Mathf.Clamp(value / 100, 0.01f, 1);
		float dB = Mathf.Log(normalized) * 20;
		
		// Set Volume
		AudioServer.SetBusVolumeDb(masterBusIndex, dB);
	}
}
