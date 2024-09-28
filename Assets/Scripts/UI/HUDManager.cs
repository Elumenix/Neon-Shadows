using Godot;
using System;
using System.Collections.Generic;

public partial class HUDManager : Control
{
    public static HUDManager Instance { get; private set; }

    [Export]
    public Godot.Collections.Array<TextureRect> HealthIconList;
    private Node2D player;
    public override async void _Ready()
    {
         Instance = this;

        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        player = GetTree().GetNodesInGroup("Player")[0] as Node2D;
    }

    public void IncreasePlayerHp() {
        foreach (TextureRect image in HealthIconList) {
            if (!image.Visible) { 
                image.Visible = true;
                return;
            }
        }
    }
    public void DecreasePlayerHp()
    {
        HealthIconList[(player as Player).GetPlayerHealth()].Visible = false;
    }
}
