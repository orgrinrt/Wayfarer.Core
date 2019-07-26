using System;
using Godot;

namespace Wayfarer.UI.Screens
{
    public class LoadScreen : Control
    {
        private Tween _tween;

        private bool _loading = false;
    
        public override void _Ready()
        {
            _tween = (Tween) GetNode("./Tween");
            
            Show();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_loading && Math.Abs(GetProgressBar().Value - 1f) < 0.01f)
            {
                EndLoadingVisuals(0.8f);
            }
        }

        public void StartLoadingVisuals(float time)
        {
            _loading = true;
            _tween.StopAll();
            _tween.InterpolateProperty(this, "modulate", new Color(Modulate.r, Modulate.g, Modulate.b, 0), new Color(Modulate.r, Modulate.g, Modulate.b, 1), time,
                Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            _tween.Start();
        }

        public void EndLoadingVisuals(float time)
        {
            _loading = false;
            _tween.StopAll();
            _tween.InterpolateProperty(this, "modulate", new Color(Modulate.r, Modulate.g, Modulate.b, 1), new Color(Modulate.r, Modulate.g, Modulate.b, 0), time,
                Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            _tween.Start();
        }

        public ProgressBar GetProgressBar()
        {
            return (ProgressBar) GetNode("./ProgressBar");
        }
        
        
    
    }
}
