using Godot;
using System;

public partial class StartMenu : CanvasLayer
{
	[Export] private NodePath StartButtonPath;
	[Export] private NodePath GameRootPath;      // Node2D root for the game
	[Export] private NodePath PlayerPath;        // Player node
	[Export] private NodePath EnemySpawnerPath;  // EnemySpawner node
	[Export] private NodePath TimerUIPath;       // <-- ADD THIS

	private Button _startButton;
	private Node2D _gameRoot;
	private Player _player;
	private EnemySpawner _spawner;
	private TimerUI _timerUI;                    // <-- ADD THIS

	public override void _Ready()
	{
		_startButton = GetNode<Button>(StartButtonPath);
		_gameRoot = GetNode<Node2D>(GameRootPath);

		_player = GetNode<Player>(PlayerPath);
		_spawner = GetNode<EnemySpawner>(EnemySpawnerPath);

		_timerUI = GetNode<TimerUI>(TimerUIPath);    // <-- GET TIMERUI

		_startButton.Pressed += OnStartButtonPressed;

		// Hide the game initially
		if (_gameRoot != null)
			_gameRoot.Visible = false;
	}

	private void OnStartButtonPressed()
	{
		Visible = false; // hide menu

		if (_gameRoot != null)
			_gameRoot.Visible = true;

		GD.Print("Game started!");

		// Start game systems
		_player?.StartGame();
		_spawner?.StartSpawner();

		// Start the timer ONLY now
		_timerUI?.ResetAndStart();    // <-- THIS DOES EXACTLY WHAT YOU WANT
	}
}
