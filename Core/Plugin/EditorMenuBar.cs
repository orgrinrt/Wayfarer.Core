#if TOOLS

using Godot;
using Wayfarer.Core.Utils.Attributes;
using Wayfarer.Core.Utils.Debug;
using Wayfarer.Core.Utils.Helpers;
using Array = Godot.Collections.Array;

namespace Wayfarer.Core.Plugin
{
    [Tool]
    public class EditorMenuBar : Control
    {
        [Get("HBox/AddNodeMenu/Button")] private Button _addNodeButton;
    
        private global::Wayfarer.Core.Plugin.AddNodeDialog _addNodeDialog;
        private EditorInterface _editorInterface;
        private WayfarerPluginManager _pluginManager;

        public global::Wayfarer.Core.Plugin.AddNodeDialog AddNodeDialog => _addNodeDialog;
    
        public override void _Ready()
        {
            this.SetupWayfarer();

            _addNodeButton.Connect("button_up", this, nameof(OnButtonPressed));

            PackedScene addNodeDialogScene = GD.Load<PackedScene>("res://Addons/Wayfarer/Assets/Scenes/Controls/AddNodeDialog.tscn");
            _addNodeDialog = (global::Wayfarer.Core.Plugin.AddNodeDialog)addNodeDialogScene.Instance();
            AddChild(_addNodeDialog);
            
        }
        

        public override void _ExitTree()
        {
            Log.Editor("Going to QueueFree() the AddNodeDialog", true);
            global::Wayfarer.Core.Plugin.AddNodeDialog dialog = this.GetChildOfType<global::Wayfarer.Core.Plugin.AddNodeDialog>();
            dialog.QueueFree();
        }
        
        private void OnButtonPressed()
        {
            EditorSelection selection = _editorInterface.GetSelection();
            Array selectedNodes = selection.GetSelectedNodes();

            _addNodeDialog.SetSelectedNodes(selectedNodes);
            _addNodeDialog.Popup_();
        
            foreach (Node node in selectedNodes)
            {
                // pass in the selectedNodes to the popup
            }
            // open pop-up to select node to create
        }
    
        public void SetEditorInterface(EditorInterface editor)
        {
            _editorInterface = editor;
        }
        
        public void SetPluginManager(WayfarerPluginManager manager)
        {
            _pluginManager = manager;
        }
    }
}

#endif