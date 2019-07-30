using Godot;
using Godot.Collections;

namespace Wayfarer.Core.Utils.Helpers
{
    public static class Arrays
    {
        public static Array ToArray(this object[] array)
        {
            Array result = new Array();
            
            foreach (object item in array)
            {
                result.Add(item);
            }

            return result;
        }

        public static Array ToArray(this Node[] nodes)
        {
            Array result = new Array();
            
            foreach (Node node in nodes)
            {
                result.Add(node);
            }

            return result;
        }
    }
}