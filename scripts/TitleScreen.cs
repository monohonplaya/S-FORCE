using Godot;
using System;

public class TitleScreen : Spatial
{
	private Control _credits;
	private Control _hats;
	private Button _hatsButton;
	private CheckBox _SForceCapCheck;
	private CheckBox _PirateHatCheck;
	public override void _Ready()
	{
		_credits = GetNode<Control>("Control");
		_hats = GetNode<VBoxContainer>("Control2/HatsVBox");
		_hatsButton = GetNode<Button>("Control2/VBoxContainer/Hats");
		_SForceCapCheck =  GetNode<CheckBox>("Control2/HatsVBox/SFORCE Cap");
		_PirateHatCheck =  GetNode<CheckBox>("Control2/HatsVBox/Pirate Hat");
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
		if (GameData.UnlockedHats["SForceCap"])
			_SForceCapCheck.Disabled = false;
		if (GameData.UnlockedHats["PirateHat"])
			_PirateHatCheck.Disabled = false;
		if (GameData.UnlockedHats["SForceCap"] || GameData.UnlockedHats["PirateHat"])
			_hatsButton.Disabled = false;

	}
	public void _onCampaignPressed()
	{
		GetTree().ChangeScene("Levels/CharacterSelect.tscn");
		GameData.ResetForNewGame();
		GameData.SaveData();
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
		GameData.SaveData();
	}
	public void OnSForceCapSelect()
	{
		GameData.SelectedHat = GameData.Hat.SForceCap;
		GameData.SaveData();
	}
	public void OnPirateHatSelect()
	{
		GameData.SelectedHat = GameData.Hat.PirateHat;
		GameData.SaveData();
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
