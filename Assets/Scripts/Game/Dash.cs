using Godot;
using System;

public partial class Dash : Node2D
{
	private const double _DashDelay = 0.4f;
	private Timer _timer;
	private Timer _ghostTimer;
	private bool _canDash = true;
	public bool CanDash { get { return _canDash; } }
    public bool IsDashing {  get { return !_timer.IsStopped(); } }

	private PackedScene _ghostScene = GD.Load<PackedScene>("res://Assets/Entities/DashGhost.tscn");
	private Sprite2D _sprite;
	private Vector2 _facing;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Attach Timer
		_timer = GetNode<Timer>("Timer");
		_ghostTimer = GetNode<Timer>("GhostTimer");
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public void StartDash(Vector2 facing, float duration)
	{
		_facing = facing.Normalized();
		_timer.WaitTime = duration;
		_timer.Start();
		
		_ghostTimer.Start();
		InstanceGhost();

	}

	private void InstanceGhost()
	{
		var ghost = (Sprite2D)_ghostScene.Instantiate();
		ghost.GlobalPosition = GlobalPosition;
        //GD.Print($"GlobalPosition {GlobalPosition}");
        //GD.Print($"Ghost GlobalPosition {ghost.GlobalPosition}");
		ghost.Texture = _sprite.Texture;
        ghost.Vframes = _sprite.Vframes;
        ghost.Hframes = _sprite.Hframes;
        ghost.FlipH = _sprite.FlipH;
        ghost.FlipV = _sprite.FlipV;

        // logic to set up frame correctly
        if (_facing.X > 0 && _facing.Y == 0) { ghost.Frame = 42; }
        else if (_facing.X < 0 && _facing.Y == 0) { ghost.Frame = 46; }
        else if (_facing.X == 0 && _facing.Y < 0) { ghost.Frame = 40; }
        else if (_facing.X == 0 && _facing.Y > 0) { ghost.Frame = 44; }
        else if (_facing.X > 0 && _facing.Y < 0) { ghost.Frame = 41; }
        else if (_facing.X > 0 && _facing.Y > 0) { ghost.Frame = 43; }
        else if (_facing.X < 0 && _facing.Y < 0) { ghost.Frame = 47; }
        else if (_facing.X < 0 && _facing.Y > 0) { ghost.Frame = 45; }
        else { ghost.Frame = 44; }
        GetParent().GetParent().AddChild(ghost);
    }

	public void GhostTimerOut()
	{
		InstanceGhost();
	}
    public async void EndDash()
	{
		_ghostTimer.Stop();
		_canDash = false;
		await ToSignal(GetTree().CreateTimer(_DashDelay), "timeout");
		_canDash = true;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
