using Godot;
using System;

public partial class MainMenuStart : Node2D
{
	[Export]
	private Panel _OptionsPanel;

	private int masterBusIndex;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.SetCustomMouseCursor(GameManager.Instance.cursor);
		masterBusIndex = AudioServer.GetBusIndex("Master");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnOptionsPressed()
	{
		SoundFx.PlayButtonClicked();
		_OptionsPanel.Show();
	}
	
	public void OnVolumeChanged(float value)
	{
		// logarithmically converts value to be between -40db (essentially mute) to 0db (normal volume)
		float normalized = Mathf.Clamp(value / 100, 0.01f, 1);
		float dB = Mathf.Log(normalized) * 20;
		
		// Set Volume
		AudioServer.SetBusVolumeDb(masterBusIndex, dB);
	}

	public void HideOptions()
	{
		SoundFx.PlayButtonClicked();
		_OptionsPanel.Hide();
	}
}
