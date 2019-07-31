using Godot;

namespace Wayfarer.Core.Utils.Helpers
{
    public static class Textures
    {
        public static Vector2 TexturePixelSizeToScale(Vector2 sourceSize, int targetHeight, int targetWidth)
        {
            return new Vector2(sourceSize.x / (sourceSize.x / targetWidth) / 250, (sourceSize.y / (sourceSize.y / targetHeight)) / 250);
        }
    }
}