using Godot;
using System;

public partial class RestartMenu : CanvasLayer
{
	[Export] public NodePath PlayerPath;
	[Export] public NodePath EnemySpawnerPath;
	[Export] public NodePath RestartButtonPath;
	[Export] public NodePath FinalTimeLabelPath; // new: Label to show final time

	private Player _player;
	private EnemySpawner _spawner;
	private Button _restartButton;
	private Label _finalTimeLabel; // new

	public override void _Ready()
	{
		_player = GetNode<Player>(PlayerPath);
		_spawner = GetNode<EnemySpawner>(EnemySpawnerPath);
		_restartButton = GetNode<Button>(RestartButtonPath);
		_finalTimeLabel = GetNode<Label>(FinalTimeLabelPath); // new

		if (_restartButton != null)
			_restartButton.Pressed += OnRestartPressed;

		// Hide menu initially
		Visible = false;
	}

	private void OnRestartPressed()
	{
		if (_player == null)
		{
			GD.PrintErr("Cannot restart: player not assigned!");
			return;
		}

		// Reset player
		_player.RestartGame(new Vector2(100, 100));

		// Reset enemies
		_spawner?.StartSpawner();

		// Hide menu
		Visible = false;
	}

	public void ShowMenu()
	{
		Visible = true;
	}

	// NEW: Set the final time label
	public void SetFinalTime(float time)
	{
		if (_finalTimeLabel != null)
			_finalTimeLabel.Text = $"Time: {time:F2}s";
	}
}
