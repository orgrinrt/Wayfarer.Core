#if TOOLS

using System;
using Godot;
using Wayfarer.Core.Utils.Debug;
using Wayfarer.Core.Utils.Files;
using Wayfarer.Core.Utils.Helpers;
using Texture = Godot.Texture;

namespace Wayfarer
{
    [Tool]
    public class WayfarerPluginManager : EditorPlugin
    {
        public EditorInterface EditorInterface => GetEditorInterface();

        public override void _EnterTree()
        {
            try
            {
                Log.Initialize();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't initialize Log (static)", e, true);
            }

            try
            {
                Directories.Initialize();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't initialize Directories (static)", e, true);
            }

            try
            {
                AddAutoLoads();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("SOMETHING BAD HAPPEND BRUH (AutoLoads)", e, true);
            }
            
            try
            {
                AddCustomTypes();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("SOMETHING BAD HAPPEND BRUH (AddCustomTypes)", e, true);
            }
            
            try
            {
                AddCustomResources();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("SOMETHING BAD HAPPEND BRUH (AddCustomResources)", e, true);
            }
            
            try
            {
                AddCustomControlsToEditor();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("SOMETHING BAD HAPPEND BRUH (AddCustomControls)", e, true);
            }
        }

        public override void _ExitTree()
        {
            RemoveAutoLoads();
            RemoveCustomTypes();
            RemoveCustomResources();
            RemoveCustomControlsFromEditor();
        }

        private void AddAutoLoads()
        {
            
        }

        private void AddCustomTypes()
        {
            Script wnScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Nodes/WayfarerNode.cs");
            Texture wnIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/wayfarer.png");
            if (wnScript != null)
            {
                if (wnIcon != null)
                {
                    AddCustomType("WayfarerNode", "Node", wnScript, wnIcon);
                }
                else
                {
                    Texture icon = GD.Load<Texture>("res://icon.png");
                    AddCustomType("WayfarerNode", "Node", wnScript, icon);
                }
            }
            

            Script managerScript = GD.Load<Script>("res://Addons/Wayfarer/Core/Systems/Managers/Manager.cs");
            Texture managerIcon = GD.Load<Texture>("res://Addons/Wayfarer/Assets/Icons/manager.png");
            if (managerScript != null)
            {
                if (managerIcon != null)
                {
                    AddCustomType("Manager", "Node", managerScript, managerIcon);
                }
                else
                {
                    Texture icon = GD.Load<Texture>("res://icon.png");
                    AddCustomType("Manager", "Node", managerScript, icon);
                }
            }
            
        }

        private void AddCustomResources()
        {
            
        }

        private void AddCustomControlsToEditor()
        {
            
        }

        private void RemoveAutoLoads()
        {
            
        }

        private void RemoveCustomTypes()
        {
            RemoveCustomType("WayfarerNode");
            RemoveCustomType("Manager");
        }

        private void RemoveCustomResources()
        {
            
        }

        private void RemoveCustomControlsFromEditor()
        {
            
        }
    }
}

#endif