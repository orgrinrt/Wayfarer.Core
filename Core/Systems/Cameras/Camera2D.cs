using Godot;
using Wayfarer.Core.Constants;

namespace Wayfarer.Core.Systems.Cameras
{
    public class Camera2D : Godot.Camera2D
    {
        public override void _Ready()
        {
            AddToGroup(Groups.Cameras);
        }
    }
}