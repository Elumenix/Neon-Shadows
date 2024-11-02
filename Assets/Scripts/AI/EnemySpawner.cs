using Godot;
using System;

public enum EnemyType{
	BaseEnemy,
	Drone
}

public partial class EnemySpawner : Node2D
{
	[Export]private PackedScene _baseEnemy;
	[Export]private PackedScene _drone;
	[Export]public float spawnCooldown;
	[Export]public int maxSpawnNum;
	[Export]public EnemyType enemySpawnType = EnemyType.BaseEnemy;
	
	private int _currentSpawnNum;
	private Timer _spawnTimer;
	
	public override void _Ready(){
		_currentSpawnNum = 0;
		_spawnTimer = GetNode<Timer>("EnemySpawnTimer");
		_spawnTimer.WaitTime = spawnCooldown;
		_spawnTimer.Timeout += SpawnEnemy;
		_spawnTimer.Start();
	}
	
	public void SpawnEnemy(){
		_currentSpawnNum ++;
		Node node;
		GD.Print("sp");
		switch(enemySpawnType){
			case EnemyType.BaseEnemy:
				node = _baseEnemy.Instantiate();
				break;
			case EnemyType.Drone:
				node = _drone.Instantiate();
				break;
			default:
				node = _baseEnemy.Instantiate();
				break;
		}
		
		GetParent().AddChild(node);
		(node as Node2D).GlobalPosition = GlobalPosition;
		
		if(_currentSpawnNum >= maxSpawnNum){
			_spawnTimer.Stop();
		}
		
		
	}
}
