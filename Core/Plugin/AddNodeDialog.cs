#if TOOLS

using Godot;
using Wayfarer.Utils.Debug;

namespace Wayfarer.Core.Plugin
{
    [Tool]
    public class AddNodeDialog : WindowDialog
    {
        [Export()] private string _test;
        
        private Godot.Collections.Array _selectedNodes;
    
        public override void _Ready()
        {
            Log.Editor("AddonDialog Ready " + this.GetType());
        }

        public void SetSelectedNodes(Godot.Collections.Array selectedNodes)
        {
            _selectedNodes = selectedNodes;
        }
    }
}

#endif