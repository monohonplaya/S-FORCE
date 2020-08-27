using Godot;
using System;

public class rotateplatform : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    [Export]
    private float _rotateSpeed = 1;
    [Export]
    private Vector3 _rotateVec = Vector3.Up;
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        Rotate(_rotateVec, _rotateSpeed * delta);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
