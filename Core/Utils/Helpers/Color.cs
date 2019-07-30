namespace Wayfarer.Core.Utils.Helpers
{
    public class Color
    {
        public static Godot.Color Red = new Godot.Color(0.5f, 0.0f, 0.0f);
        public static Godot.Color Blue = new Godot.Color(0.0f, 0.0f, 0.5f);
        public static Godot.Color Green = new Godot.Color(0.0f, 0.5f, 0.0f);
        public static Godot.Color Black = new Godot.Color(0.0f, 0.0f, 0.0f);
        public static Godot.Color White = new Godot.Color(1f, 1f, 1f);

        public static float GrayScale(Godot.Color source)
        {
            return (source.r + source.g + source.b) / 3;
        }
    }
}