using Godot;
using System;

public class topkek : KinematicBody
{
    [Export]
    private bool _physicsActive = true;
    private AnimationPlayer _animPlayer;
    private Vector3 _velocity;
    public override void _Ready()
    {
        _animPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _velocity = Vector3.Down;
        GD.Randomize();
    }
    public void InitRandomDropVelocity()
    {
        _velocity = new Vector3(GD.Randf() * 30F, 40F, GD.Randf() * 30F);
    }
    public override void _PhysicsProcess(float delta)
    {
        if (_physicsActive)
        {
            if (IsOnFloor())
                MoveAndSlideWithSnap(Vector3.Down + GetFloorVelocity() * delta, Vector3.Down, Vector3.Up, true);
            else
            {
                _velocity = _velocity.LinearInterpolate(7 * Vector3.Down, .15F);
                MoveAndSlide(_velocity, Vector3.Up, true);
            }
        }
    }
    private void onBodyEntered(Node body)
    {
        if (body.GetParent().GetType() == typeof(PlayerController))
        {
            PlayerController player = (PlayerController)body.GetParent();
            player.IncrementTopKeks();
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
