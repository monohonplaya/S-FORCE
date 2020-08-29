using Godot;
using System;

public class Savepoint : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    public void _onAreaBodyEntered(Node body)
    {
        GameData.RespawnPoint = ((Area)GetNode("Area")).GlobalTransform.origin;
        ((Particles)GetNode("Particles")).Set("speed_scale", 1.6);
    }
    public void _onAreaBodyExited(Node body)
    {
        ((Particles)GetNode("Particles")).Set("speed_scale", .6);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
