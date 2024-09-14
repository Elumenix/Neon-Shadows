using Godot;
using System;
using System.Collections.Generic;

public partial class HUDManager : Control
{
    public static HUDManager Instance { get; private set; }

    [Export]
    public Godot.Collections.Array<TextureRect> HealthIconList;

    public override void _Ready()
    {
        Instance = this;
    }

    public void IncreasePlayerHp() {
        foreach (TextureRect image in HealthIconList) {
            if (!image.Visible) { 
                image.Visible = true;
                return;
            }
        }
    }
    public void DescreasePlayerHp()
    {
        HealthIconList[HealthIconList.Count - 1].Visible = false;
    }
}
