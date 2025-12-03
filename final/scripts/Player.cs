using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 1000.0f;

	[Export] private TileMapLayer tilemap;

	[Export] public int MaxLives = 3;
	private int _currentLives;

	private bool _gameStarted = false;

	// Invincibility
	private bool _invincible = false;
	private float _invincibilityTime = 3f;
	private float _invincibilityTimer = 0f;

	// Restart menu
	[Export] public NodePath RestartMenuPath;
	private RestartMenu _restartMenu;

	public override void _Ready()
	{
		_currentLives = MaxLives;
		SetProcess(false);
		SetPhysicsProcess(false);

		// Get restart menu node
		if (RestartMenuPath != null && !string.IsNullOrEmpty(RestartMenuPath))
		{
			_restartMenu = GetNode<RestartMenu>(RestartMenuPath);
			if (_restartMenu != null)
				_restartMenu.Visible = false; // hide initially
			else
				GD.PrintErr("[Player] RestartMenu not found at path: ", RestartMenuPath);
		}
	}

	public void StartGame()
	{
		_gameStarted = true;
		SetProcess(true);
		SetPhysicsProcess(true);

		// Ensure restart menu is hidden at start
		if (_restartMenu != null)
			_restartMenu.Visible = false;
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

		if (Input.IsActionJustPressed("interact"))
			SetTileUnderPlayer();
	}

	private void SetTileUnderPlayer()
	{
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
			GD.Print("[Player] Out of lives! Game over.");
			_gameStarted = false;
			SetProcess(false);
			SetPhysicsProcess(false);

			// Show restart menu
			if (_restartMenu != null)
				_restartMenu.ShowMenu();
			else
				GD.PrintErr("[Player] RestartMenu not assigned!");
		}
	}

	public bool IsInvincible() => _invincible;

	// Called by RestartMenu when restart button is pressed
	public void RestartGame(Vector2 startPosition)
	{
		_currentLives = MaxLives;
		_invincible = false;
		_invincibilityTimer = 0f;
		GlobalPosition = startPosition;

		_gameStarted = true;
		SetProcess(true);
		SetPhysicsProcess(true);

		// Ensure restart menu is hidden
		if (_restartMenu != null)
			_restartMenu.Visible = false;
	}
}
