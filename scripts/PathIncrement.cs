using Godot;
using System;

public class PathIncrement : PathFollow
{
    [Export]
    private float _speed = 0.25F;
    public override void _Ready()
    {
    }
    public async override void _PhysicsProcess(float delta)
    {
        UnitOffset += delta * _speed;
    }
}
