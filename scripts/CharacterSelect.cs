using Godot;
using System;

public class CharacterSelect : Control
{
	Panel panel;
	public override void _Ready()
	{
		panel = ((Panel)GetNode("LoadScreen"));
	}
	public async void _OnPinkPressed()
	{
		// Godot doesn't want to show the load screen unless I force it to wait?
		// without the timer the load screen doesn't show.
		// It also doesn't show when the timer length is 0.01F
		panel.Show();
		await ToSignal(GetTree().CreateTimer(0.1F), "timeout");
		GameData.SelectedCharacter = GameData.CharSelect.Pink;
		GetTree().ChangeScene("Levels/FirstLevel.tscn");
	}
	public async void _OnEbilPressed()
	{
		panel.Show();
		await ToSignal(GetTree().CreateTimer(0.1F), "timeout");
		GameData.SelectedCharacter = GameData.CharSelect.Ebil;
		GetTree().ChangeScene("Levels/FirstLevel.tscn");
	}
	public async void _OnSmolPressed()
	{
		panel.Show();
		await ToSignal(GetTree().CreateTimer(0.1F), "timeout");
		GameData.SelectedCharacter = GameData.CharSelect.Smol;
		GetTree().ChangeScene("Levels/FirstLevel.tscn");
	}

}
