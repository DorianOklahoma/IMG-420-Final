using Godot;
using System;

public partial class LifePickup : Area2D
{
	[Export] public int LifeAmount = 1;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		Monitoring = true;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			// Only add life if current lives < max
			if (player.CurrentLives < player.MaxLives)
			{
				player.AddLife(LifeAmount);
				GD.Print("[LifePickup] Life added. Current lives: " + player.CurrentLives);
				QueueFree(); // remove pickup
			}
			else
			{
				GD.Print("[LifePickup] Player already at max lives, pickup ignored.");
				QueueFree(); // optionally remove pickup anyway
			}
		}
	}
}
