#if TOOLS

using System;
using Godot;
using Wayfarer.ModuleSystem;
using Wayfarer.Utils.Debug;
using Wayfarer.Utils.Files;
using Texture = Godot.Texture;

namespace Wayfarer.Core
{
    [Tool]
    public class WayfarerCorePlugin : WayfarerModule
    {
        public EditorInterface EditorInterface => GetEditorInterface();

        public override void _EnterTreeSafe()
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
                WayfarerProjectSettings.Initialize();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't initialize Directories (static)", e, true);
            }

            try
            {
                Files.SetPlugin(this);
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't set plugin to Files static", e, true);
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

        public override void _ExitTreeSafe()
        {
            try
            {
                RemoveAutoLoads();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't remove AutoLoads", e, true);
            }
            
            try
            {
                RemoveCustomTypes();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't remove CustomTypes", e, true);
            }

            try
            {
                RemoveCustomResources();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't remove CustomResources", e, true);
            }
            
            try
            {
                RemoveCustomControlsFromEditor();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't remove Custom Controls from Editor", e, true);
            }
        }

        public override void _Notification(int what)
        {
            if (what is MainLoop.NotificationCrash)
            {
                Log.Wf.Immediate("Crash noticed by WayfarerCorePlugin");
            }
        }

        private void AddAutoLoads()
        {
            
        }

        private void AddCustomTypes()
        {
            Script managerScript = GD.Load<Script>("res://Addons/Wayfarer.Core/Core/Systems/Managers/Manager.cs");
            Texture managerIcon = GD.Load<Texture>("res://Addons/Wayfarer.Core/Assets/Icons/manager.png");
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