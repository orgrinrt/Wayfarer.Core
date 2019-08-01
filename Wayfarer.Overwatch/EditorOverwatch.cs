#if TOOLS

using Godot;
using Wayfarer.Utils.Debug;
using Wayfarer.Utils.Helpers;

namespace Wayfarer.Core.Plugin
{
    [Tool]
    public class EditorOverwatch : EditorPlugin
    {
        public EditorInterface EditorInterface => GetEditorInterface();
        
        public override bool Build()
        {
            if (!EditorInterface.IsPluginEnabled("Wayfarer"))
            {
                Log.Editor("Solution was built, so removing the old EditorMenuBar...", true);
                RemoveOldEditorMenubar();
            }
            
            return base.Build();
        }
        
        private void RemoveOldEditorMenubar()
        {
            Node[] editorNodes = EditorInterface.GetBaseControl().GetChildrenRecursive();

            foreach (Node node in editorNodes)
            {
                if (node is EditorMenuBar)
                {
                    RemoveControlFromContainer(CustomControlContainer.CanvasEditorMenu, node as Control);
                    node.QueueFree();
                    Log.Editor("Removed old EditorMenuBar", true);
                    return;
                }
            }
        }
    }
    
    
}

#endif