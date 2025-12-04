using Godot;
using System;

public partial class AbilityPickup : Area2D
{
	[Export] public int AbilityCharges = 3;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.AddAbilityCharge(AbilityCharges);
			QueueFree();
		}
	}
}
