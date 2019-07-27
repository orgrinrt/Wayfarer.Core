using Godot;

namespace Wayfarer.Core.Plugin
{
    public class AddNodeDialog : PopupDialog
    {
        [Export()] private string _test;
        
        private Godot.Collections.Array _selectedNodes;
    
        public override void _Ready()
        {
            GD.Print("omasta: " + this.GetType());
        }

        public void SetSelectedNodes(Godot.Collections.Array selectedNodes)
        {
            _selectedNodes = selectedNodes;
        }
    }
}
