using Godot;
using System;

public class TitleScreen : Spatial
{
	private Control _credits;
	public override void _Ready()
	{
		_credits = GetNode<Control>("Control");
	}
	public void _onCampaignPressed()
	{
		GetTree().ChangeScene("Levels/CharacterSelect.tscn");
		GameData.ResetForNewGame();
	}
	public void OnCreditsPressed()
	{
		_credits.Visible = !_credits.Visible;
	}
	public void OnFullscreenTogglePressed()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}
}
