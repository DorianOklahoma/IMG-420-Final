using Godot;
using System;

public partial class PlayerHUD : Node2D
{
	[Export] private NodePath PlayerPath;
	[Export] private Vector2 Offset = new Vector2(0, -50); // HUD offset from player
	[Export] private Label LivesLabel;
	[Export] private Label AbilityLabel;

	private Player _player;

	public override void _Ready()
	{
		if (PlayerPath != null)
			_player = GetNode<Player>(PlayerPath);

		UpdateHUD();
	}

	public override void _Process(double delta)
	{
		if (_player == null) return;

		// Make the HUD follow the player
		GlobalPosition = _player.GlobalPosition + Offset;

		// Update the labels
		UpdateHUD();
	}

	private void UpdateHUD()
	{
		if (_player == null) return;

		if (LivesLabel != null)
			LivesLabel.Text = $"Lives: {_player.CurrentLives}";

		if (AbilityLabel != null)
			AbilityLabel.Text = $"Ability Charges: {_player.AbilityCharges}";
	}
}
