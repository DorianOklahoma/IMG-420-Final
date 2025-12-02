using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public float MoveSpeed = 80f;
	[Export] public float ChaseRange = 200f;

	private Node2D _player;

	public override void _Ready()
	{
		_player = GetTree().GetFirstNodeInGroup("player") as Node2D;
	}

	public override void _PhysicsProcess(double delta)
	{
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
}
