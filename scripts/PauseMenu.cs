using Godot;
using System;

public class PauseMenu : Control
{
    public override void _Ready()
    {
        
    }
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            if (GetTree().Paused)
            {
                Input.SetMouseMode(Input.MouseMode.Visible);
                Show();
            }
            else
            {
                Input.SetMouseMode(Input.MouseMode.Captured);
                Hide();
            }
        }
    }
}
