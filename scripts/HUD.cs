using Godot;
using System;

public class HUD : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private Label _numTK;
    private TextureProgress _healthBar;
    public override void _Ready()
    {
        _numTK = (Label)GetNode("Label");
        _numTK.Text = GameData.CollectedTopKek.ToString();
        _healthBar = (TextureProgress)GetNode("MarginContainer/health");
        _healthBar.Value = GameData.PlayerHealth;
    }
    public void UpdateTopKekCounter()
    {
        _numTK.Text = GameData.CollectedTopKek.ToString();
    }
    public void UpdatePlayerHealth()
    {
        _healthBar.Value = GameData.PlayerHealth;
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
