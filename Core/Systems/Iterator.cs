using System;
using System.Data;
using Godot;
using Wayfarer.Core.Nodes;
using Wayfarer.Core.Utils.Coroutine;
using Wayfarer.Core.Utils.Debug;

namespace Wayfarer.Core.Systems
{
    #if TOOLS
    [Tool]
    #endif
    public class Iterator : WayfarerNode
    {
        private static int _currId = 100; // we'll reserve 0-99 to things that we might want to have as statically identified.. can't really say why, but it's here for the off-chance
        public static CoroutineHandler Coroutine;
        
        #if TOOLS
        private CoroutineHandler _editorCoroutine;
        public CoroutineHandler EditorCoroutine => GetEditorCoroutine();
        #endif
        
        public override void _EnterTree()
        {
            if (Coroutine == null)
            {
                Coroutine = new CoroutineHandler();
            }

            #if TOOLS
            
            if (_editorCoroutine != null)
            {
                _editorCoroutine.StopAll();
                _editorCoroutine = null;
            }
            _editorCoroutine = new CoroutineHandler();
            #endif
        }
/*
        public override void _ExitTree()
        {
            try
            {
                if (Coroutine != null)
                {
                    Coroutine.StopAll();
                    Coroutine = null;
                }
            }
            catch (Exception e)
            {
                Log.Error("Couldn't dispose of the Coroutine instance", e, true);
            }
            
            #if TOOLS
            try
            {
                _editorCoroutine.StopAll();
                _editorCoroutine = null;
            }
            catch (Exception e)
            {
                Log.Wf.Error("Couldn't stop and dispose the EditorCoroutine instance", e, true);
            }
            #endif
        }*/

        public override void _Process(float delta)
        {
            Coroutine.Update(delta);
            
            #if TOOLS
            if (_editorCoroutine != null)
            {
                _editorCoroutine.Update(delta);
            }
            else
            {
                GetEditorCoroutine();
            }
            #endif
        }

        public static int GetUniqueId()
        {
            _currId++;
            return _currId - 1;
        }

        #if TOOLS
        private CoroutineHandler GetEditorCoroutine()
        {
            if (_editorCoroutine == null)
            {
                _editorCoroutine = new CoroutineHandler();
            }

            return _editorCoroutine;
        }
        #endif
    }
}
