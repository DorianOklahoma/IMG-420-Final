using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class EnemySpawner : Node
{
	[Export] public PackedScene EnemyScene;
	[Export] public TileMapLayer Tilemap;
	[Export] public NodePath PlayerPath;
	[Export] public int EnemiesPerChunk = 3;
	[Export] public float SpawnCheckRadius = 800f;
	[Export] public float MinDistanceFromPlayer = 200f;
	[Export] public int MaxRetries = 50;
	[Export] public float SpawnCheckInterval = 2f;

	private Node2D _player;
	private HashSet<Vector2I> _spawnedChunks = new HashSet<Vector2I>();
	private List<Enemy> _activeEnemies = new List<Enemy>();
	private Timer _spawnCheckTimer;
	private bool _mapReady = false;
	private bool _gameStarted = false;

	public override async void _Ready()
	{
		if (PlayerPath != null && !string.IsNullOrEmpty(PlayerPath))
			_player = GetNode<Node2D>(PlayerPath);

		await WaitForMapGeneration();
		SetupSpawnTimer();
	}

	private async Task WaitForMapGeneration()
	{
		int maxAttempts = 10;
		int attempt = 0;

		while (attempt < maxAttempts)
		{
			await Task.Delay(300);
			Vector2I usedSize = Tilemap.GetUsedRect().Size;

			if (usedSize.X > 0 && usedSize.Y > 0)
			{
				_mapReady = true;
				return;
			}

			attempt++;
		}
	}

	private void SetupSpawnTimer()
	{
		_spawnCheckTimer = new Timer();
		_spawnCheckTimer.WaitTime = SpawnCheckInterval;
		_spawnCheckTimer.Timeout += OnSpawnCheckTimeout;
		AddChild(_spawnCheckTimer);
		_spawnCheckTimer.Start();
	}

	private void OnSpawnCheckTimeout()
	{
		if (!_gameStarted) return;
		if (_player != null)
			SpawnEnemiesNearPlayer();
	}

	public void StartSpawner()
	{
		_gameStarted = true;

		// Clear old enemies and chunks
		_spawnedChunks.Clear();
		foreach (var enemy in _activeEnemies)
			enemy.QueueFree();
		_activeEnemies.Clear();
	}

	private void SpawnEnemiesNearPlayer()
	{
		if (!_mapReady || _player == null) return;

		Vector2 playerPos = _player.GlobalPosition;
		Vector2I playerTile = Tilemap.LocalToMap(Tilemap.ToLocal(playerPos));

		if (_spawnedChunks.Contains(playerTile)) return;

		_spawnedChunks.Add(playerTile);

		for (int i = 0; i < EnemiesPerChunk; i++)
		{
			int retries = 0;
			while (retries < MaxRetries)
			{
				float offsetX = GD.Randf() * SpawnCheckRadius * 2 - SpawnCheckRadius;
				float offsetY = GD.Randf() * SpawnCheckRadius * 2 - SpawnCheckRadius;
				Vector2 spawnPos = playerPos + new Vector2(offsetX, offsetY);

				if (spawnPos.DistanceTo(playerPos) < MinDistanceFromPlayer)
				{
					retries++;
					continue;
				}

				Enemy enemy = EnemyScene.Instantiate<Enemy>();
				enemy.GlobalPosition = spawnPos;
				AddChild(enemy);
				_activeEnemies.Add(enemy);
				break;
			}
		}
	}
}
