using Godot;
using System;

public class speedplatform : Spatial
{
    public override void _Ready()
    {
        
    }
    public void _onAreaBodyEntered(Node body) 
    {
        PlayerController pc = ((body as KinematicBody).GetParent() as PlayerController);
        pc.SpeedBoost(-Transform.basis.x);
    }

}
