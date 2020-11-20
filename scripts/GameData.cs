using Godot;
using System;
using System.Collections.Generic;

public class GameData : Node
{
	public enum CharSelect
	{
		Pink,
		Ebil,
		Smol
	}
	private const String _save_path = "user://savedata.json";
	public static Vector3 RespawnPoint = Vector3.Zero;
	public static int CollectedTopKek = 0;
	public static int PlayerHealth;
	public static CharSelect SelectedCharacter = CharSelect.Smol;
	public static List<String> YSFiles = new List<String>();
	public static HashSet<int> CollectedYSSet = new HashSet<int>();
	public static Godot.Collections.Dictionary<String, bool> UnlockedHats = new Godot.Collections.Dictionary<String, bool>()
	{
		{ "Pirate": false },
		{ "TopLel": false }
	};
	public static PackedScene TopKekScene;
	public static ulong ElapsedTime = 0;
	public static ulong TimerStartTime = 0;
	public static int Deaths = 0;
	public static bool TimerRunning = false;
	public override void _Ready()
	{
		TopKekScene = (PackedScene)ResourceLoader.Load("res://Props/topkek.tscn");
		// Level2Exit.TestScore(0, 0, 16, 5, 192733);
	}
	public static void ResetForNewGame()
	{
		Deaths = 0;
		CollectedTopKek = 0;
		YSFiles = new List<String>();
		CollectedYSSet = new HashSet<int>();
		PlayerHealth = 100;
		ElapsedTime = 0;
		TimerRunning = false;
	}
	public static void StartTimer()
	{
		if (!TimerRunning)
		{
			TimerStartTime = OS.GetTicksMsec();
			TimerRunning = true;
		}
	}
	public static void StopTimer()
	{
		if (TimerRunning)
		{
			ElapsedTime += OS.GetTicksMsec() - TimerStartTime;
			TimerRunning = false;
		}
			
	}
	public static void SaveData()
	{
		File save = new File();
		save.Open(_save_path, (int)File.ModeFlags.Write);
		//save
	}
	public static void LoadData()
	{

	}

}
