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
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}
