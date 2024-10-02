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
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
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
