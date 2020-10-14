using Godot;
using System;

public class spikeman : KinematicBody
{
    [Export]
    private float _speed = 10F;
    [Export]
    private float _hopSpeed = 30F;
    private float _currrentSpeed = 0;
    [Export]    
    private float _gravity = 3F;
    [Export]
    private float _jumpPower = 22F;
    [Export]
    private float _attackRange = 10F;
    [Export]
    private float _maxTerminalVelocity = 54F;
    [Export]
    private NodePath _navPath = "Root/Level1/Navigation";
    private Navigation _nav;
    private Vector3[] _path;
    private int _pathInd = 0;
    private StateMachine spikeSM;
    private KinematicBody _player;
    private AnimationTree _animTree;
    private Timer timer;
    private Timer _knockBackTimer;
    private Vector3 _velVec = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private float _yVel = 0F;
    private Vector3 _knockBack = Vector3.Zero;
    private Area _hitbox;
    private int health = 60;
    public override void _Ready()
    {
        _nav = (Navigation)GetNode(_navPath);
        foreach (Node n in GetTree().GetNodesInGroup("players"))
        {
            ((PlayerController)n).Connect("HasMoved", this, nameof(PlayerHasMoved));
            _player = ((PlayerController)n)._player;
        } 
        spikeSM = new StateMachine();
        spikeSM.AddState("HopAround");
        spikeSM.AddState("FollowPlayer");
        spikeSM.AddState("Attack");
        spikeSM.AddState("KnockedBack");
        spikeSM.SetState("HopAround");
        timer = (Timer)GetNode("Timer");
        _hitbox = (Area)GetNode("Hitbox");
        _knockBackTimer = (Timer)GetNode("Knockback");
        _animTree = (AnimationTree)GetNode("AnimationTree");
    }
    public void KnockBack(Vector3 dir, float speed)
    {
        _hitbox.Monitoring = false;
        _knockBack = dir * speed;
        _velVec = dir * speed;
        _yVel = _knockBack.y;
        spikeSM.SetState("KnockedBack");
        _knockBackTimer.Start();
    }
    public void BeDamaged(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    private async void Die()
    {
        _animTree.Set("parameters/Die/active", true);
        await ToSignal(GetTree().CreateTimer(1F), "timeout");
        QueueFree();
    }
    public void PlayerHasMoved() 
    {
        if ((_nav.GetClosestPoint(_player.GlobalTransform.origin) - _player.GlobalTransform.origin).Length() < 1F)
        {
            _path = _nav.GetSimplePath(Translation, _nav.GetClosestPoint(_player.GlobalTransform.origin));
            _pathInd = 0;
        }
        else
        {
            _path = null;
            _pathInd = 0;
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        switch (spikeSM._state) {
            case "HopAround":
                if (_path != null)
                {
                    spikeSM.SetState("FollowPlayer");
                }
                else
                {
                    if (IsOnFloor())
                    {
                        _animTree.Set("parameters/AirSlither/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/AirSlither/blend_amount"), 0, 0.8F));  
                        _currrentSpeed = _speed;
                    }
                    else
                    {
                        _animTree.Set("parameters/AirSlither/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/AirSlither/blend_amount"), 1, 0.6F));
                        _direction = Transform.basis.z;
                        _currrentSpeed = _hopSpeed;
                    }
                }
                break;
            case "FollowPlayer":
                if (IsOnFloor())
                {
                    if (_path != null && _pathInd < _path.Length)
                    {
                        _animTree.Set("parameters/AirSlither/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/AirSlither/blend_amount"), 0, 0.8F));
                        _direction = _path[_pathInd] - GlobalTransform.origin;
                        while (_direction.Length() < 1F && _pathInd < _path.Length)
                        {
                            _direction = _path[_pathInd] - GlobalTransform.origin;
                            _pathInd += 1;
                        }
                        Rotation = new Vector3(0, Mathf.LerpAngle(Rotation.y, Mathf.Atan2(_direction.x, _direction.z), .2F), 0);
                        _currrentSpeed = _speed;
                    }
                    else
                    {
                        spikeSM.SetState("HopAround");
                    }
                    Vector3 distance = _player.GlobalTransform.origin - GlobalTransform.origin;
                    if (distance.Length() < _attackRange)
                    {
                        spikeSM.SetState("Attack");
                        _animTree.Set("parameters/jump/active", true);
                        _yVel = _jumpPower;
                        _direction = _player.GlobalTransform.origin - GlobalTransform.origin;
                        _currrentSpeed = _hopSpeed;
                        Rotation = new Vector3(0, Mathf.LerpAngle(Rotation.y, Mathf.Atan2(_direction.x, _direction.z), .4F), 0);
                    }
                }
                break;
            case "Attack":
                if(IsOnFloor())
                {
                    spikeSM.SetState("FollowPlayer");
                }
                break;
        }
        if (!IsOnFloor())
        {
            _animTree.Set("parameters/AirSlither/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/AirSlither/blend_amount"), 1, 0.6F));
            _yVel = Mathf.Clamp(_yVel - _gravity, -_maxTerminalVelocity, _maxTerminalVelocity+_jumpPower);
        }
        if (spikeSM._state == "KnockedBack")
        {
            _knockBack = new Vector3(Mathf.Lerp(_knockBack.x, 0, .1F), Mathf.Lerp(_knockBack.y, 0, .1F), Mathf.Lerp(_knockBack.z, 0, .1F));
            _velVec = _knockBack;
            _velVec.y = _yVel;
            _velVec = MoveAndSlide(_velVec, Vector3.Up);
            return;
        }
        _velVec = _direction.Normalized() * _currrentSpeed;
        _velVec.y = _yVel;
        _velVec = MoveAndSlide(_velVec, Vector3.Up);
        Vector3 closest = _nav.GetClosestPoint(Translation);
        Vector3 closestNoY = new Vector3(closest.x, 0, closest.z);
        if ((new Vector3(Translation.x, 0, Translation.z) - closestNoY).Length() > 0.1F)
        {
            Translation = new Vector3(Mathf.Lerp(Translation.x,closestNoY.x, 0.9F), closest.y, Mathf.Lerp(Translation.z,closestNoY.z, 0.9F));
            _velVec = new Vector3(0,_velVec.y, 0);
            float rotAngle = (float)GD.RandRange(-Math.PI,Math.PI);
            Rotate(Vector3.Up, rotAngle);
            _direction = Transform.basis.z;
            _velVec = _direction.Normalized() * _hopSpeed;
            _velVec.y = _yVel;
        } 
    }

    public void _on_Timer_timeout()
    {
        if(IsOnFloor() && spikeSM._state == "HopAround")
        {
            _animTree.Set("parameters/jump/active", true);
            _yVel = _jumpPower;
            float rotAngle = (float)GD.RandRange(-Math.PI,Math.PI);
            Rotate(Vector3.Up, rotAngle);
        }
        timer.Start();
    }
    public void _onKnockbackTimeout()
    {
        spikeSM.SetState("HopAround");
        _knockBack = Vector3.Zero;
        _hitbox.Monitoring = true;
    }
    public void _onHitboxEntered(Node body)
    {
        KinematicBody kb = (KinematicBody)body;
        PlayerController pc = null;
        if (kb.GetParent() is PlayerController)
        {
            pc = (PlayerController)kb.GetParent();
            pc.BeDamaged(20);
            pc.KnockBack((kb.GlobalTransform.origin - GlobalTransform.origin).Normalized(), 6F);
        }
    }
}
