using Godot;
using System;

public class SpikyBar : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    public void OnHitboxBodyEntered(Node body)
    {
        KinematicBody kb = (KinematicBody)body;
        PlayerController pc = null;
        if (kb.GetParent() is PlayerController)
        {
            pc = (PlayerController)kb.GetParent();
            pc.BeDamaged(20);
            pc.KnockBack((kb.GlobalTransform.origin - GlobalTransform.origin).Normalized(), 6F);
        }
    }
}
