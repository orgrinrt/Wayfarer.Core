#if TOOLS

using System;
using Godot;
using Wayfarer.Core.Plugin;
using Wayfarer.Utils.Debug;
using Wayfarer.Utils.Helpers;
using Texture = Godot.Texture;

namespace Wayfarer
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        private EditorMenuBar _toolbar;
        public EditorInterface EditorInterface => GetEditorInterface();

        public override void _EnterTree()
        {
            AddCustomTypes();
            AddCustomResources();
            AddCustomControlsToEditor();
        }

        public override void _ExitTree()
        {
            RemoveCustomTypes();
            RemoveCustomResources();
            RemoveCustomControlsFromEditor();
        }

        public override void DisablePlugin()
        {
            base.DisablePlugin();

            RemoveOldEditorMenubar();
        }

        private void AddCustomTypes()
        {
            Script wnScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Nodes/WayfarerNode.cs");
            Texture wnIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/wayfarer.png");
            AddCustomType("WayfarerNode", "Node", wnScript, wnIcon);

            Script managerScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Systems/Managers/Manager.cs");
            Texture managerIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/manager.png");
            AddCustomType("Manager", "Node", managerScript, managerIcon);
        }

        private void AddCustomResources()
        {
            Script nodeScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Plugin/Resources/WayfarerNodeData.cs");
            Texture nodeIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/resource.png");
            AddCustomType("WayfarerNodeData", "Resource", nodeScript, nodeIcon);

            Script nodeListScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Plugin/Resources/WayfarerNodeList.cs");
            Texture nodeListIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/wayfarerNodes.png");
            AddCustomType("WayfarerNodeList", "Resource", nodeListScript, nodeListIcon);
        }

        private void AddCustomControlsToEditor()
        {
            RemoveOldEditorMenubar();
            
            PackedScene toolbarScene = GD.Load<PackedScene>("res://Addons/Wayfarer/Assets/Scenes/Controls/EditorMenuBar.tscn");
            _toolbar = (EditorMenuBar)toolbarScene.Instance();
            _toolbar.SetEditorInterface(GetEditorInterface());
            
            AddControlToContainer(CustomControlContainer.CanvasEditorMenu, _toolbar);
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

        private void RemoveCustomTypes()
        {
            RemoveCustomType("WayfarerNode");
            RemoveCustomType("Manager");
        }

        private void RemoveCustomResources()
        {
            RemoveCustomType("WayfarerNodeData");
            RemoveCustomType("WayfarerNodeList");
        }

        private void RemoveCustomControlsFromEditor()
        {
            RemoveControlFromContainer(CustomControlContainer.CanvasEditorMenu, _toolbar);
        }
    }
}

#endif