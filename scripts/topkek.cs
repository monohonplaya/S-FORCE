using Godot;
using System;

public class topkek : KinematicBody
{
    private AnimationPlayer _animPlayer;
    public override void _Ready()
    {
      _animPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
    }
    public override void _PhysicsProcess(float delta)
    {
        
        MoveAndSlideWithSnap(new Vector3(0, -1F, 0) + GetFloorVelocity() * delta, Vector3.Down, Vector3.Up, true);

    }
    private void onBodyEntered(Node body)
    {
        if (body.GetParent().GetType() == typeof(PlayerController))
        {
            PlayerController player = (PlayerController)body.GetParent();
            player.IncrementPoints();
            SetCollisionLayerBit(4, false);
            _animPlayer.Play("pickedup");
        }
    }
    private void onPickupAnimFinished(String name)
    {
        if (name == "pickedup")
        {
            QueueFree();
        }
    }
}
