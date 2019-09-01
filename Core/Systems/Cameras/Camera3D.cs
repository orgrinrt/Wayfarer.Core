using Wayfarer.Core.Constants;

namespace Wayfarer.Core.Systems.Cameras
{
    public class Camera3D : Godot.Camera
    {
        public override void _Ready()
        {
            AddToGroup(Groups.Cameras);
        }
    }
}