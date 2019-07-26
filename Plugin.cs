#if TOOLS

using Godot;

namespace Wayfarer
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        private EditorMenuBar _toolbar;
        public EditorInterface EditorInterface => GetEditorInterface();
        
        public override void _EnterTree()
        {
            Script wnScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Nodes/WayfarerNode.cs");
            Texture wnIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/wayfarer.png");
            AddCustomType("WayfarerNode", "Node", wnScript, wnIcon);

            Script managerScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Systems/Managers/Manager.cs");
            Texture managerIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/manager.png");
            AddCustomType("Manager", "WayfarerNode", managerScript, managerIcon);

            PackedScene toolbarScene = GD.Load<PackedScene>("res://Addons/Wayfarer/Assets/Scenes/Controls/EditorMenuBar.tscn");
            _toolbar = (EditorMenuBar)toolbarScene.Instance();
            _toolbar.SetEditorInterface(GetEditorInterface());
            
            AddControlToContainer(CustomControlContainer.CanvasEditorMenu, _toolbar);
        }

        public override void _ExitTree()
        {
            RemoveCustomType("WayfarerNode");
            RemoveCustomType("Manager");
            
            RemoveControlFromContainer(CustomControlContainer.CanvasEditorMenu, _toolbar);
        }
    }
}

#endif