using Godot;
using System;
public class PlayerController : Spatial
{
    [Signal]
    public delegate void HasMoved();
    [Export]
    private float _moveSpeed = 30;
    [Export]
    private float _runSpeed = 30;
    [Export]
    private float _walkSpeed = 15;
    [Export]
    private float _acceleration = 15;
    [Export]
    private float _airAcceleration = 7;
    [Export]
    private float _angularAcceleration = 30;
    [Export]
    private float _gravity = 3F;
    [Export]
    private float _maxTerminalVelocity = 54;
    [Export]
    private float _jumpPower = 50;
    [Export]
    public float _hitpoints {get; private set; } = 100;
    [Export]
    public float _maxHP {get; private set; } = 100;
    [Export(PropertyHint.Range, "0.1,1")]
    private float _mouseSensitivity = 0.3F;
    [Export(PropertyHint.Range, "-90,0")]
    private float _minPitch = -90;
    [Export(PropertyHint.Range, "0,90")]
    private float _maxPitch = 90;
    private Vector3 _velocity;
    public KinematicBody _player { get; private set; } = null;
    private Area _hitbox; 
    private Vector3 _playerDirection;
    private AnimationTree _animTree;
    private AnimationPlayer _animPlayer;
    private Particles _partic1;
    private Particles _partic2;
    private Timer _knockBackTimer;
    private Timer _landRecoveryTimer;
    private Timer _attackActive;
    private Timer _attackCooldown;
    private Timer _fallTimer;
    private StateMachine _moveFSM;
    private Spatial _hor;
    private Spatial _vert;
    private Spatial _camroot;
    private float _camRotH = 0;
    private float _camRotV = 0;
    private int points = 0;
    private Vector3 _knockBack = Vector3.Zero;
    private bool _knockBackState = false;
    private StateMachine _actionFSM;
    private ClippedCamera _mainCam;
    private ClippedCamera _fallCam;
    private bool _fallDying = false;
    public override void _Ready()
    {
        _player = (KinematicBody)GetNode("bearypink");
        _hitbox = (Area)_player.GetNode("Hitbox");
        _hitbox.Monitoring = false;
        _playerDirection = _player.Rotation;
        _animTree = (AnimationTree)GetNode("bearypink/bearypink/AnimationTree");
        _landRecoveryTimer = (Timer)GetNode("LandRecovery");
        _knockBackTimer = (Timer)GetNode("KnockBack");
        _attackActive = (Timer)GetNode("AttackActive");
        _attackCooldown = (Timer)GetNode("AttackCooldown");
        _fallTimer = (Timer)GetNode("FallTimer");
        _hor = (Spatial)GetNode("Camroot/H");
        _vert = (Spatial)GetNode("Camroot/H/V");
        _camroot = (Spatial)GetNode("Camroot");
        _partic1 = (Particles)GetNode("bearypink/Particles");
        _partic2 =  (Particles)GetNode("bearypink/Particles2");
        _mainCam = (ClippedCamera)_vert.GetNode("ClippedCamera");
        _mainCam.AddException(_player);
        _fallCam = (ClippedCamera)GetNode("FallCamera");
        Input.SetMouseMode(Input.MouseMode.Captured);

        _moveFSM = new StateMachine();
        _moveFSM.AddState("stand_idle");
        _moveFSM.AddState("walkrun");
        _moveFSM.AddState("jump");
        _moveFSM.AddState("air_idle");
        _moveFSM.AddState("land");
        _moveFSM.SetState("stand_idle");

        _actionFSM = new StateMachine();
        _actionFSM.AddState("attack");
        _actionFSM.AddState("cooldown");
        _actionFSM.AddState("none");
        _actionFSM.SetState("none");
        
    }
    public void _on_LandRecovery_timeout()
    {
        if (_moveFSM._state == "land")
        {
            if (_velocity.x != 0 || _velocity.z != 0)
                _moveFSM.SetState("walkrun");
            else
                _moveFSM.SetState("stand_idle");
        }
    }
    public override void _Process(float delta) 
    {
        if (Input.IsActionJustPressed("ui_cancel")){
            if (Input.GetMouseMode() == Input.MouseMode.Visible)
            {
                Input.SetMouseMode(Input.MouseMode.Hidden);
            }
            else
            {
                Input.SetMouseMode(Input.MouseMode.Visible);
            }
        }
    }
    public void IncrementPoints()
    {
        points += 1;
    }
    public override void _Input(InputEvent @event) 
    {
        if (@event is InputEventMouseMotion inputmousemotion)
        {
            _camRotH -= inputmousemotion.Relative.x * _mouseSensitivity;
            _camRotV -= inputmousemotion.Relative.y * _mouseSensitivity;
            _camRotV = Mathf.Clamp(_camRotV, _minPitch, _maxPitch);
        } 
    }
    public override void _PhysicsProcess(float delta)
    {
        if (_fallDying)
        {
            Vector3 camToPlayer = _fallCam.Translation - _player.GlobalTransform.origin;
           //_fallCam.GlobalTransform = new Transform(new Basis(new Quat(_player.GlobalTransform.basis)), _fallCam.GlobalTransform.origin);
            _fallCam.GlobalTransform = _fallCam.GlobalTransform.LookingAt(_player.GlobalTransform.origin, Vector3.Up);
        }
        HandleMovement(delta);
    }
    public String GetState()
    {
        return _moveFSM._state;
    }
    public void BeDamaged(float damage)
    {
        _hitpoints -= damage;
    }
    public void KnockBack(Vector3 dir, float speed)
    {
        _knockBack = dir * speed;
        _velocity = dir * speed;
        _knockBackState = true;
        _knockBackTimer.Start();
    }
    public void FallDeath()
    {
        _fallCam.GlobalTransform =  _mainCam.GlobalTransform;
        _mainCam.Current = false;
        _fallCam.Current = true;
        _fallDying = true;
        _fallTimer.Start();
    }
    public void _onHitboxBodyEntered(Node body)
    {
        GD.Print("_onHitboxBodyEntered");
        spikeman kb = (spikeman)body;
        Vector3 knock = ((kb.GlobalTransform.origin - _hitbox.GlobalTransform.origin).Normalized() + _playerDirection.Normalized());
        //knock.y += .2F;
        kb.KnockBack(knock.Normalized(), 100F);
    }
    public void _onKnockBackTimeout()
    {
        _knockBackState = false;
        _knockBack = Vector3.Zero;
    }
    public void _onAttackActiveTimeout()
    {
        _hitbox.Monitoring = false;
        _actionFSM.SetState("cooldown");
        _attackCooldown.Start();
    }
    public void _onAttackCooldownTimeout()
    {
        _actionFSM.SetState("none");
    }
    public void _onFallTimerTimeout()
    {
        _fallCam.Current = false;
        _mainCam.Current = true;
        _player.Translation = Vector3.Zero;
        _fallDying = false;
    }
    private void HandleMovement(float delta) 
    {
        Vector3 direction = new Vector3();
        float accel = _player.IsOnFloor() ? _acceleration : _airAcceleration;
        // change movement direction and save the most recent player orientation
        if (Input.IsActionPressed("up")) {
            direction -= _hor.Transform.basis.z;
            _playerDirection = direction;
        }
        if (Input.IsActionPressed("down")) {
            direction += _hor.Transform.basis.z;
            _playerDirection = direction;
        }
        if (Input.IsActionPressed("left")) {
            direction -= _hor.Transform.basis.x;
            _playerDirection = direction;
        }
        if (Input.IsActionPressed("right")) {
            direction += _hor.Transform.basis.x;
            _playerDirection = direction;
        }
        switch (_moveFSM._state) 
        {
            case "stand_idle":
                if (direction != Vector3.Zero)
                    _moveFSM.SetState("walkrun");
                _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), -1, delta * accel));
                break;
            case "walkrun":
                if (direction == Vector3.Zero)
                {
                    _moveFSM.SetState("stand_idle");
                    break;
                }
                if (Input.IsActionPressed("movementmod"))
                {
                    _moveSpeed = _walkSpeed;
                    _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), 0, delta * accel));
                }
                else
                {
                    _moveSpeed = _runSpeed;
                    _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), 1, delta * accel));     
                }
                break;
            case "jump":
                _velocity.y = _jumpPower;
                if (!_player.IsOnFloor())
                    _moveFSM.SetState("air_idle");
                    _animTree.Set("parameters/air/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/air/blend_amount"), 1, .01F));
                break;
            case "air_idle":
                _animTree.Set("parameters/air/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/air/blend_amount"), 1, .07F));
                if (_player.IsOnFloor() && _velocity.y < _jumpPower)
                {
                    _animTree.Set("parameters/air/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/air/blend_amount"), 0, .6F));
                    _animTree.Set("parameters/land/active", true);
                    _moveFSM.SetState("land");
                    _landRecoveryTimer.Start();
                }
                    
                break;
            case "land":
                _animTree.Set("parameters/air/blend_amount", 0F);
                break;
        }
        direction = direction.Normalized();
        _player.Rotation = new Vector3(0, Mathf.LerpAngle(_player.Rotation.y, Mathf.Atan2(_playerDirection.x, _playerDirection.z), delta * _angularAcceleration), 0);
       
        var vely = _velocity.y;
        if (direction != Vector3.Zero)
            _velocity = _velocity.LinearInterpolate(_player.Transform.basis.z * _moveSpeed, accel * delta);
        else
        {
            _velocity = _velocity.LinearInterpolate(direction * _moveSpeed, accel * delta);
        }
        _velocity.y = vely;
        
       
        if (_player.IsOnFloor() && _moveFSM._state != "jump") {
            _velocity.y = -0.1F;
        }
        else {
            _velocity.y = Mathf.Clamp(_velocity.y - _gravity, -_maxTerminalVelocity, _maxTerminalVelocity+_jumpPower);
            if (_moveFSM._state != "air_idle" && _moveFSM._state != "jump")
            {
                _moveFSM.SetState("air_idle");
                _animTree.Set("parameters/air/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/air/blend_amount"), 1, .01F));
            }
        }

        if (Input.IsActionJustPressed("jump") && (_moveFSM._state == "stand_idle" || _moveFSM._state == "walkrun" || _moveFSM._state == "land")) {
            _animTree.Set("parameters/jump/active", true);
            _moveFSM.SetState("jump");
        }   
        if (_actionFSM._state == "none" && Input.IsActionJustPressed("attack")) {
            _actionFSM.SetState("attack");
            _attackActive.Start();
            _hitbox.Monitoring = true;
            _animTree.Set("parameters/attack/active", true);
            _partic1.Restart();
            _partic2.Restart();
        }
        if (_knockBackState)
        {
            _velocity += _knockBack;
        }
        if (_moveFSM._state == "jump" || _moveFSM._state == "air_idle")
        {
            _velocity = _player.MoveAndSlide(_velocity, Vector3.Up, true);
        }
        else
        {
            _velocity = _player.MoveAndSlideWithSnap(_velocity + _player.GetFloorVelocity() * delta, Vector3.Down, Vector3.Up, true);
        } 
        if (_velocity.Length() > 0.5F)
        {
            EmitSignal(nameof(HasMoved));
        }
        _hor.RotationDegrees = new Vector3(0,_camRotH,0);
        _vert.RotationDegrees = new Vector3(_camRotV,0,0);
        _camroot.Translation = new Vector3(_player.Translation.x, Mathf.Lerp(_camroot.Translation.y, _player.Translation.y, 0.6F), _player.Translation.z);
    }
}
