using Godot;
using System;

public partial class LevelSelect : CanvasLayer
{
	[Export]
	private Panel _tutorialDescriptionPanel;
	[Export]
	private Panel _levelOneDescriptionPanel;
	[Export]
	private Panel _arenaDescriptionPanel;

	[Export]
	private Panel _quitConfirmPanel;

	public override void _Ready()
	{
		_quitConfirmPanel.Hide();
		_arenaDescriptionPanel.Hide();
		_levelOneDescriptionPanel.Hide();
		_tutorialDescriptionPanel.Hide();
	}
	
	/// <summary>
	/// open the tutorial panel
	/// </summary>
	public void OnTutorialPressed()
	{
		GD.Print("tutorial");
        SoundFx.PlayButtonClicked();
        _tutorialDescriptionPanel.Show();
		_levelOneDescriptionPanel.Hide();
		_arenaDescriptionPanel.Hide();
		_quitConfirmPanel.Hide();
	}
	
	/// <summary>
	/// open the level 1 panel
	/// </summary>
	public void OnLevelOnePressed() 
	{
		GD.Print("level 1");
        SoundFx.PlayButtonClicked();
        _tutorialDescriptionPanel.Hide();
		_levelOneDescriptionPanel.Show();
		_arenaDescriptionPanel.Hide();
		_quitConfirmPanel.Hide();
	}

	/// <summary>
	/// open the level 2 panel
	/// </summary>
	public void OnLevelTwoPressed() 
	{
		GD.Print("level 2");
        SoundFx.PlayButtonClicked();
        _tutorialDescriptionPanel.Hide();
		_levelOneDescriptionPanel.Hide();
		_arenaDescriptionPanel.Show();
		_quitConfirmPanel.Hide();
	}

	/// <summary>
	/// open the quit panel
	/// </summary>
	public void OnQuitPressed()
	{
		GD.Print("quit");
        SoundFx.PlayButtonClicked();
        _tutorialDescriptionPanel.Hide();
		_levelOneDescriptionPanel.Hide();
		_arenaDescriptionPanel.Hide();
		_quitConfirmPanel.Show();
	}

	/// <summary>
	/// start the tutorial
	/// </summary>
	public void OnTutorialStartPressed()
	{
        SoundFx.PlayButtonClicked();
        GD.Print("tutorial start");
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Tutorial.tscn");
	}
	
	/// <summary>
	/// start level 1
	/// </summary>
	public void OnLevelOneStartPressed() 
	{
		GD.Print("level 1 start");
        SoundFx.PlayButtonClicked();
        GetTree().ChangeSceneToFile("res://Assets/Scenes/TileMapTest.tscn");
	}

	/// <summary>
	/// start level 2
	/// </summary>
	public void OnLevelTwoStartPressed()
	{
		GD.Print("level 2 start");
        SoundFx.PlayButtonClicked();
        GetTree().ChangeSceneToFile("res://Assets/Scenes/ArenaScene.tscn");
	}

	/// <summary>
	/// Back to main menu
	/// </summary>
	public void OnConfirmQuit()
	{
		GD.Print("Quitting");
        SoundFx.PlayButtonClicked();
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
	}

	/// <summary>
	/// Close quit menu
	/// </summary>
	public void OnRejectQuit()
	{
		GD.Print("Refuse to quit");
        SoundFx.PlayButtonClicked();
        _quitConfirmPanel.Hide();
	}
}
