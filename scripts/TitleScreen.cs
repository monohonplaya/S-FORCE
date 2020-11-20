using Godot;
using System;

public class TitleScreen : Spatial
{
	private Control _credits;
	private Control _hats;
	public override void _Ready()
	{
		_credits = GetNode<Control>("Control");
		_hats = GetNode<VBoxContainer>("Control2/HatsVBox");
		switch (GameData.SelectedHat) {
			case GameData.Hat.None:
				_hats.GetChild<CheckBox>(0).Pressed = true;
				break;
			case GameData.Hat.SForceCap:
				_hats.GetChild<CheckBox>(1).Pressed = true;
				break;
			case GameData.Hat.PirateHat:
				_hats.GetChild<CheckBox>(2).Pressed = true;
				break;
		}
	}
	public void _onCampaignPressed()
	{
		GetTree().ChangeScene("Levels/CharacterSelect.tscn");
		GameData.ResetForNewGame();
	}
	public void OnHatsPressed()
	{
		_credits.Visible = false;
		_hats.Visible = !_hats.Visible;
	}
	// Terrible, but couldn't figure out how to load a ButtonGroup
	public void OnNoneSelect()
	{
		GameData.SelectedHat = GameData.Hat.None;
	}
	public void OnSForceCapSelect()
	{
		GameData.SelectedHat = GameData.Hat.SForceCap;
	}
	public void OnPirateHatSelect()
	{
		GameData.SelectedHat = GameData.Hat.PirateHat;
	}
	public void OnBackPressed()
	{
		_hats.Visible = false;
	}
	public void OnCreditsPressed()
	{
		_hats.Visible = false;
		_credits.Visible = !_credits.Visible;
	}
	public void OnFullscreenTogglePressed()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}
}
