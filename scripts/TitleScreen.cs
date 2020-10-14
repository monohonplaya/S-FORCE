using Godot;
using System;

public class TitleScreen : Spatial
{
    public override void _Ready()
    {
        
    }
    public void _onCampaignPressed()
    {
        GetTree().ChangeScene("Levels/CharacterSelect.tscn");
    }
}
