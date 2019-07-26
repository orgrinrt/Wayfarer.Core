#if TOOLS

using Godot;
using System;
using Wayfarer.Utils.Attributes;
using Wayfarer.Utils.Helpers;
using Array = Godot.Collections.Array;

[Tool]
public class EditorMenuBar : Control
{
    [Get("HBox/AddNodeMenu/Button")] private Button _addNodeButton;

    private EditorInterface _editorInterface;
    
    public override void _Ready()
    {
        this.SetupWayfarer();

        _addNodeButton.Connect("button_up", this, nameof(OnButtonPressed));
    }

    private void OnButtonPressed()
    {
        EditorSelection selection = _editorInterface.GetSelection();
        Array selectedNodes = selection.GetSelectedNodes();

        foreach (Node node in selectedNodes)
        {
            GD.Print(node.Name);
        }
        // open pop-up to select node to create
    }
    
    public void SetEditorInterface(EditorInterface editor)
    {
        _editorInterface = editor;
    }
}

#endif