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

    public void OnLevelOnePressed() 
    {
        GD.Print("level 1");
        _levelOneDescriptionPanel.Show();
        _levelTwoDescriptionPanel.Hide();
        _quitConfirmPanel.Hide();
    }

    public void OnLevelTwoPressed() 
    {

        GD.Print("level 2");
        _levelOneDescriptionPanel.Hide();
        _levelTwoDescriptionPanel.Show();
        _quitConfirmPanel.Hide();
    }

    public void OnQuitPressed()
    {
        GD.Print("quit");
        _levelOneDescriptionPanel.Hide();
        _levelTwoDescriptionPanel.Hide();
        _quitConfirmPanel.Show();

    }

    public void OnLevelOneStartPressed() 
    {
        GD.Print("level 1 start");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Level1.tscn");
    }

    public void OnLevelTwoStartPressed()
    {
        GD.Print("level 2 start");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/TileMapTest.tscn");
    }
}
