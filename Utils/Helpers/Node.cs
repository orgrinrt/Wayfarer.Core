using System.Collections.Generic;
using Godot;

namespace Wayfarer.Utils.Helpers
{
    public static class Node
    {
        public static void ReParent(this Godot.Node child, Godot.Node newParent)
        {
            child.GetParent().RemoveChild(child);
            newParent.AddChild(child);
        }

        public static Godot.Node[] GetChildrenRecursive(this Godot.Node parent)
        {
            List<Godot.Node> result = new List<Godot.Node>();

            foreach (Godot.Node node in parent.GetChildren())
            {
                result.Add(node);

                if (node.GetChildCount() > 0)
                {
                    foreach (Godot.Node child in GetChildrenRecursive(node))
                    {
                        result.Add(child);
                    }
                }
            }

            return result.ToArray();
        }

        private static void GetAllNodes(Godot.Node parent)
        {
            // ? do we need this
        }

        public static T GetParentOfType<T>(this Godot.Node node) where T : Godot.Node
        {
            Godot.Node parentCandidate = node.GetParent();
            
            while (parentCandidate != null && parentCandidate != node.GetTree().GetRoot())
            {
                if (parentCandidate is T match)
                {
                    return match as T;
                }
                
                parentCandidate = parentCandidate.GetParent();
            }
            
            Debug.Log.Error("Helpers.GetParentOfType<T>()", "Couldn't find a parent with type " + typeof(T) + " to node " + node.Name, true);

            return null;
        }

        public static Theme GetThemeUsed(this Control control)
        {
            Control currentControl = control;
            
            while (currentControl != null)
            {
                if (currentControl.Theme != null)
                {
                    return  currentControl.Theme;
                }
                else
                {
                    currentControl = currentControl.GetParentOfType<Control>();
                }
            }

            Debug.Log.Error("Helpers.GetThemeUsed()", "Couldn't find a theme for control: " + control.Name, true);
            return null;
        }
    }
}