using Godot;
using System;

public partial class StartMenu : CanvasLayer
{
	[Export] public NodePath StartButtonPath;
	[Export] public NodePath GameRootPath; // Node2D root
	[Export] public NodePath PlayerPath;   // Player node
	[Export] public NodePath EnemySpawnerPath; // EnemySpawner node

	private Button _startButton;
	private Node2D _gameRoot;
	private Player _player;
	private EnemySpawner _spawner;

	public override void _Ready()
	{
		// Get nodes
		_startButton = GetNode<Button>(StartButtonPath);
		_gameRoot = GetNode<Node2D>(GameRootPath);
		_player = GetNode<Player>(PlayerPath);
		_spawner = GetNode<EnemySpawner>(EnemySpawnerPath);

		_startButton.Pressed += OnStartButtonPressed;

		// Hide the game initially
		if (_gameRoot != null)
			_gameRoot.Visible = false;
	}

	private void OnStartButtonPressed()
	{
		Visible = false; // hide start menu

		if (_gameRoot != null)
			_gameRoot.Visible = true; // show game

		// Start the player
		_player?.StartGame();

		// Start the spawner
		_spawner?.StartSpawner();

		GD.Print("Game started!");
	}
}
