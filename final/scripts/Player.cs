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

	// Ability
	[Export] public int AbilityCharges = 0;
	[Export] private NodePath AbilityAreaPath;
	[Export] private float AbilityActiveTime = 0.15f;

	private Area2D _abilityArea;
	private CollisionShape2D _abilityShape;
	private bool _abilityActive = false;
	private float _abilityTimer = 0f;

	private RestartMenu _restartMenu;
	private TimerUI _timer;

	public int CurrentLives => _currentLives;    // getter for pickups
	public int MaxLivesPublic => MaxLives;       // getter for pickups


	public override void _Ready()
	{
		_currentLives = MaxLives;
		SetProcess(false);
		SetPhysicsProcess(false);

		// Restart menu
		if (RestartMenuPath != null && !string.IsNullOrEmpty(RestartMenuPath))
		{
			_restartMenu = GetNode<RestartMenu>(RestartMenuPath);
			if (_restartMenu != null)
				_restartMenu.Visible = false;
		}

		// TimerUI
		if (TimerPath != null && !string.IsNullOrEmpty(TimerPath))
		{
			_timer = GetNode<TimerUI>(TimerPath);
			if (_timer != null)
				_timer.StopTimer();
		}

		// Ability Area
		if (AbilityAreaPath != null && !string.IsNullOrEmpty(AbilityAreaPath))
		{
			_abilityArea = GetNode<Area2D>(AbilityAreaPath);
			if (_abilityArea != null)
			{
				_abilityArea.Monitoring = false;
				_abilityShape = _abilityArea.GetNode<CollisionShape2D>("CollisionShape2D");
				if (_abilityShape != null)
					_abilityShape.Disabled = true;

				_abilityArea.BodyEntered += OnAbilityBodyEntered;
			}
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
			if (_invincible && _invincibilityTimer <= 0f)
				_invincible = false;
		}

		// Ability timer
		if (_abilityActive)
		{
			_abilityTimer -= (float)delta;
			if (_abilityTimer <= 0f)
			{
				_abilityActive = false;
				if (_abilityArea != null)
					_abilityArea.Monitoring = false;
				if (_abilityShape != null)
					_abilityShape.Disabled = true;
			}
		}

		// Movement
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		velocity.X = direction.X * Speed;
		velocity.Y = direction.Y * Speed;
		Velocity = velocity;
		MoveAndSlide();

		// Tile / Ability interaction
		if (Input.IsActionJustPressed("interact"))
		{
			if (AbilityCharges > 0)
				ActivateAbility();
			else
				SetTileUnderPlayer();
		}
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

	private void ActivateAbility()
	{
		if (AbilityCharges <= 0) return;

		AbilityCharges--;
		GD.Print("[Player] Ability used! Charges left: " + AbilityCharges);

		_abilityActive = true;
		_abilityTimer = AbilityActiveTime;

		if (_abilityArea != null)
			_abilityArea.Monitoring = true;
		if (_abilityShape != null)
			_abilityShape.Disabled = false;
	}

	private void OnAbilityBodyEntered(Node body)
	{
		if (body is Enemy enemy)
		{
			enemy.QueueFree();
			GD.Print("[Player] Enemy destroyed by ability!");
		}
	}

	public void TakeDamage(int amount = 1)
	{
		if (_invincible) return;

		_currentLives -= amount;
		GD.Print("[Player] Lives left: " + _currentLives);

		_invincible = true;
		_invincibilityTimer = _invincibilityTime;

		if (_currentLives <= 0)
		{
			_gameStarted = false;
			SetProcess(false);
			SetPhysicsProcess(false);

			if (_timer != null)
				_timer.StopTimer();

			if (_restartMenu != null)
			{
				_restartMenu.ShowMenu();
				if (_timer != null)
					_restartMenu.SetFinalTime(_timer.TimeElapsed);
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

		if (_timer != null)
		{
			_timer.ResetTimer();
			_timer.StartTimer();
		}

		if (_restartMenu != null)
			_restartMenu.Visible = false;

		AbilityCharges = 0;

		if (_abilityArea != null)
			_abilityArea.Monitoring = false;
		if (_abilityShape != null)
			_abilityShape.Disabled = true;
	}

	// Utility
	public bool IsInvincible() => _invincible;

	public void AddLife(int amount = 1)
	{
		_currentLives = Math.Min(_currentLives + amount, MaxLives);
		GD.Print("[Player] Gained life. Current: " + _currentLives);
	}

	public void AddAbilityCharge(int amount = 1)
	{
		AbilityCharges += amount;
		GD.Print("[Player] Gained ability charge. Current: " + AbilityCharges);
	}
}
