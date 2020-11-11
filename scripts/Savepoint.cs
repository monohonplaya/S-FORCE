using Godot;
using System;

public class Savepoint : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private Particles _rings; 
    private bool _resetSpeedScale = false;
    public override void _Ready()
    {
        _rings = (Particles)GetNode("Particles");
    }
    public void _onAreaBodyEntered(Node body)
    {
        if (body.GetParent() is PlayerController)
        {
            GameData.RespawnPoint = ((Area)GetNode("Area")).GlobalTransform.origin;
            _rings.Set("speed_scale", 1.6);
            PlayerController p = body.GetParent() as PlayerController;
            p.PlayCheckpointSound();
            p.ShowCheckpointNotice();
        }
    }
    public void _onAreaBodyExited(Node body)
    {
        _rings.Set("speed_scale", Mathf.Lerp((float)_rings.Get("speed_scale"), .3F, .8F));
        _resetSpeedScale = true;
    }
 
    public override void _Process(float delta)
    {
        if (_resetSpeedScale)
            _rings.Set("speed_scale", Mathf.Lerp((float)_rings.Get("speed_scale"), .3F, .8F));
        if ((float)_rings.Get("speed_scale") < .35F)
            _resetSpeedScale = false;
    }
}
