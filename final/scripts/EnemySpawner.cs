using Godot;
using System;

public partial class EnemySpawner : Node
{
	[Export] public PackedScene EnemyScene;
	[Export] public int MaxEnemies = 10;
	[Export] public float SpawnRadius = 1500f; 
	[Export] public Node2D Player;

	public override void _Process(double delta)
	{
		// Count how many enemies exist right now
		int current = GetTree().GetNodesInGroup("enemy").Count;

		if (current < MaxEnemies)
			SpawnEnemy();
	}

	private void SpawnEnemy()
	{
		if (EnemyScene == null || Player == null)
		{
			GD.PrintErr("Spawner missing references!");
			return;
		}

		Random rand = new Random();

		// random position around the player
		float angle = (float)(rand.NextDouble() * Math.PI * 2);
		float distance = (float)(rand.NextDouble() * SpawnRadius);

		Vector2 spawnPos = Player.Position + new Vector2(
			Mathf.Cos(angle),
			Mathf.Sin(angle)
		) * distance;

		var enemy = EnemyScene.Instantiate<Node2D>();
		enemy.AddToGroup("enemy");
		enemy.Position = spawnPos;

		GetParent().AddChild(enemy);
	}
}
