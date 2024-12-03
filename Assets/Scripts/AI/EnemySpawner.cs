using Godot;
using System;

public enum EnemyType
{
	BaseEnemy,
	Drone,
	Ooze
}

public partial class EnemySpawner : Node2D
{
	[Export] public int GateNum;
	[Export] private PackedScene _baseEnemy;
	[Export] private PackedScene _drone;
	[Export] private PackedScene _ooze;
	[Export] public float spawnCooldown;
	[Export] public int maxSpawnNum;
	[Export] public EnemyType enemySpawnType = EnemyType.BaseEnemy;

	[Export] public NodePath EnemiesContainerPath;

	private int _currentSpawnNum;
	private Node enemiesContainer;
	private Timer _spawnTimer;
	private Node _currentEnemy;
	private bool _startSpawn;
	private AudioStreamPlayer2D sfx;
	
	public override void _Ready()
	{
		_startSpawn = false;
		_currentSpawnNum = 0;
		_spawnTimer = GetNode<Timer>("EnemySpawnTimer");
		_spawnTimer.WaitTime = spawnCooldown;
		_spawnTimer.Timeout += SpawnEnemy;

		// Get the enemies container node
		enemiesContainer = GetNode(EnemiesContainerPath);
		sfx = GetNode<AudioStreamPlayer2D>("%SpawnSound");
	}

	public override void _Process(double delta){
		if(GameManager.Instance.currentGate == GateNum){
			GateNum = -1; // so this only trigger once
			GameManager.Instance.TotalEnemies += maxSpawnNum;
			_startSpawn = true;
		}
		
		if(_spawnTimer.TimeLeft == 0 && _startSpawn && (_currentEnemy == null || (_currentEnemy as BaseEnemyAI).isDead)){
			_spawnTimer.Start();
		}
	}

	public void SpawnEnemy()
	{
		if (_currentSpawnNum < maxSpawnNum){
			sfx.Play();
			_currentSpawnNum++;
			Node node;
			switch (enemySpawnType)
			{
				case EnemyType.BaseEnemy:
					node = _baseEnemy.Instantiate();
					break;
				case EnemyType.Drone:
					node = _drone.Instantiate();
					break;
				case EnemyType.Ooze:
					node = _ooze.Instantiate();
					break;
				default:
					node = _baseEnemy.Instantiate();
					break;
			}
			_currentEnemy = node;
			// Add the enemy to the enemies container instead of the spawner's parent
			enemiesContainer.AddChild(node);
			(node as Node2D).GlobalPosition = GlobalPosition;
		}

	}
}
