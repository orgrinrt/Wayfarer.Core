#if TOOLS

using System;
using Godot;
using Wayfarer.Utils.Attributes;
using Wayfarer.Utils.Debug;
using Wayfarer.Utils.Helpers;
using Array = Godot.Collections.Array;

namespace Wayfarer.Core.Plugin
{
    [Tool]
    public class EditorMenuBar : Control
    {
        [Get("HBox/AddNodeMenu/Button")] private Button _addNodeButton;
    
        private Wayfarer.Core.Plugin.AddNodeDialog _addNodeDialog;
        private EditorInterface _editorInterface;

        public AddNodeDialog AddNodeDialog => _addNodeDialog;
    
        public override void _Ready()
        {
            this.SetupWayfarer();

            _addNodeButton.Connect("button_up", this, nameof(OnButtonPressed));
        
            PackedScene addNodeDialogScene = GD.Load<PackedScene>("res://Addons/Wayfarer/Assets/Scenes/Controls/AddNodeDialog.tscn");
            Node node = addNodeDialogScene.Instance();
            
            try
            {
                Wayfarer.Core.Plugin.AddNodeDialog dialog = (Wayfarer.Core.Plugin.AddNodeDialog)addNodeDialogScene.Instance();
            }
            catch (Exception e)
            {
                Log.Editor(e.Message, true);
            }
            
            //_addNodeDialog = (AddNodeDialog)addNodeDialogScene.Instance();
            AddChild(node);
            GD.Print(GetType());
            GD.Print(addNodeDialogScene.Instance().GetType());
            GD.Print(node.Name);
            GD.Print(node.GetType());
        
            try
            {
                _addNodeDialog = (AddNodeDialog)node;
            }
            catch (Exception e)
            {
                Log.Editor(e.Message, true);
            }
            GD.Print("Is not null: " + (_addNodeDialog != null));
            //GD.Print(_addNodeDialog.Name);
        
            //_editorInterface.GetBaseControl().AddChild(_addNodeDialog);

        }

        private void OnButtonPressed()
        {
            EditorSelection selection = _editorInterface.GetSelection();
            Array selectedNodes = selection.GetSelectedNodes();

            GD.Print(GetChildCount());
            GD.Print(_addNodeDialog.Name);
            //_addNodeDialog.Name = "Lol";
            //GD.Print(_addNodeDialog.Name);
            //_addNodeDialog.SetSelectedNodes(selectedNodes);
            //_addNodeDialog.Popup_();
        
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
    }
}

#endif