using Godot;
using System;

public partial class Wrapper : Node
{
	[Export] private Node _generator;
	
	public override void _Ready()
	{
		if (_generator == null)
		{
			GD.PrintErr("No ProceduralGenerator2D assigned!");
			return;
		}
		
		_generator.Call("generate");
		
		GD.Print("C#: procedural generation triggered!");
	}
}
