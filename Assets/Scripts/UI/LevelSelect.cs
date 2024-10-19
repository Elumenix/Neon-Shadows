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
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Level1.tscn");
    }

    /// <summary>
    /// start level 2
    /// </summary>
    public void OnLevelTwoStartPressed()
    {
        GD.Print("level 2 start");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/TileMapTest.tscn");
    }
}
