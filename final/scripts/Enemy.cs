using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float MoveSpeed = 80f;
	[Export] public float ChaseRange = 200f;
	[Export] public float DamageCooldown = 3f; // seconds

	private Node2D _player;
	private float _damageTimer = 0f;

	public override void _Ready()
	{
		_player = GetTree().GetFirstNodeInGroup("player") as Node2D;

		// Connect the Area2D signal
		Area2D hitbox = GetNode<Area2D>("Hitbox");
		hitbox.BodyEntered += OnHitboxBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		_damageTimer -= (float)delta;

		if (_player == null) return;

		float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

		if (distance <= ChaseRange)
		{
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * MoveSpeed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	private void OnHitboxBodyEntered(Node body)
	{
		// Only damage if cooldown is done
		if (_damageTimer > 0f) return;

		if (body is Player player && !player.IsInvincible())
		{
			player.TakeDamage(1);
			_damageTimer = DamageCooldown;
		}
	}
}
