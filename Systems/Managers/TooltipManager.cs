using Godot;

namespace Wayfarer.Systems.Managers
{
    public class TooltipManager : Manager
    {
        private CanvasLayer _canvas;

        public override void _Ready()
        {
            base._Ready();

            _canvas = (CanvasLayer) GetNode("./CanvasLayer");
        }
    }
}