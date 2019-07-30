using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Wayfarer.Core.Nodes;
using Wayfarer.Core.Utils.Attributes;
using Wayfarer.Core.Utils.Debug;

namespace Wayfarer.Core.Utils.Helpers
{
    public static class Nodes
    {
        public static void SetupWayfarer(this Godot.Node self)
        {
            self.SetupAttributes();

            if (self is WayfarerNode)
            {
                // further stuff that only concerns the WayfarerNodes
            }
            // what else? if nothing else, we should just directly call SetupAttributes
        }
        
        public static T GetNode<T>(this Godot.Node self, string path) where T : Godot.Node
        {
            return self.GetNode(path) as T;
        }
        
        public static T GetNode<T>(this Godot.Node self, NodePath path) where T : Godot.Node
        {
            return self.GetNode(path) as T;
        }
        
        public static T FindNode<T>(this Godot.Node self, string mask, bool recursive = true, bool owned = true) where T : Godot.Node
        {
            return self.FindNode(mask, recursive, owned) as T;
        }
        
        public static T GetChild<T>(this Godot.Node self, int idx) where T : Godot.Node
        {
            return self.GetChild(idx) as T;
        }

        public static T GetChildOfType<T>(this Node self) where T : Node
        {
            Node[] children = self.GetChildrenRecursive();

            foreach (Node node in children)
            {
                if (node is T)
                {
                    return node as T;
                }
            }

            return null;
        }

        public static T[] GetChildrenOfType<T>(this Node self) where T : Node // we might consider using Godot.Array instead of T[]
        {
            Node[] children = self.GetChildrenRecursive();
            List<T> matches = new List<T>();

            foreach (Node node in children)
            {
                if (node is T t)
                {
                    matches.Add(t);
                }
            }

            return matches.ToArray();
        }
        
        public static void ReParent(this Godot.Node child, Godot.Node newParent)
        {
            // NOTE: Currently it seems to do it so that it fires _EnterTree and _Ready again when its reparented...
            // maybe we don't want that?
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
            
            Log.Error("Couldn't find a parent with type " + typeof(T) + " to node " + node.Name, true);

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

            Log.Error("Couldn't find a theme for control: " + control.Name, true);
            return null;
        }
    }
}