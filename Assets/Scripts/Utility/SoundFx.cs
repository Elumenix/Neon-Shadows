using Godot;
using System;

public partial class SoundFx : Node
{
	static AudioStreamPlayer buttonClick;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buttonClick = GetChild<AudioStreamPlayer>(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	internal static void PlayButtonClicked()
	{
		buttonClick.Play();
	}
}
