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
	public enum Hat
	{
		None,
		SForceCap,
		PirateHat
	}
	private const String _save_path = "user://savedata.dat";
	public static Vector3 RespawnPoint = Vector3.Zero;
	public static int CollectedTopKek = 0;
	public static int PlayerHealth;
	public static CharSelect SelectedCharacter = CharSelect.Smol;
	public static List<String> YSFiles = new List<String>();
	public static HashSet<int> CollectedYSSet = new HashSet<int>();
	public static Dictionary<string, bool> UnlockedHats = new Dictionary<string, bool>();
	public static PackedScene TopKekScene;
	public static ulong ElapsedTime = 0;
	public static ulong TimerStartTime = 0;
	public static int Deaths = 0;
	public static bool TimerRunning = false;
	public static Hat SelectedHat = Hat.None;
	public override void _Ready()
	{
		TopKekScene = (PackedScene)ResourceLoader.Load("res://Props/topkek.tscn");
		UnlockedHats.Add("None", true);
		UnlockedHats.Add("SForceCap", false);
		UnlockedHats.Add("PirateHat", false);
		LoadData();
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
	// Godot's C# is making json impossible so this nonsense is done
	// might be fixed in more recent version of Godot
	private static string Encode(Dictionary<string, bool> dict)
	{
		string data = "";
		foreach (string k in dict.Keys)
		{
			data += k + "=" + dict[k].ToString() + "\n";
		}
		data += "HatSelection=" + SelectedHat.ToString() + "\n";
		return data;
	}
	private static Dictionary<string, bool> Decode(String data)
	{
		Dictionary<string, bool> ret = new Dictionary<string, bool>();
		string[] tmp = data.Split('\n');
		//GD.Print(tmp);
		foreach(string s in tmp)
		{
			string[] kvpair = s.Split('=');
			if (kvpair.Length > 1)
			{
				if (kvpair[0].Equals("HatSelection"))
				{
					switch (kvpair[1])
					{
						case "None":
							SelectedHat = Hat.None;
							break;
						case "SForceCap":
							SelectedHat = Hat.SForceCap;
							break;
						case "PirateHat":
							SelectedHat = Hat.PirateHat;
							break;
					}
					continue;
				}
				ret.Add(kvpair[0], kvpair[1] == "True" ? true : false);
			}
		}
		return ret;
	}
	public static void SaveData()
	{
		String data = Encode(UnlockedHats);
		File save = new File();
		if (save.Open(_save_path, File.ModeFlags.Write) != Error.Ok)
		{
			GD.Print("Couldn't write to save file at " + _save_path);
		}
		else
		{
			save.StoreLine(data);
			save.Close();
		}
		//GD.Print(data);

	}
	public static void LoadData()
	{
		File save = new File();
		if (save.Open(_save_path, File.ModeFlags.Read) != Error.Ok)
		{
			GD.Print("Couldn't read save file at " + _save_path);
		}
		else
		{
			string text = save.GetAsText();
			//GD.Print(text);
			UnlockedHats = Decode(text);
			save.Close();
		}
	}

}
