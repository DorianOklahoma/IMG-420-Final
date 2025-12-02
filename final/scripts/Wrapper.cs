using Godot;
using System;

public partial class Wrapper : Node
{
	[Export] private Node _generator;
	[Export] private Node2D _centerNode;

	private void _process(double delta)
	{
		_generator.Call("generate");
	}
}
