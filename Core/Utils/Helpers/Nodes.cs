﻿using System.Collections.Generic;
using Godot;
using Wayfarer.Utils.Attributes;

namespace Wayfarer.Utils.Helpers
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
            
            Debug.Log.Error("Couldn't find a parent with type " + typeof(T) + " to node " + node.Name, true);

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

            Debug.Log.Error("Couldn't find a theme for control: " + control.Name, true);
            return null;
        }
    }
}