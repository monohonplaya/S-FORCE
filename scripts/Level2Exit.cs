using Godot;
using System;

public class Level2Exit : Spatial
{
    private Label _topkeks;
    private Label _YSFiles;
    private Label _Time;
    private Label _FinalScore;
    private Control _scorescreen;

    public override void _Ready()
    {
        _topkeks = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/Topkeks");
        _YSFiles = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/YSFiles");
        _Time = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/Time");
        _FinalScore = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/FinalScore");
        _scorescreen = GetNode<Control>("Control");
    }

    public async void OnPlayerEntered(Node body)
    {
        await ToSignal(GetTree().CreateTimer(0.3F), "timeout");
        GameData.StopTimer();
        GetTree().Paused = true;
        Input.SetMouseMode(Input.MouseMode.Visible);
        _topkeks.Text = GameData.CollectedTopKek.ToString() + " [x100pts]";
        _YSFiles.Text = GameData.CollectedYSSet.Count.ToString() + "/" + GameData.YSFiles.Count.ToString() + " [x1000pts]";
        ulong minutes = (GameData.ElapsedTime / 1000UL) / 60UL;
        ulong seconds = (GameData.ElapsedTime / 1000UL) % 60UL;
        _Time.Text = String.Format("{0}:{1:D2}", minutes, seconds) + " [x" + 30F * 60F / (float)seconds + "]";
        _FinalScore.Text = ((ulong)(((ulong)GameData.CollectedTopKek * 100UL + (ulong)GameData.CollectedYSSet.Count * 1000UL) * (30F * 60F / (float)(GameData.ElapsedTime / 1000UL)))).ToString();
        _scorescreen.Visible = true;
    }
    public void OnMainMenuPressed()
    {
        GetTree().Paused = !GetTree().Paused;
        GetTree().ChangeScene("Levels/IntroScene.tscn");
    }
}
