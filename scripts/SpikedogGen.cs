using Godot;
using System;

public class SpikedogGen : Navigation
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    private int _numDogs = 128;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PackedScene spikedog = (PackedScene)ResourceLoader.Load("res://Actors/spikedog.tscn");
        Spatial spawns = (Spatial)GetNode("Spawns");
        int i = 0;
        while (i < _numDogs)
        {
            foreach (Spatial s in spawns.GetChildren())
            {
                Vector3 rp = new Vector3(s.Translation.x + GD.Randf()*300, 0, s.Translation.z + GD.Randf()*300);
                Spikedog sd = (Spikedog)spikedog.Instance();
                sd.nav = this;
                sd.Translation = GetClosestPoint(rp);
                //sd.Translation = new Vector3(sd.Translation.x, sd.Translation.y + 20, sd.Translation.z);
                AddChild(sd);
                i += 1;
                if (i >= _numDogs - 1)
                {
                    break;
                }
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
