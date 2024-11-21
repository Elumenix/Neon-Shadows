using Godot;
using System;

public partial class Gate : Node
{
	[Export]
	public int GateNum;
	
	[Export]
	public NodePath EnemiesContainerPath;

	[Export]
	public int InitEnemiesNum;

    public override void _Ready() {
		if (GameManager.Instance.currentGate == GateNum) {
            StartGate();
		}

    }


    public void OpenGate()
	{
		GD.Print("Gate opening");
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
		QueueFree();
	}

	public void StartGate() {
		GameManager.Instance.TotalEnemies += InitEnemiesNum;
	}
}
