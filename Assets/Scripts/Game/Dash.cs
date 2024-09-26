using Godot;
using System;

public partial class Dash : Node2D
{
	private const double _DashDelay = 0.4f;
	private Timer _timer;
	private bool _canDash = true;
	public bool CanDash { get { return _canDash; } }
    public bool IsDashing {  get { return !_timer.IsStopped(); } }

	private PackedScene _ghostScene = GD.Load<PackedScene>("res://Assets/Entities/DashGhost.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Attach Timer
		_timer = GetNode<Timer>("Timer");
	}

	public void StartDash(float duration)
	{
		_timer.WaitTime = duration;
		_timer.Start();

		InstanceGhost();

	}

	private void InstanceGhost()
	{
		var ghost = _ghostScene.Instantiate();
		GetParent().GetParent().AddChild(ghost);
	}

	
	public async void EndDash()
	{
		_canDash = false;
		await ToSignal(GetTree().CreateTimer(_DashDelay), "timeout");
		_canDash = true;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
