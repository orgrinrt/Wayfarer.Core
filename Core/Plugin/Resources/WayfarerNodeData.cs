using Godot;
using Godot.Collections;
using Wayfarer.Core.Utils.Debug;

namespace Wayfarer.Core.Plugin.Resources
{
    public class WayfarerNodeData : Resource
    {
        [Export()] private string _nodeName;
        [Export()] private string _parentName;

        [Export()] private bool _isScene;
        [Export()] private Script _script;
        [Export()] private PackedScene _scene;
        
        [Export()] private string _description;
        [Export()] private Texture _icon;

        public string NodeName => _nodeName;
        public string ParentName => _parentName;
        public PackedScene Scene => _scene;
        public Texture Icon => _icon;

        public override void _Init()
        {
            base._Init();
            
            Log.Wayfarer("NodeData " + _nodeName + " was initialized", true);
        }

        public override Array _GetPropertyList()
        {
            Array source = base._GetPropertyList();

            foreach (Dictionary property in source)
            {
                if (property.TryGetValue("name", out object value))
                {
                    if (value is string s)
                    {
                        if (s == "_script")
                        {
                            if (_isScene)
                            {
                                Log.Wayfarer("Removed property " + s, true);
                                source.Remove(property);
                            } 
                        }
                        else if (s == "_scene")
                        {
                            if (!_isScene)
                            {
                                Log.Wayfarer("Removed property " + s, true);
                                source.Remove(property);
                            }
                        }
                        else
                        {
                            Log.Wayfarer("There was no property named _script or _scene (current: " + s, true);
                        }
                    }
                }
                else
                {
                    Log.Wayfarer("A property didn't have a name", true);
                }
            }

            foreach (Dictionary property in source)
            {
                if (property.TryGetValue("name", out object value))
                {
                    Log.Wayfarer("Property: " + value, true);
                }
            }
            return source;
        }
    }
}