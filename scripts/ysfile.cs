using Godot;
using System;

public class ysfile : Area
{
    [Export]
    private String text;
    private AnimationPlayer _animPlayer;
    
    public override void _Ready()
    {
        GameData.YSFiles.Add(text);
        _animPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
    }
    private void onBodyEntered(Node body)
    {
        if (body.GetParent().GetType() == typeof(PlayerController))
        {
            PlayerController player = (PlayerController)body.GetParent();
            GameData.CollectedYSSet.Add(GameData.YSFiles.IndexOf(text));
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
