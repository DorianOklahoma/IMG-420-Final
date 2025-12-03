using Godot;
using System;

public partial class TimerUI : CanvasLayer
{
	[Export] private Label TimerLabel;

	private float _timeElapsed = 0f;
	private bool _running = false;

	public float TimeElapsed => _timeElapsed; 

	public override void _Process(double delta)
	{
		if (!_running) return;

		_timeElapsed += (float)delta;
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		if (TimerLabel != null)
			TimerLabel.Text = $"Time: {_timeElapsed:F2}s";
	}

	public void StartTimer()
	{
		_running = true;
	}

	public void StopTimer()
	{
		_running = false;
	}

	public void ResetTimer()
	{
		_timeElapsed = 0f;
		UpdateLabel();
	}
}
