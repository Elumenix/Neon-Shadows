using Godot;
using System;
/// <summary>
/// This class is very minimal. It's main methods are DealDamage and _timeOut. It acts as an intermediary between the player and enemy for dealing damage
/// </summary>
public partial class PlayerSlash : StaticBody2D
{
    private Timer _timer;
    public float AttackTime;

    private int _damage;
    public int Damage { get { return _damage; } set { _damage = value; } }

    private Player _player = null;
    public Player Player { set { _player = value; } }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _timer = GetNode<Timer>("Timer");
        GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    public override void _PhysicsProcess(double delta)
    {
    }
    /// <summary>
    /// Allows the Slash to delete itself after it's timer goes off
    /// </summary>
    private void _timeOut()
    {
        this.QueueFree();
    }
    /// <summary>
    /// Helps share information between Player & Enemy. Should be called in enemies TakeDamage method
    /// </summary>
    /// <param name="player">Reference to the Player so they can reload</param>
    /// <returns>The amount of damage this attack deals</returns>
    public int DealDamage()
    {
        // Let the player know we've dealt damage
        _player.Reload();
        // return damage value so enemy can take damage
        if(_damage < 0) { return 0; }
        return _damage;
    }

    public void OnBodyEntered(Node2D body) {
        if (body.IsInGroup("Enemy"))
        {
            if (body is BaseEnemyAI enemy)
            {
                enemy.TakeDamage(DealDamage());
                Vector2 knockback = Vector2.Right.Rotated(RotationDegrees);
                knockback *= 50.0f;
                enemy.ApplyKnockback(knockback);
            }
        }
        else if(body is Barrel barrel)
        {
            barrel._onHit();
        }
        
    }
    /// <summary>
	/// Returns a normalized vector based off the rotation in degrees
	/// </summary>
	/// <param name="degrees">Angle of rotation in degrees</param>
	/// <returns></returns>
	private Vector2 _vectorFromRotation(float degrees)
    {
        // Use a 45 degree offset so it seems more accurate
        if (degrees >= 315.0f || degrees < 45.0f)
        {
            return Vector2.Right;
        }
        else if (degrees >= 45.0f && degrees < 135.0f)
        {
            return Vector2.Down;
        }
        else if (degrees >= 135.0f && degrees < 225.0f)
        {
            return Vector2.Left;
        }
        else if (degrees >= 225.0f && degrees < 315.0f)
        {
            return Vector2.Up;
        }
        return Vector2.Zero;
    }
}
