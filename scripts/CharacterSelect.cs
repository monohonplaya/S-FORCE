using Godot;
using System;

public class CharacterSelect : Control
{
    public void _OnPinkPressed()
    {
        GameData.SelectedCharacter = GameData.CharSelect.Pink;
        GetTree().ChangeScene("Levels/FirstLevel.tscn");
    }
    public void _OnEbilPressed()
    {
        GameData.SelectedCharacter = GameData.CharSelect.Ebil;
        GetTree().ChangeScene("Levels/FirstLevel.tscn");
    }
    public void _OnSmolPressed()
    {
        GameData.SelectedCharacter = GameData.CharSelect.Smol;
        GetTree().ChangeScene("Levels/FirstLevel.tscn");
    }

}
