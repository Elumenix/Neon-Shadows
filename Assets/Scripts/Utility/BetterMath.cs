using Godot;
using System;
using System.Threading;

public partial class BetterMath
{
	public float DistanceBetweenTwoVector(Vector2 v1, Vector2 v2) {
		return Mathf.Sqrt(Mathf.Pow(v2.X-v1.X,2) + Mathf.Pow(v2.Y - v1.Y, 2));
	}

	public float VectorToAngle(Vector2 direction) { 
		return Mathf.Atan2(direction.Y,direction.X);
	}

	public Vector2 AngleToVector(float angle) {
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}
	
}
