using System.Linq;
using Godot;
using Godot.Collections;

namespace Wayfarer.Core.Plugin.Resources
{
    public class WayfarerNodeData : Resource
    {
        [Export()] private string _nodeName;
        [Export()] private string _parentName;
        [Export()] private bool _isAScene;
        [Export()] private PackedScene _scene;
        [Export()] private Texture _icon;

        public string NodeName => _nodeName;
        public string ParentName => _parentName;
        public PackedScene Scene => _scene;
        public Texture Icon => _icon;
    }
}