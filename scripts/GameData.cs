using Godot;
using System;

public class GameData : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private int _score = 0;
    public static Vector3 RespawnPoint = Vector3.Zero;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
