using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 1000.0f;

	[Export] private TileMapLayer tilemap;
	[Export] public NodePath RestartMenuPath;
	[Export] public NodePath TimerPath;

	[Export] public int MaxLives = 3;
	private int _currentLives;

	private bool _gameStarted = false;

	// Invincibility
	private bool _invincible = false;
	private float _invincibilityTime = 3f;
	private float _invincibilityTimer = 0f;

	private RestartMenu _restartMenu;
	private TimerUI _timer;

	public override void _Ready()
	{
		_currentLives = MaxLives;
		SetProcess(false);
		SetPhysicsProcess(false);

		// Get restart menu
		if (RestartMenuPath != null && !string.IsNullOrEmpty(RestartMenuPath))
		{
			_restartMenu = GetNode<RestartMenu>(RestartMenuPath);
			if (_restartMenu != null)
				_restartMenu.Visible = false;
		}

		// Get TimerUI
		if (TimerPath != null && !string.IsNullOrEmpty(TimerPath))
		{
			_timer = GetNode<TimerUI>(TimerPath);
			if (_timer != null)
				_timer.StopTimer(); // ensure timer doesn't start yet
		}
	}

	public void StartGame()
	{
		_gameStarted = true;
		SetProcess(true);
		SetPhysicsProcess(true);

		if (_restartMenu != null)
			_restartMenu.Visible = false;

		if (_timer != null)
		{
			_timer.ResetTimer();
			_timer.StartTimer();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_gameStarted) return;

		// Invincibility timer
		if (_invincible)
		{
			_invincibilityTimer -= (float)delta;
			if (_invincibilityTimer <= 0f)
				_invincible = false;
		}

		// Movement
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		velocity.X = direction.X * Speed;
		velocity.Y = direction.Y * Speed;

		Velocity = velocity;
		MoveAndSlide();

		// Tile interaction
		if (Input.IsActionJustPressed("interact"))
			SetTileUnderPlayer();
	}

	private void SetTileUnderPlayer()
	{
		if (tilemap == null) return;

		Vector2 worldPos = GlobalPosition;
		Vector2 localPos = tilemap.ToLocal(worldPos);
		Vector2I tileCoords = tilemap.LocalToMap(localPos);

		Godot.Collections.Array<Vector2I> tileArray = new Godot.Collections.Array<Vector2I>();
		for (int i = -1; i <= 1; i++)
			for (int j = -1; j <= 1; j++)
				tileArray.Add(tileCoords + new Vector2I(i, j));

		tileArray.Add(tileCoords);
		tilemap.SetCellsTerrainConnect(tileArray, 0, 0);
	}

	public void TakeDamage(int amount = 1)
	{
		if (_invincible) return;

		_currentLives -= amount;
		GD.Print($"[Player] Lives left: {_currentLives}");

		_invincible = true;
		_invincibilityTimer = _invincibilityTime;

		if (_currentLives <= 0)
		{
			// Stop movement and game
			_gameStarted = false;
			SetProcess(false);
			SetPhysicsProcess(false);

			// Stop timer
			if (_timer != null)
				_timer.StopTimer();

			// Show restart menu and send final time
			if (_restartMenu != null)
			{
				_restartMenu.ShowMenu();
				if (_timer != null)
					_restartMenu.SetFinalTime(_timer.TimeElapsed); // property in TimerUI
			}
		}
	}

	public void RestartGame(Vector2 startPosition)
	{
		_currentLives = MaxLives;
		_invincible = false;
		_invincibilityTimer = 0f;
		GlobalPosition = startPosition;

		_gameStarted = true;
		SetProcess(true);
		SetPhysicsProcess(true);

		// Reset timer
		if (_timer != null)
		{
			_timer.ResetTimer();
			_timer.StartTimer();
		}

		if (_restartMenu != null)
			_restartMenu.Visible = false;
	}

	// Optional: check invincibility
	public bool IsInvincible() => _invincible;
}
