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
    public int DealDamage(Player player)
    {
        // Let the player know we've dealt damage
        player.Reload();
        // return damage value so enemy can take damage
        if(_damage < 0) { return 0; }
        return _damage;
    }
}
