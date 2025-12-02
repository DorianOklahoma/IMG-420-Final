using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 1000.0f;
	[Export] private TileMapLayer tilemap;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		
		if (Input.IsActionJustPressed("interact"))
		{
			SetTileUnderPlayer();
		}
	}
	
	private void SetTileUnderPlayer()
	{
		Vector2 worldPos = GlobalPosition;
		Vector2 localPos = tilemap.ToLocal(worldPos);
		Vector2I tileCoords = tilemap.LocalToMap(localPos);
		
		Godot.Collections.Array<Vector2I> tileArray = new Godot.Collections.Array<Vector2I>();
		for (int i = -1; i <= 1; i++) 
		{
			for (int j = -1; j <= 1; j++)
			{
				tileArray.Add(tileCoords + new Vector2I(i, j));
			}
		}
		tileArray.Add(tileCoords);
		
		tilemap.SetCellsTerrainConnect(tileArray, 0, 0);
	}
}
