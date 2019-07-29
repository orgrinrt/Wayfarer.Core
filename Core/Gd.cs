using Godot;

namespace Wayfarer.Utils.Debug
{
    #if TOOLS
    [Tool]
    #endif
    public class Gd : Node
    {
        public void Print(string print, bool gdPrint = false)
        {
            Log.Print(print, gdPrint);
        }

        public void Test()
        {
            Log.Print("HAHAHA", true);
            
        }
    }
}