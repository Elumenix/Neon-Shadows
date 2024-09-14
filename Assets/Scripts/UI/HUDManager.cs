using Godot;
using System;

public partial class HUDManager : Control
{
    public static HUDManager Instance { get; private set; }

    [Export] 
	public Label playerHPLabel = null;

    public override void _Ready()
    {
        Instance = this;
    }

    public void ChangePlayerHpLabel(int value) {
        playerHPLabel.Text = value.ToString();
    }
}
