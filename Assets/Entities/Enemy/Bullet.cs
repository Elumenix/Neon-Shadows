using Godot;
using System;
using System.Runtime.InteropServices;

public partial class Bullet : Sprite2D
{
	private Vector2 _direction;

	[Export]
	private float _speed;

	[Export]
	private double _lifeTime = 3;
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
}
