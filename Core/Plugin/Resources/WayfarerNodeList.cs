using Godot;
using Godot.Collections;

namespace Wayfarer.Core.Plugin.Resources
{
    public class WayfarerNodeList : Resource
    {
        [Export()] private Array<WayfarerNodeData> _nodes;

        public Array<WayfarerNodeData> Nodes => _nodes;
    }
}