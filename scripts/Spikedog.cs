using Godot;
using System;

public class Spikedog : KinematicBody
{
    public Navigation nav = null;
    private StateMachine _sm;
    private float _speed = 8F;
    private Vector3 _dir;
    private Vector3 _vel;
    private Timer _timer;
    private float _gravity = 2F;
    private const float _terminalVel = 22F;
    private float _angularAcceleration = 10;
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _sm = new StateMachine();
        _sm.AddState("Moving");
        _sm.AddState("Still");
        _sm.SetState("Moving");
        _timer = new Timer();
        _timer.WaitTime = 3F;
        _timer.Autostart = true;
        _timer.Connect("timeout", this, nameof(_on_Timer_timeout));
        AddChild(_timer);
        _timer.Start();
        _dir = new Vector3(0,0,1F);
        _vel = _dir * _speed;
        _animPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        GD.Randomize();
    }
    public void HitReaction()
    {
        _animPlayer.Play("spikedog_jump");
        if (GD.Randf() < 0.4F)
        {
            topkek tk = (topkek)GameData.TopKekScene.Instance();
            tk.Translation = this.GlobalTransform.origin + 4 * Vector3.Up;
            GetTree().Root.AddChild(tk);
            tk.InitRandomDropVelocity();
        }
    }
    public void _on_Timer_timeout()
    {
        if (GD.Randf() > 0.5F)
        {
            _sm.SetState("Moving");
            _animPlayer.Play("spikedog_walk-loop");
        }
        else
        {
            _sm.SetState("Still");
            _animPlayer.Play("spikedog_idle-loop");
        }
        _timer.Start();
    }
    public override void _PhysicsProcess(float delta)
    {
        float velytmp = _vel.y;
        switch (_sm._state) {
            case "Moving":
                Vector3 old_dir = _dir;
                Vector3 circlePoint = Vector3.Forward.Rotated(Vector3.Up, (float)GD.RandRange(0, 2 * (double)Mathf.Pi));
                _vel = new Vector3(_vel.x, 0, _vel.z);
                _vel = (_vel + circlePoint).Normalized() * _speed;
                _dir = _vel.Normalized();
                //LookAt(this.Translation - _dir, Vector3.Up);
                //Rotate(Vector3.Up, old_dir.AngleTo(_dir));
                Rotation = new Vector3(0, Mathf.LerpAngle(Rotation.y, Mathf.Atan2(_dir.x, _dir.z), delta * _angularAcceleration), 0);
                break;
            case "Still":
                _vel = Vector3.Zero;
                break;
        }
        if (!IsOnFloor())
        {
            _vel.y = velytmp;
            _vel.y -= _gravity;
            _vel.y = Mathf.Clamp(_vel.y, -_terminalVel, _terminalVel);
        }
        MoveAndSlide(_vel, Vector3.Up);
    }
}
