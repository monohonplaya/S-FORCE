using Godot;
using System;

public class wheelobstacle : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    [Export]
    private float _rollSpeed = 4F;
    [Export]
    private float _moveSpeed = .8F;
    private PathFollow _path;
    private MeshInstance _wheel;
    private int _direction = 1;
    public override void _Ready()
    {
        _path = (PathFollow)GetNode("Path/PathFollow");
        _wheel = (MeshInstance)GetNode("Path/PathFollow/Cylinder");
    }
    public override void _PhysicsProcess(float delta)
    {
        _path.UnitOffset += _direction * delta * _moveSpeed;
        _wheel.Rotate(new Vector3(1F,0F,0F), _direction * delta * _rollSpeed);
        if (_path.UnitOffset == 1 || _path.UnitOffset == 0)
            _direction *= -1;
    }
    public void _onHitboxBodyEntered(Node body)
    {
        KinematicBody kb = (KinematicBody)body;
        PlayerController player = (PlayerController)kb.GetParent();
        player.BeDamaged(50);
        player.PlaySquishedAnim();
        //player.KnockBack((kb.GlobalTransform.origin - GlobalTransform.origin), 3F);
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
