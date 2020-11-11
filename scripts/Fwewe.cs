using Godot;
using System;
using System.Collections.Generic;
public class Fwewe : KinematicBody
{
    [Export]
    private string _teleportNodes;
    private List<Spatial> _teleportDests;
    private int _currLoc = 0;
    public Control Dialogue;
    public PlayerController player;
    private AnimationPlayer _anim;
    private Particles _particles;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _particles = GetNode<Particles>("Particles");
        Dialogue = GetNode<Control>("Control");
        _teleportDests = new List<Spatial>();

        // There should be a safer way to get it via absolute path, but GetTree().Root wasn't working for some reason
        foreach (Spatial s in GetParent().GetNode<Spatial>( _teleportNodes).GetChildren())
        {
            _teleportDests.Add(s);
        }
    }
    private void Teleport()
    {
        _anim.Play("Cast");
        _particles.Emitting = true;
    }
    public void OnYesPressed()
    {
        Dialogue.Visible = false;
        Teleport();
    }
    public void OnNoPressed()
    {
        Dialogue.Visible = false;
        player.RegainControl();
    }
    public void OnAnimFinished(String anim_name)
    {
        if (anim_name.Equals("Cast"))
        {
            _anim.Play("fwewe-idle-loop");
            if (player != null)
            {
                /* 
                This seems bad, because the player usually moves its KinematicBody to move,
                but for some reason I couldn't get it working with player._player's local coordinates. 
                Seems to work. Hopefully doesn't cause strange behavior
                */
                Vector3 diffVec = player.Translation - Translation;
                _currLoc += 1;
                if (_currLoc >= _teleportDests.Count)
                    _currLoc = 0;
                Translation = _teleportDests[_currLoc].Translation;
                player.Translation = Translation + diffVec + 4 * Vector3.Up;
                player.RegainControl();
            }
        }
    }
}
