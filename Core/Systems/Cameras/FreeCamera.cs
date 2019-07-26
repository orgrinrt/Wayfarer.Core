using Godot;

namespace Wayfarer.Systems.Cameras
{
    public class FreeCamera : Camera
    {
        private int _panSpeed = 13;
        private float _zoomSpeed = 0.5f;
        private Vector2 _maxZoomIn = new Vector2(0.5f, 0.5f);
        private Vector2 _maxZoomOut = new Vector2(2.6f, 2.6f);
        
        public int PanSpeed => _panSpeed;
        public float ZoomSpeed => _zoomSpeed;

        private Tween _tween;
        
        public override void _Ready()
        {
            base._Ready();
            
            _tween = (Tween) GetNode("./Tween");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionPressed("cam_pan_right"))
            {
                Translate(new Vector2(PanSpeed, 0));
            }
            else if (Input.IsActionPressed("cam_pan_left"))
            {
                Translate(new Vector2(-PanSpeed, 0));
            }
            
            if (Input.IsActionPressed("cam_pan_up"))
            {
                Translate(new Vector2(0, -PanSpeed));
            }
            else if (Input.IsActionPressed("cam_pan_down"))
            {
                Translate(new Vector2(0, PanSpeed));
            }
        }

        public override void _UnhandledInput(InputEvent inputEvent)
        {
            base._Input(inputEvent);

            if (inputEvent.IsAction("cam_zoom_in"))
            {
                Vector2 target = new Vector2(
                    Mathf.Clamp(Zoom.x - ZoomSpeed, _maxZoomIn.x, _maxZoomOut.x),
                    Mathf.Clamp(Zoom.y - ZoomSpeed, _maxZoomIn.y, _maxZoomOut.y));
                _tween.InterpolateProperty(
                    this, 
                    "zoom", 
                    Zoom, 
                    target,
                    0.2f, 
                    Tween.TransitionType.Cubic, 
                    Tween.EaseType.Out);
                _tween.Start();
            }
            else if (inputEvent.IsAction("cam_zoom_out"))
            {
                Vector2 target = new Vector2(
                    Mathf.Clamp(Zoom.x + ZoomSpeed, _maxZoomIn.x, _maxZoomOut.x),
                    Mathf.Clamp(Zoom.y + ZoomSpeed, _maxZoomIn.y, _maxZoomOut.y));
                _tween.InterpolateProperty(
                    this, 
                    "zoom", 
                    Zoom, 
                    target,
                    0.2f, 
                    Tween.TransitionType.Cubic, 
                    Tween.EaseType.Out);
                _tween.Start();
            }
        }
    }
}
