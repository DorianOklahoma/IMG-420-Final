using Godot;
using System;

public partial class RestartMenu : CanvasLayer
{
	[Export] public NodePath PlayerPath;
	[Export] public NodePath EnemySpawnerPath;
	[Export] public NodePath RestartButtonPath;

	private Player _player;
	private EnemySpawner _spawner;
	private Button _restartButton;

	public override void _Ready()
	{
		// Get the nodes from the exported paths
		_player = GetNode<Player>(PlayerPath);
		_spawner = GetNode<EnemySpawner>(EnemySpawnerPath);
		_restartButton = GetNode<Button>(RestartButtonPath);

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

		// Reset enemies by calling StartSpawner
		_spawner?.StartSpawner();

		// Hide menu
		Visible = false;
	}

	public void ShowMenu()
	{
		Visible = true;
	}
}
