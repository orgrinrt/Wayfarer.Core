#if TOOLS

using System;
using Godot;
using Wayfarer.Core.Plugin;
using Wayfarer.Core.Utils.Debug;
using Wayfarer.Core.Utils.Helpers;
using Texture = Godot.Texture;

namespace Wayfarer
{
    [Tool]
    public class WayfarerPluginManager : EditorPlugin
    {
        private EditorMenuBar _editorMenuBar;
        public EditorInterface EditorInterface => GetEditorInterface();

        public override void _EnterTree()
        {
            Log.Instantiate();
            AddAutoLoads();
            AddCustomTypes();
            AddCustomResources();
            AddCustomControlsToEditor();
        }

        public override void _ExitTree()
        {
            RemoveAutoLoads();
            RemoveCustomTypes();
            RemoveCustomResources();
            RemoveCustomControlsFromEditor();
        }

        public override void DisablePlugin()
        {
            base.DisablePlugin();

            try
            {
                RemoveOldEditorMenubar();
            }
            catch (Exception e)
            {
                Log.Editor("Couldn't remove the OLD EditorMenuBar", e, true);
            }
        }

        private void AddAutoLoads()
        {
            AddAutoloadSingleton("CS", "res://Addons/Wayfarer/Core/Gd.cs");
            AddAutoloadSingleton("Log", "res://Addons/Wayfarer/GDInterfaces/log.gd");
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

            Script nodeListScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Plugin/Resources/WayfarerNodeDatabase.cs");
            Texture nodeListIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/wayfarerNodes.png");
            AddCustomType("WayfarerNodeDatabase", "Resource", nodeListScript, nodeListIcon);
        }

        private void AddCustomControlsToEditor()
        {
            
            PackedScene toolbarScene = GD.Load<PackedScene>("res://Addons/Wayfarer/Assets/Scenes/Controls/EditorMenuBar.tscn");
            _editorMenuBar = (EditorMenuBar)toolbarScene.Instance();
            _editorMenuBar.SetEditorInterface(GetEditorInterface());
            _editorMenuBar.SetPluginManager(this);
            AddControlToContainer(CustomControlContainer.CanvasEditorMenu, _editorMenuBar);
        }

        private void RemoveAutoLoads()
        {
            RemoveAutoloadSingleton("CS");
            RemoveAutoloadSingleton("Log");
        }

        private void RemoveOldEditorMenubar()
        {
            Node[] editorNodes = EditorInterface.GetBaseControl().GetChildrenRecursive();

            foreach (Node node in editorNodes)
            {
                if (node is EditorMenuBar)
                {
                    try
                    {
                        RemoveControlFromContainer(CustomControlContainer.CanvasEditorMenu, node as Control);
                    }
                    catch (Exception e)
                    {
                        Log.Editor("Tried to remove EditorMenuBar from Toolbar, but couldn't", e, true);
                    }
                    try
                    {
                        node.QueueFree();
                        Log.Editor("Removed old EditorMenuBar (QueueFree)", true);
                    }
                    catch (Exception e)
                    {
                        Log.Editor("Tried to QueueFree() EditorMenuBar from Toolbar, but couldn't", e, true);
                    }
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
            RemoveCustomType("WayfarerNodeDatabase");
        }

        private void RemoveCustomControlsFromEditor()
        {
            try
            {
                RemoveControlFromContainer(CustomControlContainer.CanvasEditorMenu, _editorMenuBar);

                _editorMenuBar.QueueFree();
            }
            catch (Exception e)
            {
                Log.Editor("Couldn't remove the EditorMenuBar", e, true);
            }
            
        }
    }
}

#endif