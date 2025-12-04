using Godot;
using System;
using System.Collections.Generic;

public partial class ItemSpawner : Node2D
{
	[Export] private NodePath PlayerPath;
	[Export] private PackedScene LifePickupScene;
	[Export] private PackedScene AbilityPickupScene;

	[Export] private float SpawnRadius = 300f;      // distance around player to spawn
	[Export] private float DespawnRadius = 500f;    // distance beyond which items despawn
	[Export] private float SpawnCooldown = 2f;      // seconds between spawns

	private Player _player;
	private float _spawnTimer = 0f;
	private List<Node2D> _spawnedItems = new List<Node2D>();

	public override void _Ready()
	{
		if (PlayerPath != null)
			_player = GetNode<Player>(PlayerPath);
	}

	public override void _Process(double delta)
	{
		if (_player == null) return;

		_spawnTimer -= (float)delta;

		// Despawn items far from player or already freed
		for (int i = _spawnedItems.Count - 1; i >= 0; i--)
		{
			Node2D item = _spawnedItems[i];

			if (item == null || !IsInstanceValid(item) || item.GetParent() == null)
			{
				_spawnedItems.RemoveAt(i);
				continue;
			}

			if (item.GlobalPosition.DistanceTo(_player.GlobalPosition) > DespawnRadius)
			{
				_spawnedItems.RemoveAt(i);
				item.QueueFree();
			}
		}

		// Spawn new item if cooldown finished
		if (_spawnTimer <= 0f)
		{
			_spawnTimer = SpawnCooldown;

			Vector2 randomOffset = new Vector2(
				(float)GD.RandRange(-SpawnRadius, SpawnRadius),
				(float)GD.RandRange(-SpawnRadius, SpawnRadius)
			);

			Vector2 spawnPos = _player.GlobalPosition + randomOffset;

			// Randomly pick LifePickup (70%) or AbilityPickup (30%)
			PackedScene spawnScene = (GD.Randf() < 0.7f) ? LifePickupScene : AbilityPickupScene;
			if (spawnScene != null)
			{
				Node2D newItem = spawnScene.Instantiate<Node2D>();
				newItem.GlobalPosition = spawnPos;
				AddChild(newItem);
				_spawnedItems.Add(newItem);
			}
		}
	}
}
