using Godot;
using System;
using System.Threading;

public partial class BetterMath
{
	/// <summary>
	/// calculate the discante between two vector
	/// </summary>
	/// <param name="v1">The first vector</param>
	/// <param name="v2">the second vector</param>
	/// <returns>the distance between the two vector</returns>
	public float DistanceBetweenTwoVector(Vector2 v1, Vector2 v2) {
		return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(v2.X-v1.X,2) + Mathf.Pow(v2.Y - v1.Y, 2)));
	}

	/// <summary>
	/// convert vector to angle
	/// </summary>
	/// <param name="direction">the direction where the vector is pointing</param>
	/// <returns>the angle</returns>
	public float VectorToAngle(Vector2 direction) { 
		return Mathf.Atan2(direction.Y,direction.X);
	}
	
	/// <summary>
	/// convert angle to vector
	/// </summary>
	/// <param name="angle">the angle</param>
	/// <returns>the direction vector of that angle</returns>
	public Vector2 AngleToVector(float angle) {
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	/// <summary>
	/// get random number including negative numbers
	/// </summary>
	/// <param name="offset"> the positive and negetive range</param>
	/// <returns>the random number</returns>
	public double GetRandomWithNegative(double offset) {
		return (new Random().NextDouble() * (offset * 2) - offset);
    }

	/// <summary>
	/// easing equation for enemy movement
	/// </summary>
	/// <param name="x">the current process</param>
	/// <returns>the result</returns>
    public double EasingCalculation(double x) {
		return Mathf.Sin(Mathf.Pi * x);
    }
}

