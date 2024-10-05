using Godot;
using System;

public partial class PlayerSlash : StaticBody2D
{
    private Timer _timer;
    public float AttackTime;
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

    //public void _on_area_entered(Area2D collision)
    //{
    //	if(collision.Owner is BaseEnemyAI)
    //	{
    //		BaseEnemyAI temp = (BaseEnemyAI)collision.Owner;
    //		temp.TakeDamage(50);
    //	}
    //}
}
