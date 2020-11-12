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
    private static int _score = 0;
    public static Vector3 RespawnPoint = Vector3.Zero;
    public static int CollectedTopKek = 0;
    public static int PlayerHealth;
    public static CharSelect SelectedCharacter = CharSelect.Smol;
    public static List<String> YSFiles = new List<String>();
    public static HashSet<int> CollectedYSSet = new HashSet<int>();
    public static PackedScene TopKekScene;
    public static ulong ElapsedTime = 0;
    public static ulong TimerStartTime = 0;
    public static bool TimerRunning = false;
    public override void _Ready()
    {
        TopKekScene = (PackedScene)ResourceLoader.Load("res://Props/topkek.tscn");
    }
    public static void ResetForNewGame()
    {
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

}
