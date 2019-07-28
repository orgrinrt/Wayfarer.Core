using System.Diagnostics;
using Godot;
using Wayfarer.Core.Constants;
using Wayfarer.Systems.Managers;
using Wayfarer.Utils.Debug;

namespace Wayfarer.Core
{
    public class Game : Node
    {
        // Level
        [Signal] public delegate void StartedToChangeLevel();
        
        // Game state
        [Signal] public delegate void GameStateChanged(GameContext newState);

        private string _hostName = "Testing";
        private GameState _state;
        private GameContext _currContext;
        
        public string HostName => _hostName;
        public GameState State => _state;
        public GameContext CurrContext => _currContext;
        /*
        private Directories _directories = new Directories();
        public Directories Directories => _directories;
        */

        private static Game _self;
        private static UserPrefs _userPrefs;
        private static SceneManager _sceneManager;
        private static AudioManager _audioManager;
        private static MusicManager _musicManager;
        private static SubtitleManager _subtitleManager;
        private static CameraManager _cameraManager;
        private static MouseManager _mouseManager;

        public static Game Self => _self;
        public static UserPrefs UserPrefs => _userPrefs;
        public static SceneManager SceneManager => _sceneManager;
        public static AudioManager AudioManager => _audioManager;
        public static MusicManager MusicManager => _musicManager;
        public static SubtitleManager SubtitleManager => _subtitleManager;
        public static CameraManager CameraManager => _cameraManager;
        public static MouseManager MouseManager => _mouseManager;
    
        public override void _Ready()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_self == null)
            {
                _self = this;
            }
            Log.Instantiate();
            Log.Print("Initializing game");
            
            SetReferencesToEssentialNodes();
        }

        public void LoadGame(string saveName)
        {
            State.Load(saveName);
        }

        public void SaveGame(string saveName)
        {
            State.Save(saveName);
        }

        public void SetState(GameState state)
        {
            _state = state;
        }
        
        /// <summary>
        /// Crashes the game and shows the "send debug info" dialog (WIP)
        /// </summary>
        public  void Crash()
        {
            Quit(true, true);
        }
        
        public void Quit(bool errorCrash = false, bool showSendDebugInfo = false)
        {
            GetTree().Quit();
            
            // TODO: Debugging behaviour
        }
        
        private void SetReferencesToEssentialNodes()
        {
            _sceneManager = (SceneManager) GetNode(NodePaths.SceneManager);
            _audioManager = (AudioManager) GetNode(NodePaths.AudioManager);
            _musicManager = (MusicManager) GetNode(NodePaths.MusicManager);
            _subtitleManager = (SubtitleManager) GetNode(NodePaths.SubtitleManager);
            _cameraManager = (CameraManager) GetNode(NodePaths.CameraManager);
            _mouseManager = (MouseManager) GetNode(NodePaths.MouseManager);
        }
    }
}
