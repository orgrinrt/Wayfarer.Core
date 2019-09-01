using Godot;
using Wayfarer.Core.Systems.Managers;

namespace Wayfarer.Core.Systems.Cameras
{
    public class FreeCamera3D : Camera3D
    {
        [Export] private float _mouseSensitivity = 0.05f;
        private int _panSpeed = 13;
        private float _zoomSpeed = 0.5f;
        private int _invertXaxis = -1; // -1 = no invert, 1 = invert
        private int _invertYaxis = -1;
        private Spatial _head;
        private KinematicBody _observer;
        private RayCast _groundRay;
        
        protected float _acceleration = 7;
        protected float _slowDownRate = -16;
        protected float _gravity = -41;
        
        protected float _maxSpeed = 20;
        protected float _maxWalkSpeed = 10;
        protected float _maxSprintSpeed = 50;

        protected float _jumpStrength = 15;

        protected Vector3 _velocity = Vector3.Zero;
        protected Vector3 _dir = Vector3.Zero;
        
        public int PanSpeed => _panSpeed;
        public float ZoomSpeed => _zoomSpeed;
        public float MouseSensitivity => _mouseSensitivity;
        public Spatial Head => _head;
        public KinematicBody Observer => _observer;
        public RayCast GroundRay => _groundRay;

        private Tween _tween;
        
        public override void _Ready()
        {
            base._Ready();
            
            _tween = new Tween();
            AddChild(_tween);
            _head = GetParent<Spatial>();
            _observer = _head.GetParent<KinematicBody>();
            _groundRay = _observer.GetNode<RayCast>("GroundRay");
        }

        public override void _PhysicsProcess(float delta)
        {
            FlyingLocomotion(delta);
        }

        public override void _Input(InputEvent inputEvent)
        {
            base._Input(inputEvent);
            
            // TODO: We want to handle this on physicsProcess so that there's no "jankyness", for now it's a little janky and "shaky"

            if (Game.CameraManager.IsMousecaptured)
            {
                if (inputEvent is InputEventMouseMotion m)
                {
                    float diffY = (m.Relative.y * _invertYaxis) * MouseSensitivity;
                    float diffX = (m.Relative.x * _invertXaxis) * MouseSensitivity;
                    
                    Head.RotateY(Mathf.Deg2Rad(diffX));
                    
                    if (Head.RotationDegrees.x + diffY < 90 && Head.RotationDegrees.x + diffY > -90)
                    {
                        Head.Rotation = new Vector3(Head.Rotation.x + Mathf.Deg2Rad(diffY), Head.Rotation.y + Mathf.Deg2Rad(diffX), 0);
                    }
                    else
                    {
                        Head.Rotation = new Vector3(Head.Rotation.x, Head.Rotation.y + Mathf.Deg2Rad(diffX), 0);
                    }
                    
                }
            }
        }
        
        private void GroundedLocomotion(float delta)
        {
            _dir = Vector3.Zero;
            Basis aimDir = Head.GlobalTransform.basis;

            if (Input.IsActionPressed("move_front"))
            {
                _dir -= aimDir.GetColumn(2);
            }
            else if (Input.IsActionPressed("move_back"))
            {
                _dir += aimDir.GetColumn(2);
            }
            
            if (Input.IsActionPressed("move_left"))
            {
                _dir -= aimDir.GetColumn(0);
            }
            else if (Input.IsActionPressed("move_right"))
            {
                _dir += aimDir.GetColumn(0);
            }

            _dir = _dir.Normalized();
            _velocity.y += _gravity * delta;

            float targetSpeed = _maxSpeed;

            if (Input.IsActionPressed("walk"))
            {
                targetSpeed = _maxWalkSpeed;
            }
            else if (Input.IsActionPressed("sprint"))
            {
                targetSpeed = _maxSprintSpeed;
            }
            
            Vector3 targetVel = _dir * targetSpeed;
            Vector3 flatVel = Vector3.Zero;
            
            flatVel = _velocity;
            flatVel.y = 0;

            float currAccel = 0;
            if (_dir.Dot(flatVel) > 0)
            {
                currAccel = _acceleration;
            }
            else
            {
                currAccel = _slowDownRate;
            }

            flatVel = flatVel.LinearInterpolate(targetVel, _acceleration * delta);

            _velocity.x = flatVel.x;
            _velocity.z = flatVel.z;

            _velocity = Observer.MoveAndSlide(_velocity, Vector3.Up);

            // if ((_char.IsOnFloor() || _char.GroundRay.IsColliding()) && Input.IsActionJustPressed("jump"))
            if ((GroundRay.IsColliding()) && Input.IsActionJustPressed("jump"))
            {
                _velocity.y = _jumpStrength;
            }
        }

        private void FlyingLocomotion(float delta)
        {
            _dir = Vector3.Zero;

            Basis _aimDir = Head.GlobalTransform.basis;

            if (Input.IsActionPressed("move_front"))
            {
                _dir += _aimDir.GetAxis(2);
            }
            else if (Input.IsActionPressed("move_back"))
            {
                _dir -= _aimDir.GetAxis(2);
            }
            
            if (Input.IsActionPressed("move_left"))
            {
                _dir += _aimDir.GetAxis(0);
            }
            else if (Input.IsActionPressed("move_right"))
            {
                _dir -= _aimDir.GetAxis(0);
            }

            _dir = _dir.Normalized();
            
            float targetSpeed = _maxSpeed;
            
            if (Input.IsActionPressed("walk"))
            {
                targetSpeed = _maxWalkSpeed;
            }
            else if (Input.IsActionPressed("sprint"))
            {
                targetSpeed = _maxSprintSpeed;
            }

            Vector3 targetVel = _dir * (targetSpeed * 1.5f);

            _velocity = -_velocity.LinearInterpolate(targetVel, _acceleration * delta);

            Observer.MoveAndSlide(_velocity);
        }
    }
}