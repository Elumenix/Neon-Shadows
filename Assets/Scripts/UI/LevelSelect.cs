using Godot;
using System;

public partial class LevelSelect : CanvasLayer
{
	[Export]
	private Panel _levelOneDescriptionPanel;
    [Export]
    private Panel _levelTwoDescriptionPanel;

    [Export]
    private Panel _quitConfirmPanel;

    public override void _Ready()
    {
        _quitConfirmPanel.Hide();
        _levelTwoDescriptionPanel.Hide();
        _levelOneDescriptionPanel.Show();
    }
    
    /// <summary>
    /// open the level 1 panel
    /// </summary>
    public void OnLevelOnePressed() 
    {
        GD.Print("level 1");
        _levelOneDescriptionPanel.Show();
        _levelTwoDescriptionPanel.Hide();
        _quitConfirmPanel.Hide();
    }

    /// <summary>
    /// open the level 2 panel
    /// </summary>
    public void OnLevelTwoPressed() 
    {

        GD.Print("level 2");
        _levelOneDescriptionPanel.Hide();
        _levelTwoDescriptionPanel.Show();
        _quitConfirmPanel.Hide();
    }

    /// <summary>
    /// open the quit panel
    /// </summary>
    public void OnQuitPressed()
    {
        GD.Print("quit");
        _levelOneDescriptionPanel.Hide();
        _levelTwoDescriptionPanel.Hide();
        _quitConfirmPanel.Show();
    }

    /// <summary>
    /// start level 1
    /// </summary>
    public void OnLevelOneStartPressed() 
    {
        GD.Print("level 1 start");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/TileMapTest.tscn");
    }

    /// <summary>
    /// start level 2
    /// </summary>
    public void OnLevelTwoStartPressed()
    {
        GD.Print("level 2 start");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/TileMapTest.tscn");
    }

    /// <summary>
    /// Back to main menu
    /// </summary>
    public void OnConfirmQuit()
    {
        GD.Print("Quitting");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
    }

    /// <summary>
    /// Close quit menu
    /// </summary>
    public void OnRejectQuit()
    {
        GD.Print("Refuse to quit");
        _quitConfirmPanel.Hide();
    }
}
