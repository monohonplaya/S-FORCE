using Godot;
using System;

public class DebugInfo : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    // Called when the node enters the scene tree for the first time.
    private PlayerController _player;
    public override void _Ready()
    {
        _player = (PlayerController)GetNode("/root/Root/PlayerController");
    }
    public override void _PhysicsProcess(float delta)
    {
        Text = _player._hitpoints.ToString();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
