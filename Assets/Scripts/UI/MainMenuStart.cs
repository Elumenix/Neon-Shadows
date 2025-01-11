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
		
		// This whole set of instructions is needed so that the visual volume level matches the current volume
		// Logarithmically converting db to a volume value 
		float volumeDb = AudioServer.GetBusVolumeDb(masterBusIndex);
		float volume = Mathf.DbToLinear(volumeDb) * 100;
		
		GetNode<HSlider>("%VolumeSlider").Value = volume;
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
		// Decibels will logarithmically go from -40 to 0
		float dB = Mathf.LinearToDb(value / 100);
		
		// Set Volume
		AudioServer.SetBusVolumeDb(masterBusIndex, dB);
	}

	public void HideOptions()
	{
		SoundFx.PlayButtonClicked();
		_OptionsPanel.Hide();
	}
}
