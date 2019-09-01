using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;
using Wayfarer.Core.Constants;
using Wayfarer.Utils.Debug;
using Camera2D = Wayfarer.Core.Systems.Cameras.Camera2D;

namespace Wayfarer.Core.Systems.Managers
{
    public class CameraManager : Manager
    {
        [Signal] public delegate void CurrentCameraChanged(Camera2D newCamera);
        
        private Camera2D _currCam;
        private Camera2D _prevCam;
        private bool _isMouseCaptured = false;
        
        public Camera2D CurrCam => _currCam;
        public Camera2D PrevCam => _prevCam;
        public bool IsMousecaptured => _isMouseCaptured;

        public override void _Ready()
        {
            base._Ready();

            Game.Self.Connect(nameof(Game.StartedToChangeLevel), this, nameof(OnLevelChangeInitiated));
            SetProcessInput(true);
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            
            if (@event.IsActionPressed("toggle_capture_mouse"))
            {
                Game.CameraManager.SetIsMouseCaptured(!Game.CameraManager.IsMousecaptured);
                Input.SetMouseMode(IsMousecaptured ? Input.MouseMode.Captured : Input.MouseMode.Visible);
            }
        }

        public void SetCurrentCam(Camera2D cam)
        {
            if (_currCam != null)
            {
                _prevCam = _currCam;
                _prevCam.Hide();
            }
            _currCam = cam;
            _currCam.Show();
            _currCam.MakeCurrent();
            EmitSignal(nameof(CurrentCameraChanged), _currCam);
        }
        
        public void SetCurrentCam(string camName)
        {
            if (_currCam != null)
            {
                _prevCam = _currCam;
                _prevCam.Hide();
            }
            _currCam = GetCamera(camName);
            _currCam.Show();
            _currCam.MakeCurrent();
            EmitSignal(nameof(CurrentCameraChanged), _currCam);
        }
        
        public Array<Camera2D> GetCameras()
        {
            Array nodes = GetTree().GetNodesInGroup(Groups.Cameras);
            Array<Camera2D> casted = new Array<Camera2D>();
            foreach (Node node in nodes)
            {
                if (node is Camera2D cam)
                {
                    casted.Add(cam);
                }
            }

            return casted;
        }

        public Camera2D GetCamera(string name)
        {
            Array<Camera2D> cameras = GetCameras();
            foreach (Camera2D cam in cameras)
            {
                if (cam.Name == name)
                {
                    return cam;
                }
                else continue;
            }

            Log.Error("Couldn't find specified (" + name + ") camera!", true);
            return null;
        }

        private void OnLevelChangeInitiated()
        {
            _prevCam = null;
            _currCam = null;
        }

        public void SetIsMouseCaptured(bool value)
        {
            _isMouseCaptured = value;
        }
    }
}