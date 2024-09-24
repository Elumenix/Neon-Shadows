using Godot;
using System;

public partial class PlayButton : Button
{
    public void _on_pressed()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/GameTest.tscn");
    }
}
