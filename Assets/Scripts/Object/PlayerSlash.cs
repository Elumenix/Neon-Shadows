using Godot;
using System;

public partial class PlayerSlash : StaticBody2D
{
    private Timer _timer;
    public float AttackTime;

    private int _damage;
    public int Damage { get { return _damage; } set { _damage = value; } }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
    }
    private void _timeOut()
    {
        this.QueueFree();
    }

    
}
