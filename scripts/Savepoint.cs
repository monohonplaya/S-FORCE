using Godot;
using System;

public class Savepoint : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private Particles _rings; 
    public override void _Ready()
    {
        _rings = (Particles)GetNode("Particles");
    }
    public void _onAreaBodyEntered(Node body)
    {
        GameData.RespawnPoint = ((Area)GetNode("Area")).GlobalTransform.origin;
        _rings.Set("speed_scale", 1.6);
    }
    public void _onAreaBodyExited(Node body)
    {
        _rings.Set("speed_scale", Mathf.Lerp(1.6F, .3F, .8F));
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
