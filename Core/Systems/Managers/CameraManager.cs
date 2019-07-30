using Godot;
using Godot.Collections;
using Wayfarer.Core.Constants;
using Wayfarer.Core.Utils.Debug;
using Camera = Wayfarer.Core.Systems.Cameras.Camera;

namespace Wayfarer.Core.Systems.Managers
{
    public class CameraManager : Manager
    {
        [Signal] public delegate void CurrentCameraChanged(Camera newCamera);
        
        private Camera _currCam;
        private Camera _prevCam;
        
        public Camera CurrCam => _currCam;
        public Camera PrevCam => _prevCam;

        public override void _Ready()
        {
            base._Ready();

            Game.Self.Connect(nameof(Game.StartedToChangeLevel), this, nameof(OnLevelChangeInitiated));
        }

        public void SetCurrentCam(Camera cam)
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
        
        public Array<Camera> GetCameras()
        {
            Array nodes = GetTree().GetNodesInGroup(Groups.Cameras);
            Array<Camera> casted = new Array<Camera>();
            foreach (Node node in nodes)
            {
                if (node is Camera cam)
                {
                    casted.Add(cam);
                }
            }

            return casted;
        }

        public Camera GetCamera(string name)
        {
            Array<Camera> cameras = GetCameras();
            foreach (Camera cam in cameras)
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
    }
}