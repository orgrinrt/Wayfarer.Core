using Godot;

namespace Wayfarer.Core.Systems.Managers
{
    public class SubtitleManager : Manager
    {
        private CanvasLayer _canvas;

        public override void _Ready()
        {
            base._Ready();

            _canvas = (CanvasLayer) GetNode("./CanvasLayer");
        }

        public void Play(string textToShow)
        {
            // TODO: Show subtitle in the subtitle area
        }
    }
}