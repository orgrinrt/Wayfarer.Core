using System;
using System.Collections.Generic;
using Godot;
using Wayfarer.Core.UI.Screens;
using Wayfarer.Utils.Debug;

namespace Wayfarer.Core.Systems.Managers
{
    public class SceneManager : Manager
    {
        [Signal] public delegate void SceneChanged(Node newSceneRoot);
        
        private ResourceInteractiveLoader _loader;
        private int _maxFrameTime = 20; //ms
        
        public override void _Ready()
        {
            
        }

        /// <summary>
        /// Changes the whole Main scene to a PackedScene in a specified path
        /// </summary>
        /// <param name="path"></param>
        public void ChangeScene(string path)
        {
            Game.Self.EmitSignal(nameof(Game.StartedToChangeLevel));
            
            _loader = ResourceLoader.LoadInteractive(path);
            
            if (_loader == null || path == "res://")
            {
                Log.Crash("Couldn't load a scene: Loader at path " + path + " was NULL", true);
            }
            
            GetLoadScreen().StartLoadingVisuals(0.5f);

            Log.Print("Changing scene to: " + path, true);
            Iterator.Coroutine.Run(LoadSceneAndMakeItMainScene());
        }
        
        /// <summary>
        /// Changes the whole Main scene to a PackedScene in a specified path
        /// </summary>
        /// <param name="path"></param>
        public void ChangeScene(PackedScene scene)
        {
            string path = scene.ResourcePath;
            
            Game.Self.EmitSignal(nameof(Game.StartedToChangeLevel));
            
            _loader = ResourceLoader.LoadInteractive(path);
            
            if (_loader == null || path == "res://")
            {
                Log.Crash("Couldn't load a scene: Loader at path " + path + " was NULL", true);
            }
            
            GetLoadScreen().StartLoadingVisuals(0.5f);

            Log.Print("Changing scene to: " + path, true);
            Iterator.Coroutine.Run(LoadSceneAndMakeItMainScene());
        }

        /// <summary>
        /// Interactively loads a packedScene and in the end _replaces_ the whole Main scene to the loaded scene
        /// NOTE: This method doesn't simply load 
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> LoadSceneAndMakeItMainScene()
        {
            while (true)
            {
                if (_loader == null)
                {
                    if (Math.Abs(GetLoadScreen().GetProgressBar().Value - 1f) < 0.01f)
                    {
                        break;
                    }
                    else if (Log.LoggingLevel > LoggingLevel.None)
                    {
                        Log.Error("Loader was NULL, couldn't load the scene", true);
                    }

                    break;
                }
                
                int startTicksMs = OS.GetTicksMsec();

                while (OS.GetTicksMsec() < startTicksMs + _maxFrameTime)
                {
                    if (_loader != null)
                    {
                        Error error = _loader.Poll();

                        if (error == Error.FileEof)
                        {
                            PackedScene scene = (PackedScene) _loader.GetResource();
                            if (Log.LoggingLevel > LoggingLevel.Default)
                            {
                                Log.Print("FileEOF, loaded resource = " + scene.ResourceName + " (" + scene.ResourcePath + ")", true);
                            }
                            GetTree().ChangeSceneTo(scene);
                            FinishProgress();
                            break;
                        }
                        else if (error == Error.Ok)
                        {
                            UpdateProgess();
                        }
                        else // Means there was an error while loading
                        {
                            if (Log.LoggingLevel > LoggingLevel.None)
                            {
                                Log.Crash("Error while loading a scene (" + error + ") at stage: " + _loader.GetStage() + "/" + _loader.GetStageCount(), true);
                            }

                            FailLoading();
                            break;
                        }
                    }
                    else
                    {
                        if (Log.LoggingLevel > LoggingLevel.None)
                        {
                            Log.Error("Loader was flushed in middle of interactive loading ... ?", true);
                        }

                        break;
                    }
                }

                yield return 0.0005f;
            }
        }

        private void UpdateProgess()
        {
            if (Log.LoggingLevel == LoggingLevel.All)
            {
                Log.Print("Polling scene loader stage: " + _loader.GetStage() + "/" + _loader.GetStageCount());
            }
            
            float progress = (float)_loader.GetStage() / _loader.GetStageCount();
            //GD.Print("!!!!!!!!!!!!!! " + (float)_loader.GetStage() + " / " + _loader.GetStageCount());

            GetLoadScreen().GetProgressBar().Value = progress;
        }

        private void FinishProgress()
        {
            if (Log.LoggingLevel == LoggingLevel.All)
            {
                Log.Print("Polling scene loader stage: FINISHED");
            }
            
            EmitSignal(nameof(SceneChanged), GetTree().CurrentScene); // NOTE: this might return dated info (the last level instead of this new one) because it may take time to changeSceneTo
            _loader = null;
            GetLoadScreen().GetProgressBar().Value = 1f;
        }

        private void FailLoading()
        {
            _loader = null;
        }

        private LoadScreen GetLoadScreen()
        {
            return (LoadScreen) GetNode("./CanvasLayer/LoadScreen"); 
        }

        public Node GetCurrentMainSceneAsNode()
        {
            return GetTree().GetRoot().GetChild(GetTree().GetRoot().GetChildCount() - 1);
        }
    }
}
