using Godot;
using System;

public class Level2Exit : Spatial
{
	private Label _topkeks;
	private Label _YSFiles;
	private Label _YSComplete;
	private Label _Deaths;
	private Label _Time;
	private Label _FinalScore;
	private Control _scorescreen;

	public override void _Ready()
	{
		_topkeks = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/Topkeks");
		_YSFiles = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/YSFiles");
		_YSComplete = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/YSComplete");
		_Deaths = GetNode<Label>("Control/Panel/VBoxContainer/HBoxContainer/VBoxContainer2/Deaths");
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

		// score calculation
		int collectedTKs = GameData.CollectedTopKek;
		int tkScore = collectedTKs * 100;
		int collectedYSFiles = GameData.CollectedYSSet.Count;
		int YSScore = collectedYSFiles * 1000;
		int YSBonus = 19992445;
		int totalYSFiles = GameData.YSFiles.Count;
		int noDeathBonus = 5000;
		int deaths = GameData.Deaths; 
		int deathScore = deaths * -200;
		if (deaths == 0)
		{
			deathScore += noDeathBonus;
			GameData.UnlockedHats["SForceCap"] = true;
		}
		ulong ms_time = GameData.ElapsedTime;
		ulong minutes = (ms_time / 1000UL) / 60UL;
		ulong seconds = (ms_time / 1000UL) % 60UL;
		ulong ms = ms_time % 1000;

		_topkeks.Text = collectedTKs + " [" + tkScore + "pts]";
		_YSFiles.Text = collectedYSFiles + "/" + totalYSFiles + " [" + YSScore + "pts]";
		if (collectedYSFiles == totalYSFiles)
		{
			_YSComplete.Text = " [" + YSBonus.ToString() + "pts]";
			GameData.UnlockedHats["PirateHat"] = true;
		}
		else
			_YSComplete.Text = " [" + 0.ToString() + "pts]";
		_Deaths.Text = deaths + " [" + deathScore.ToString() + "pts]";
		_Time.Text = String.Format("{0}:{1:D2}.{2:D3}", minutes, seconds, ms) + 
			" [x" + (30F * 60F / ((float)ms_time / 1000UL)).ToString("n2") + "]";
		_FinalScore.Text = ((ulong)(((ulong)collectedTKs * 100UL + (ulong)collectedYSFiles * 1000UL + (ulong)deathScore + (ulong)YSBonus) 
			* (30F * 60F / ((float)(ms_time / 1000UL))))).ToString();
		_scorescreen.Visible = true;
		GameData.SaveData();
	}
	/* public static void TestScore(int collectedTKs, int collectedYSFiles, int totalYSFiles, int deaths, ulong ms_time)
	{
		int tkScore = collectedTKs * 100;
		int YSScore = collectedYSFiles * 1000;
		int YSBonus = 19992445;
		int noDeathBonus = 5000;
		int deathScore = deaths * -200;
		if (deaths == 0)
			deathScore += noDeathBonus;
		ulong minutes = (ms_time / 1000UL) / 60UL;
		ulong seconds = (ms_time / 1000UL) % 60UL;
		ulong ms = ms_time % 1000;

		GD.Print("TopKeks: " + collectedTKs + " [" + tkScore + "pts]");
		GD.Print("YSFiles: " + collectedYSFiles + "/" + totalYSFiles + " [" + YSScore + "pts]");
		if (collectedYSFiles == totalYSFiles)
			GD.Print("YSComplete: " + " [" + YSBonus.ToString() + "pts]");
		else
			GD.Print("YSComplete: " + " [" + 0.ToString() + "pts]");
		
		GD.Print("Deaths: " + deaths + " [" + deathScore.ToString() + "pts]");
		GD.Print("Time: " + String.Format("{0}:{1:D2}.{2:D3}", minutes, seconds, ms) + 
			" [x" + (30F * 60F / ((float)ms_time / 1000UL)).ToString("n2") + "]");
		GD.Print("Final Score: " + 
			((ulong)(((ulong)collectedTKs * 100UL + (ulong)collectedYSFiles * 1000UL + (ulong)deathScore + YSBonus) 
			* (30F * 60F / ((float)(ms_time / 1000UL))))).ToString()
			);
	} */
	public void OnMainMenuPressed()
	{
		GetTree().Paused = !GetTree().Paused;
		GetTree().ChangeScene("Levels/IntroScene.tscn");
	}
}
