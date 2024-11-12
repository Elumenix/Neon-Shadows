using Godot;
using System;

public enum EnemyType
{
    BaseEnemy,
    Drone
}

public partial class EnemySpawner : Node2D
{
    [Export] private PackedScene _baseEnemy;
    [Export] private PackedScene _drone;
    [Export] public float spawnCooldown;
    [Export] public int maxSpawnNum;
    [Export] public EnemyType enemySpawnType = EnemyType.BaseEnemy;

    [Export] public NodePath EnemiesContainerPath;

    private int _currentSpawnNum;
    private Timer _spawnTimer;
    private Node enemiesContainer;

    public override void _Ready()
    {
        _currentSpawnNum = 0;
        _spawnTimer = GetNode<Timer>("EnemySpawnTimer");
        _spawnTimer.WaitTime = spawnCooldown;
        _spawnTimer.Timeout += SpawnEnemy;
        _spawnTimer.Start();

        // Get the enemies container node
        enemiesContainer = GetNode(EnemiesContainerPath);
    }

    public void SpawnEnemy()
    {
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
            default:
                node = _baseEnemy.Instantiate();
                break;
        }

        // Add the enemy to the enemies container instead of the spawner's parent
        enemiesContainer.AddChild(node);
        (node as Node2D).GlobalPosition = GlobalPosition;
        GameManager.Instance.TotalEnemies++;

        if (_currentSpawnNum >= maxSpawnNum)
        {
            _spawnTimer.Stop();
        }
    }
}
