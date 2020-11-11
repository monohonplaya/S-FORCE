using Godot;
using System;
public class PlayerController : Spatial
{
    [Signal]
    public delegate void HasMoved();
    [Export]
    private float _moveSpeed = 30;
    [Export]
    private float _baseRunSpeed = 30;
    [Export]
    private float _runSpeed = 30;
    [Export]
    private float _baseWalkSpeed = 15;
    [Export]
    private float _walkSpeed = 15;
    [Export]
    private float _acceleration = 15;
    [Export]
    private float _airAcceleration = 7;
    [Export]
    private float _angularAcceleration = 30;
    [Export]
    private float _boostSpeed = 2.4F;
    [Export]
    private float _gravity = 3F;
    [Export]
    private float _maxTerminalVelocity = 54;
    [Export]
    private float _jumpPower = 50;
    [Export]
    public int _hitpoints {get; private set; } = 100;
    [Export]
    public int _maxHP {get; private set; } = 100;
    [Export(PropertyHint.Range, "0.1,1")]
    private float _mouseSensitivity = 0.3F;
    [Export(PropertyHint.Range, "-90,0")]
    private float _minPitch = -90;
    [Export(PropertyHint.Range, "0,90")]
    private float _maxPitch = 90;
    private AudioStreamPlayer _collectSound;
    private Vector3 _velocity;
    public KinematicBody _player { get; private set; } = null;
    private HUD _HUD;
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
    private Timer _stepTimer;
    private StateMachine _moveFSM;
    private Spatial _hor;
    private Spatial _vert;
    private Spatial _camroot;
    private float _camRotH = 0;
    private float _camRotV = 0;
    private int topkeks = 0;
    private Vector3 _knockBack = Vector3.Zero;
    private bool _knockBackState = false;
    private StateMachine _actionFSM;
    private ClippedCamera _mainCam;
    private ClippedCamera _fallCam;
    private AudioStreamPlayer3D _attackSound;
    private bool _fallDying = false;
    private bool _dying = false;
    private bool _damagedInvincibility = false;
    private bool _speedBoostState = false;
    private Vector3 _speedBoostVector = Vector3.Zero;
    private AudioStreamPlayer3D _stepPlayer;
    private AudioStream[] _steps = new AudioStream[8];
    private AudioStreamPlayer _checkpointSound;
    private AudioStreamPlayer _speedSound;
    private VBoxContainer _YSList;
    private Label _YSNotice;
    private Label _checkpointNotice;
    private bool _fadeOutNotice = false;
    private bool _ignoreMovement = false;

