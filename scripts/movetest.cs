using Godot;
using System;
public class movetest : Spatial
{
    // Declare member variables here. Examples:
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
    private float _gravity = 4F;
    [Export]
    private float _maxTerminalVelocity = 54;
    [Export]
    private float _jumpPower = 90;
    [Export(PropertyHint.Range, "0.1,1")]
    private float _mouseSensitivity = 0.3F;
    [Export(PropertyHint.Range, "-90,0")]
    private float _minPitch = -90;
    [Export(PropertyHint.Range, "0,90")]
    private float _maxPitch = 90;
    private Timer _jumpDelay;
    private Vector3 _velocity;
    private Spatial _cameraPivot;
    private Camera _camera;
    private KinematicBody _player;
    private Vector3 _playerDirection;
    private AnimationTree _animTree;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cameraPivot = (Spatial)GetNode("CameraPivot");
        _camera = (Camera)GetNode("CameraPivot/SpringArm/Camera");
        _player = (KinematicBody)GetNode("bearypink");
        _playerDirection = _player.Rotation;
        _animTree = (AnimationTree)GetNode("bearypink/bearypink/AnimationTree");
        _jumpDelay = new Timer();
        _jumpDelay.WaitTime = 0.3F;
        _jumpDelay.OneShot = true;
        _jumpDelay.Connect("timeout", this, nameof(_on_jump_timeout));
        Input.SetMouseMode(Input.MouseMode.Captured);

    }

    public void _on_jump_timeout() 
    {
        _velocity.y = _jumpPower;
    }
    public override void _Process(float delta) 
    {
        if (Input.IsActionJustPressed("ui_cancel")){
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
    }
    public override void _Input(InputEvent @event) 
    {
        if (@event is InputEventMouseMotion inputmousemotion)
        {
            @event = (InputEventMouseMotion)@event;
            Vector3 newCamRot = _cameraPivot.RotationDegrees;
            newCamRot.x = Mathf.Clamp(_cameraPivot.RotationDegrees.x - inputmousemotion.Relative.y * _mouseSensitivity, _minPitch, _maxPitch);
            newCamRot.y -= inputmousemotion.Relative.x * _mouseSensitivity;
            _cameraPivot.RotationDegrees = newCamRot;


        } 
    }

    public override void _PhysicsProcess(float delta)
    {
        HandleMovement(delta);
    }

    private void HandleMovement(float delta) 
    {
        // This is supposed to remove the verticle rotation from the camera angle
        // doesn't seem to work and the workaround with player rotation will p
        /* Quat fixed_cam_rot = new Quat(camera_pivot.Transform.basis);
        fixed_cam_rot.x = 0;
        Basis char_direction_basis = new Basis(fixed_cam_rot);
         */
         Vector3 direction = new Vector3();
        float accel = _player.IsOnFloor() ? _acceleration : _airAcceleration;
        // change movement direction and save the most recent player orientation
        if (Input.IsActionPressed("up")) {
            direction -= _cameraPivot.Transform.basis.z;
            _playerDirection = direction;
        }
        if (Input.IsActionPressed("down")) {
            direction += _cameraPivot.Transform.basis.z;
            _playerDirection = direction;
        }
        if (Input.IsActionPressed("left")) {
            direction -= _cameraPivot.Transform.basis.x;
            _playerDirection = direction;
        }
        if (Input.IsActionPressed("right")) {
            //direction += cameria_pivot.Transform.basis.x;
            direction += _cameraPivot.Transform.basis.x;
            _playerDirection = direction;
        }
        if (!Input.IsActionPressed("up") && !Input.IsActionPressed("down") && !Input.IsActionPressed("left") && !Input.IsActionPressed("right"))
        {
            if (_player.IsOnFloor())
                _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), -1, delta * accel));
        }
        else 
        {
            // Toggle walk/run based on modifier key
            if (Input.IsActionPressed("movementmod"))
            {
                _moveSpeed = _walkSpeed;
                if (_player.IsOnFloor())
                {
                    _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), 0, delta * accel));
                }
            }
            else
            {
                _moveSpeed = _runSpeed;
                if (_player.IsOnFloor())
                {
                    _animTree.Set("parameters/iwr_blend/blend_amount", Mathf.Lerp((float)_animTree.Get("parameters/iwr_blend/blend_amount"), 1, delta * accel));
                }
                    
            }
        }
        direction = direction.Normalized();
        //_player.Rotation = new Vector3(0,Mathf.Atan2(_playerDirection.x, _playerDirection.z), 0);
        _player.Rotation = new Vector3(0, Mathf.LerpAngle(_player.Rotation.y, Mathf.Atan2(_playerDirection.x, _playerDirection.z), delta * _angularAcceleration), 0);
       
        
        if (direction != Vector3.Zero)
            _velocity = _velocity.LinearInterpolate(_player.Transform.basis.z * _moveSpeed, accel * delta);
        else
        {
            _velocity = _velocity.LinearInterpolate(direction * _moveSpeed, accel * delta);
        }
        _cameraPivot.Translation = new Vector3(_player.Translation.x, Mathf.Lerp(_cameraPivot.Translation.y, _player.Translation.y, 0.5F), _player.Translation.z);
        if (_player.IsOnFloor()) {
            _velocity.y = -0.01F;
            _animTree.Set("parameters/is_grounded/current", 0);
        }
        else {
            _velocity.y = Mathf.Clamp(_velocity.y - _gravity, -_maxTerminalVelocity, _maxTerminalVelocity+_jumpPower);
            _animTree.Set("parameters/is_grounded/current", 1);
        }

        if (Input.IsActionJustPressed("jump") && _player.IsOnFloor()) {
            _animTree.Set("parameters/jump/active", true);
            _jumpDelay.Start();
            // _velocity.y = _jumpPower;
        }
                
        if (Input.IsActionJustPressed("shoot") && _player.IsOnFloor()) {
            // to do
        }
        _velocity = _player.MoveAndSlide(_velocity, Vector3.Up);
    }
}
