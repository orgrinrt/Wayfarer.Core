//#if TOOLS

using System;
using Godot;

namespace Wayfarer
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        public override void _EnterTree()
        {
            Script script = GD.Load<Script>("res://Addons/Wayfarer/WayfarerNode.cs");
            Texture icon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/wayfarer.png");
            AddCustomType("WayfarerNode", "Node", script, icon);
        }

        public override void _ExitTree()
        {
            RemoveCustomType("WayfarerNode");
        }
    }
}
//#endif