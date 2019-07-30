﻿using Godot;
using Godot.Collections;

namespace Shardwielders.Addons.Wayfarer.Core.Plugin.Resources
{
    public class WayfarerNodeDatabase : Resource
    {
        [Export()] private Array<WayfarerNodeData> _all;

        public Array<WayfarerNodeData> All => _all;
    }
}