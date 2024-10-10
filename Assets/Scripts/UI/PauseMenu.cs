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


	public void OnRestartPressed() {
		GD.Print("Restart Pressed");
        _restartConfirmPanel.Show();
        _OptionsPanel.Hide();
        _quitConfirmPanel.Hide();
    }

	public void OnOptionPressed() {
        GD.Print("Option Pressed");
        _OptionsPanel.Show();
        _quitConfirmPanel.Hide();
        _restartConfirmPanel.Hide();
    }

    public void OnQuitPressed()
    {
        GD.Print("Quit Pressed");
		_quitConfirmPanel.Show();
        _OptionsPanel.Hide();
        _restartConfirmPanel.Hide();
    }

    public void OnResumePressed()
    {
        GD.Print("Resume Pressed");
        GameManager.Instance.PauseGame();
    }

	public void OnQuitYesPressed() {

		GD.Print("quit yes pressed");
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Main Menu.tscn");
    }

	public void OnQuitNoPressed() {
		GD.Print("on quit no pressed");
        _quitConfirmPanel.Hide();
    }

    public void OnRestartYesPressed()
    {
        GD.Print("restart yes pressed");
        GetTree().ReloadCurrentScene();

    }

    public void OnRestartNoPressed()
    {
        GD.Print("on restart no pressed");
        _restartConfirmPanel.Hide();
    }
}
