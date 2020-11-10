using Godot;
using System;

public class PauseMenu : Control
{
    private VBoxContainer _menu;
    private VBoxContainer _YSFileScreen;
    private VBoxContainer _YSList;
    private bool _isYSListInit;
    private DynamicFont _font;
    public override void _Ready()
    {
        _menu = GetNode<VBoxContainer>("Menu");
        _YSFileScreen = GetNode<VBoxContainer>("YSFileScreen");
        _YSList = GetNode<VBoxContainer>("YSFileScreen/YSFileList/List");
        _isYSListInit = false;
        _font = GD.Load<DynamicFont>("res://Fonts/YSFileFont.tres");
    }
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            if (GetTree().Paused)
            {
                Input.SetMouseMode(Input.MouseMode.Visible);
                Show();
            }
            else
            {
                _menu.Visible = true;
                _YSFileScreen.Visible = false;
                Input.SetMouseMode(Input.MouseMode.Captured);
                Hide();
            }
        }
    }
    public void onUnpausePressed()
    {
        GetTree().Paused = !GetTree().Paused;
        Input.SetMouseMode(Input.MouseMode.Captured);
        Hide();
    }
    public void onYSFilePressed()
    {
        if (!_isYSListInit)
        {
            for(int i = 0; i < GameData.YSFiles.Count; i++)
            {
                HBoxContainer hb = new HBoxContainer();
                hb.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
                hb.SizeFlagsVertical = (int)SizeFlags.ShrinkCenter;
                Label ind = new Label();
                ind.Text = (i+1).ToString();
                Label txt = new Label();
                if(GameData.CollectedYSSet.Contains(i))
                {
                    txt.Text = GameData.YSFiles[i];
                }
                else
                    txt.Text = "???";
                // ind.Autowrap = true;
                ind.Align = Label.AlignEnum.Center;
                txt.Align = Label.AlignEnum.Left;
                txt.Autowrap = true;
                ind.AddFontOverride("font", _font);
                txt.AddFontOverride("font", _font);
                ind.SizeFlagsHorizontal = (int)SizeFlags.Fill;
                txt.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
                ind.AnchorRight = 0.1F;
                txt.AnchorLeft = 0.1F;
                hb.AddChild(ind);
                hb.AddChild(txt);
                _YSList.AddChild(hb);
            }
            _isYSListInit = true;
        }
        else
        {
            foreach(HBoxContainer h in _YSList.GetChildren())
            {
                int index = h.GetChild<Label>(0).Text.ToInt()-1;
                if (GameData.CollectedYSSet.Contains(index))
                {
                    h.GetChild<Label>(1).Text = GameData.YSFiles[index];
                }
            }
        }
        _menu.Visible = false;
        _YSFileScreen.Visible = true;
    }
    public void onMainMenuPressed()
    {
        GetTree().Paused = !GetTree().Paused;
        GetTree().ChangeScene("Levels/IntroScene.tscn");
    }
    public void onQuitPressed()
    {
        GetTree().Quit();
    }
    public void onBackPressed()
    {
        _menu.Visible = true;
        _YSFileScreen.Visible = false;
    }
}
