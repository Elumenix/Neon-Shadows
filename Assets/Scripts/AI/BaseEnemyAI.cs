using Godot;
using System;

public partial class BaseEnemyAI : CharacterBody2D
{
	[ExportCategory("Health")]
	[Export] public int MaxHealth = 100;
	private int currentHealth;
	[Export] private float flashTime = 0.1f;

	[ExportCategory("Movement")]
	[Export] protected float _speed;
	[Export] protected NavigationAgent2D _navigationAgent;
	protected bool _usePathFinding = true;
	protected bool _shouldMove = true;

	protected float _iFrames = 0.3f;

	protected Node2D _player;

	protected float _detectionRange;

	public override async void _Ready()
	{
		currentHealth = MaxHealth;

		// Small delay for initialization
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

		// Get player node (assuming player is in the "Player" group)
		_player = GetTree().GetNodesInGroup("Player")[0] as Node2D;

		// Connect the Area2D signal for collision detection
		GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
		UpdateNavigationTarget();
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("EnemySprite");

		//create the shader for enemy if there are none
		if (sprite.Material != null && sprite.Material is ShaderMaterial)
		{
			//duplicate the material so each enemy has its own instance
			ShaderMaterial shaderMaterial = (ShaderMaterial)sprite.Material.Duplicate();
			sprite.Material = shaderMaterial;
		}

		_detectionRange = 300.0f;
	}

	public override void _PhysicsProcess(double delta)
	{
		//update the target position and exit if there are none
		UpdateNavigationTarget();
		Vector2 nextPathPosition = nextPathPosition = _navigationAgent.GetNextPathPosition();
		if (_navigationAgent.DistanceToTarget() < _detectionRange)
		{
			_shouldMove = true;
		}
		else
		{
			_shouldMove = false;
		}

		if (_navigationAgent.IsNavigationFinished())
		{
			nextPathPosition = GlobalPosition;
		}

		MoveTowardsTarget(nextPathPosition, delta);
	}


	public override void _Process(double delta)
	{
		if(_iFrames > 0)
		{
			_iFrames -= (float)delta;
			if(_iFrames < 0)
			{
				_iFrames = 0;
			}
		}
	}

	/// <summary>
	/// update the target position of the enemy pathfinding
	/// </summary>
	private void UpdateNavigationTarget()
	{
		if (_player != null)
		{
			_navigationAgent.TargetPosition = _player.GlobalPosition; 
		}
	}

	/// <summary>
	/// Move the enemy towards the next target position
	/// </summary>
	/// <param name="targetPosition">the position enemy is targeting</param>
	/// <param name="delta">the delta time</param>
	private void MoveTowardsTarget(Vector2 targetPosition, double delta)
	{
		//only use base enemy pathfinding if usePathFinding and shouldMove are true
		if (_usePathFinding && _shouldMove) { 
			//create the direction vector and the collision for enemy
			Vector2 direction = (targetPosition - GlobalPosition).Normalized();
			//Position += direction * (float)(_speed * delta);
			var collision = MoveAndCollide(direction * (float)(_speed * delta));
			if(collision != null)
			{
				// Enemy collided with a slash
				if (collision.GetCollider() is PlayerSlash)
				{
					// take damage from the slash
					
					PlayerSlash temp = (PlayerSlash)collision.GetCollider();
					if(_iFrames <= 0)
					{
						this.TakeDamage(temp.DealDamage());
					}
				}
				else if(collision.GetCollider() is Player)
				{
					// Collided with the player
					HandlePlayerCollision();
				}
			}
			
		}
	}

	/// <summary>
	/// Manages enemy's health when taking damage
	/// </summary>
	/// <param name="damageAmount"></param>
	public void TakeDamage(int damageAmount)
	{
		if (_iFrames <=0)
		{
			FlashOnDamage();
			currentHealth -= damageAmount;
			_iFrames = 0.3f;
		}
		

		if (currentHealth <= 0)
		{
			currentHealth = 0;
			HandleDeath();
		}
	}

	/// <summary>
	/// Handle enemy death
	/// </summary>
	private void HandleDeath()
	{
		QueueFree();
	}

	// This function will be called when a collision with the player happens
	public void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			//HandlePlayerCollision();
		}
	}

	/// <summary>
	/// handles player collision, create camera shake and deal damage to player
	/// </summary>
	public void HandlePlayerCollision()
	{
		Player temp = (Player)_player;
		// Moving the screen shake before damage means the screen shakes on the last damage
		if (!temp.IsDead && !temp.IsInvulenerable)
		{
			Camera.Instance.StartShakeCamera(0.1f, 25);
		}
		temp.takeDamage(1);
		HUDManager.Instance.DecreasePlayerHp();
		
	}

	/// <summary>
	/// create the flash animation when enemy is damaged
	/// </summary>
	public void FlashOnDamage() {
		GetNode<AnimationPlayer>("EnemySprite/FlashAnimation").Play("Flash");
	}
}
