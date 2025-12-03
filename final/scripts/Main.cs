using Godot;
using System;

public partial class Main : Node2D
{
	[Export] public NodePath PlayerPath;
	[Export] public NodePath EnemySpawnerPath;
	[Export] public NodePath RestartMenuPath;
	[Export] public NodePath TimerUIPath;

	private Player _player;
	private EnemySpawner _spawner;
	private RestartMenu _restartMenu;
	private TimerUI _timerUI;

	public override void _Ready()
	{
		// Get nodes using correct types
		if (PlayerPath != null)
			_player = GetNode<Player>(PlayerPath);
		if (EnemySpawnerPath != null)
			_spawner = GetNode<EnemySpawner>(EnemySpawnerPath);
		if (RestartMenuPath != null)
			_restartMenu = GetNode<RestartMenu>(RestartMenuPath);
		if (TimerUIPath != null)
			_timerUI = GetNode<TimerUI>(TimerUIPath);

		// Ensure restart menu is hidden
		if (_restartMenu != null)
			_restartMenu.Visible = false;

		// Start the game timer
		_timerUI?.StartTimer();

		// Connect restart button if needed
		if (_restartMenu != null && _restartMenu.RestartButtonPath != null)
		{
			Button restartBtn = _restartMenu.GetNode<Button>(_restartMenu.RestartButtonPath);
			restartBtn.Pressed += OnRestartPressed;
		}
	}

	// Called by Player when lives reach 0
	public void OnPlayerDied()
	{
		GD.Print("[Main] Player died! Stopping timer and showing time.");
		_timerUI?.StopTimer();

		if (_restartMenu != null)
		{
			_restartMenu.ShowMenu();
			_restartMenu.SetFinalTime(_timerUI?.TimeElapsed ?? 0f); // optional display
		}
	}

	private void OnRestartPressed()
	{
		GD.Print("[Main] Restarting game!");

		// Reset player
		_player?.RestartGame(new Vector2(100, 100));

		// Reset enemies
		_spawner?.StartSpawner();

		// Reset timer
		_timerUI?.ResetTimer();
		_timerUI?.StartTimer();

		// Hide restart menu
		_restartMenu.Visible = false;
	}
}