    public override void _Ready()
    {
        GD.Randomize();
        GameData.RespawnPoint = GlobalTransform.origin;
        GameData.PlayerHealth = _hitpoints;
        PackedScene _playerScene;
        if (GameData.SelectedCharacter == GameData.CharSelect.Pink)
        {
            _playerScene = (PackedScene)ResourceLoader.Load("res://ModelsAndCollisions/bearypink.tscn");
        } 
        else if (GameData.SelectedCharacter == GameData.CharSelect.Ebil) {
            _playerScene = (PackedScene)ResourceLoader.Load("res://ModelsAndCollisions/bearyebil.tscn");
        }
        else
        {
            _playerScene = (PackedScene)ResourceLoader.Load("res://ModelsAndCollisions/bearysmol.tscn");
        }
        _player = (KinematicBody)_playerScene.Instance();
        AddChild(_player);
        

        // _player = (KinematicBody)GetNode("bearypink");
        _hitbox = (Area)_player.GetNode("Hitbox");
        _hitbox.Monitoring = false;
        _playerDirection = _player.Rotation;
        _hitbox.Connect("body_entered", this, nameof(_onHitboxBodyEntered));

        /* this is needed because moving the animation nodes in ebil and smol causes the animation paths to
           be wrong and every bone for every animation needs to be manually edited to fix it >_<
        */
        if (GameData.SelectedCharacter == GameData.CharSelect.Pink)
        {
            _animTree = (AnimationTree)_player.GetNode("mesh/AnimationTree");
        }
        else
        {
            _animTree = (AnimationTree)_player.GetNode("AnimationTree");
        }
        _landRecoveryTimer = (Timer)GetNode("LandRecovery");
        _knockBackTimer = (Timer)GetNode("KnockBack");
        _attackActive = (Timer)GetNode("AttackActive");
        _attackCooldown = (Timer)GetNode("AttackCooldown");
        _fallTimer = (Timer)GetNode("FallTimer");
        _stepTimer = (Timer)GetNode("StepTimer");
        _hor = (Spatial)GetNode("Camroot/H");
        _vert = (Spatial)GetNode("Camroot/H/V");
        _camroot = (Spatial)GetNode("Camroot");
        _partic1 = (Particles)_player.GetNode("Particles");
        _partic2 =  (Particles)_player.GetNode("Particles2");
        _mainCam = (ClippedCamera)_vert.GetNode("ClippedCamera");
        _mainCam.AddException(_player);
        _fallCam = (ClippedCamera)GetNode("FallCamera");
        _attackSound = (AudioStreamPlayer3D)_player.GetNode("Hitbox/AttackSound");
        _collectSound = new AudioStreamPlayer();
        _checkpointSound = GetNode<AudioStreamPlayer>("CheckPointSound");
        _collectSound.Stream = ResourceLoader.Load("res://SFX/collect.wav") as AudioStream;
        _collectSound.VolumeDb = 4F;
        _speedSound = GetNode<AudioStreamPlayer>("SpeedPlatSound");
        _player.AddChild(_collectSound);
        _HUD = (HUD)GetNode("HUD");
        _HUD.UpdatePlayerHealth();
        Input.SetMouseMode(Input.MouseMode.Captured);

        _YSList = GetNode<VBoxContainer>("PauseMenu/YSFileScreen/YSFileList/List");
        _YSNotice = GetNode<Label>("HUD/YSCollectedMessage");
        _checkpointNotice = GetNode<Label>("HUD/Checkpoint");
        _stepPlayer = (AudioStreamPlayer3D)_player.GetNode("StepPlayer");
        for (int i = 0; i < _steps.Length; ++i)
        {
            _steps[i] = ResourceLoader.Load("res://SFX/footsteps/step" + (i+1) + ".wav") as AudioStream;
        }
        _stepPlayer.Stream =  _steps[GD.Randi() % 8];
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
    private void _playRandomStepSound()
    {
        _stepPlayer.Stream =  _steps[GD.Randi() % 8];
        _stepPlayer.Play();
        //GD.Print("PlayStepSound");
    }
    public void _onStepTimerTimeout()
    {
        //GD.Print("SteptimerTimeout");
        _playRandomStepSound();
        _stepTimer.Start();
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
    public async void SpeedBoost(Vector3 direction) 
    {
        _speedSound.Play();
        _animTree.Set("parameters/runscale/scale", 3F);
        _runSpeed = Mathf.Clamp(_runSpeed * _boostSpeed, _baseRunSpeed, _baseRunSpeed * Mathf.Pow(_boostSpeed, 3));
        _walkSpeed = Mathf.Clamp(_walkSpeed * _boostSpeed, _baseWalkSpeed, _baseWalkSpeed * Mathf.Pow(_boostSpeed, 3));
        _moveSpeed = _runSpeed;
        _speedBoostState = true;
        _playerDirection = direction;
        _speedBoostVector = direction;
        _player.Rotation = new Vector3(0, Mathf.Atan2(direction.x, direction.z), 0);
        _camRotH = 180 + _player.RotationDegrees.y;
        _hor.RotationDegrees = new Vector3(0,_camRotH,0);
        _stepTimer.WaitTime = .1F;
        await ToSignal(GetTree().CreateTimer(1.45F), "timeout");
        _runSpeed = Mathf.Clamp(_runSpeed / _boostSpeed, _baseRunSpeed, _baseRunSpeed * Mathf.Pow(_boostSpeed, 3));
        _walkSpeed = Mathf.Clamp(_walkSpeed / _boostSpeed, _baseWalkSpeed, _baseWalkSpeed * Mathf.Pow(_boostSpeed, 3));
        _moveSpeed = _runSpeed;
        _speedBoostState = false;
        _speedBoostVector = Vector3.Zero;
        _stepTimer.WaitTime = .4F;
        _animTree.Set("parameters/runscale/scale", 1.35F);
    }
    public override void _Process(float delta) 
    {
        // if (Input.IsActionJustPressed("ui_cancel")){
        //     if (Input.GetMouseMode() == Input.MouseMode.Visible)
        //     {
        //         Input.SetMouseMode(Input.MouseMode.Hidden);
        //     }
        //     else
        //     {
        //         Input.SetMouseMode(Input.MouseMode.Visible);
        //     }
        // }
    }
    public void PlayCollectSound()
    {
        _collectSound.Play();
    }
    public async void ShowYSNotice()
    {
        _YSNotice.Text = "You got YSFile! (" + GameData.CollectedYSSet.Count + "/" + GameData.YSFiles.Count + ")";
        _YSNotice.PercentVisible = 1F;
        await ToSignal(GetTree().CreateTimer(1.7F), "timeout");
        _fadeOutNotice = true;
    }
    public async void ShowCheckpointNotice()
    {
        _checkpointNotice.PercentVisible = 1F;
        await ToSignal(GetTree().CreateTimer(1.7F), "timeout");
        _checkpointNotice.PercentVisible = 0F;
    }
    public void PlayCheckpointSound()
    {
        _checkpointSound.Play();
    }
    public void IncrementTopKeks()
    {
        PlayCollectSound();
        GameData.CollectedTopKek += 1;
        _HUD.UpdateTopKekCounter();
    }
    public override void _Input(InputEvent @event) 
    {
        if (@event is InputEventMouseMotion inputmousemotion && !_ignoreMovement)
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
        if (_damagedInvincibility)
        {
            _player.Visible = !_player.Visible;
        }
        if (!_dying)
            HandleMovement(delta);
        if (_fadeOutNotice)
        {
            float tmp = _YSNotice.PercentVisible;
            tmp -= 0.06F;
            if (tmp < 0.01F)
            {
                _YSNotice.PercentVisible = 0F;
                _fadeOutNotice = false;
            }
            else
                _YSNotice.PercentVisible = tmp;
                
        }
    }
    public String GetState()
    {
        return _moveFSM._state;
    }
    public async void BeDamaged(int damage)
    {
        if (!_damagedInvincibility)
        {
            _hitpoints -= damage;
            GameData.PlayerHealth = _hitpoints;
            _HUD.UpdatePlayerHealth();
            if (_hitpoints <= 0)
            {
                _dying = true;
                 _damagedInvincibility = true;
                _player.SetCollisionLayerBit(1, false);
                await ToSignal(GetTree().CreateTimer(1F), "timeout");
                _dying = false;
                _damagedInvincibility = false;
                _player.SetCollisionLayerBit(1, true);
                _player.Visible = true;
                _die();
                return;
            }
            _damagedInvincibility = true;
            _player.SetCollisionLayerBit(1, false);
            await ToSignal(GetTree().CreateTimer(2F), "timeout");
            _damagedInvincibility = false;
            _player.SetCollisionLayerBit(1, true);
            _player.Visible = true;
        }
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
    public void PlaySquishedAnim()
    {
        _animTree.Set("parameters/squished/active", true);
    }
    public void _onHitboxBodyEntered(Node body)
    {
        if (body.IsInGroup("spikedog"))
        {
            Spikedog sd = body as Spikedog;
            sd.HitReaction();
        }
        if (body.IsInGroup("spikeman"))
        {
            spikeman kb = body as spikeman;
            Vector3 knock = ((kb.GlobalTransform.origin - _hitbox.GlobalTransform.origin).Normalized() + _playerDirection.Normalized());
            kb.KnockBack(knock.Normalized(), 100F);
            kb.BeDamaged(20);
        }
        if (body.IsInGroup("fwewe"))
        {
            Fwewe fb = body as Fwewe;
            fb.Dialogue.Visible = true;
            if (fb.player == null)
                fb.player = this;
            Input.SetMouseMode(Input.MouseMode.Visible);
            _ignoreMovement = true;
        }
    }
    public void RegainControl()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
        _ignoreMovement = false;
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
        _die();
        _fallDying = false;
    }
    private void _die()
    {
        _player.Translation = ToLocal(GameData.RespawnPoint);
        _hitpoints = _maxHP;
        GameData.PlayerHealth = _hitpoints;
        _HUD.UpdatePlayerHealth();
    }
    private void HandleMovement(float delta) 
    {
        Vector3 direction = Vector3.Zero;
        float accel = _player.IsOnFloor() ? _acceleration : _airAcceleration;
        // change movement direction and save the most recent player orientation
        if (!_ignoreMovement)
        {
            if (_speedBoostState)
            {
                direction = _speedBoostVector * 7;
            }
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
        }
        switch (_moveFSM._state) 
        {
            case "stand_idle":
                _stepTimer.Paused = true;
                if (direction != Vector3.Zero)
                {
                    _moveFSM.SetState("walkrun");
                    _stepTimer.Paused = false;
                    _stepTimer.Start();
                    _playRandomStepSound();
                }
                _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), -1, delta * accel));
                break;
            case "walkrun":
                _stepTimer.Paused = false;
                if (direction == Vector3.Zero)
                {
                    _moveFSM.SetState("stand_idle");
                    _stepTimer.Paused = true;
                    break;
                }
                if (Input.IsActionPressed("movementmod"))
                {
                    _stepTimer.WaitTime = .6F;
                    _moveSpeed = _walkSpeed;
                    _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), 0, delta * accel));
                }
                else
                {
                    _stepTimer.WaitTime = .3F;
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
                _stepTimer.Paused = true;
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
                _playRandomStepSound();
                _animTree.Set("parameters/air/blend_amount", 0F);
                break;
        }
        if (_speedBoostState)
        {
            _stepTimer.WaitTime = .1F;
            _moveSpeed = _runSpeed;
        }
            
        direction = direction.Normalized();
        _player.Rotation = new Vector3(0, Mathf.LerpAngle(_player.Rotation.y, Mathf.Atan2(_playerDirection.x, _playerDirection.z), delta * _angularAcceleration), 0);
       
        var vely = _velocity.y;
        if (direction != Vector3.Zero)
            _velocity = _velocity.LinearInterpolate(_player.Transform.Rotated(Vector3.Up, Rotation.y).basis.z * _moveSpeed, accel * delta);
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

        if (!_ignoreMovement && (Input.IsActionJustPressed("jump") && (_moveFSM._state == "stand_idle" || _moveFSM._state == "walkrun" || _moveFSM._state == "land"))) {
            _animTree.Set("parameters/jump/active", true);
            _moveFSM.SetState("jump");
            _stepTimer.Paused = true;
        }   
        if (_actionFSM._state == "none" && Input.IsActionJustPressed("attack") && !_ignoreMovement) {
            _actionFSM.SetState("attack");
            _attackActive.Start();
            _hitbox.Monitoring = true;
            _animTree.Set("parameters/attack/active", true);
            _attackSound.Play();
            _partic1.Restart();
            _partic2.Restart();
        }
        if (_hitbox.Monitoring == true)
        {
            // I don't know how to do collisions of still bodies in Godot
            // this seems to increase the chance of the hitbox collision, but not always.
            foreach(Node body in _hitbox.GetOverlappingBodies())
            {
                if (body.IsInGroup("spikedog"))
                {
                    Spikedog sd = body as Spikedog;
                    sd.HitReaction();
                    _hitbox.Monitoring = false;
                    break;
                }
            }
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
