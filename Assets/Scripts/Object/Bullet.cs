using Godot;
using System;
using System.Runtime.InteropServices;

public partial class Bullet : Sprite2D
{
	private Vector2 _direction;

	[Export]
	private float _speed = 200;

	[Export]
	private double _lifeTime = 3;

	public Node2D player;


    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        _direction = new BetterMath().AngleToVector(Rotation);
        GlobalTranslate(_direction * _speed * (float)delta);

		_lifeTime -= delta;

		if (_lifeTime < 0) { 
			QueueFree();
		}
	}

    public void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            HandlePlayerCollision();
            QueueFree();
        }
        if (!body.IsInGroup("Enemy"))
        {
            QueueFree();
        }
    }

    public void HandlePlayerCollision()
    {
        Player temp = (Player)player;
        if (!temp.IsDead)
        {
            Camera.Instance.StartShakeCamera(0.1f, 25);
        }
        temp.takeDamage(1);
        HUDManager.Instance.DecreasePlayerHp();

    }
}
