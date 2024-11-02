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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //don't remove these two line, it will break the game(don't ask me why)
        GameManager.Instance.PauseGame();
        GameManager.Instance.PauseGame();
    }

    /// <summary>
    /// open the restart confirm panel
    /// </summary>
	public void OnRestartPressed() {
		GD.Print("Restart Pressed");
        _restartConfirmPanel.Show();
        _OptionsPanel.Hide();
        _quitConfirmPanel.Hide();
    }

    /// <summary>
    /// open the options panel
    /// </summary>
	public void OnOptionPressed() {
        GD.Print("Option Pressed");
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
        GameManager.Instance.PauseGame();
    }

    /// <summary>
    /// quit to the main menu 
    /// </summary>
	public void OnQuitYesPressed() {

		GD.Print("quit yes pressed");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
    }

    /// <summary>
    /// close the quit panel
    /// </summary>
	public void OnQuitNoPressed() {
		GD.Print("on quit no pressed");
        _quitConfirmPanel.Hide();
    }

    /// <summary>
    /// restart the current level
    /// </summary>
    public void OnRestartYesPressed()
    {
        GD.Print("restart yes pressed");
        GetTree().ReloadCurrentScene();

    }

    /// <summary>
    /// close the restart panel
    /// </summary>
    public void OnRestartNoPressed()
    {
        GD.Print("on restart no pressed");
        _restartConfirmPanel.Hide();
    }
}
