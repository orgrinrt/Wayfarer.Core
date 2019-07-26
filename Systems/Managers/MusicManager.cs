using Godot;
using Wayfarer.Core;

namespace Wayfarer.Systems.Managers
{
    public class MusicManager : Node
    {
        private Tween _tween;
    
        private AudioStreamPlayer _a;
        private AudioStreamPlayer _b;

        private AudioStream _menuMusic;
        private AudioStream _gamePlaceholder;

        private int _silenceDb = -86;

        private bool _isAPlaying = false;
        private GameContext _oldCtx = GameContext.Null;

        public override void _Ready()
        {
            _a = (AudioStreamPlayer) GetNode("./A");
            _b = (AudioStreamPlayer) GetNode("./B");

            _tween = (Tween) GetNode("./Tween");
        
            //_menuMusic = (AudioStream) ResourceLoader.Load("res://Assets/Audio/Music/virtutes_instrumenti03.ogg");
            //_gamePlaceholder = (AudioStream) ResourceLoader.Load("res://Assets/Audio/Music/MourningSong.ogg");
        }

        public void Init()
        {
            // TODO: This behaviour to signal system
            // Game.Game.GameStateChanged += UpdateCurrMusic;
        }

        public override void _PhysicsProcess(float delta)
        {
            //UpdateCurrMusic(Game.CurrGameStateContext);
        }

        private void UpdateCurrMusic(GameContext context)
        {
            if (context != _oldCtx)
            {
                switch (context)
                {
                    /*
                    case GameContext.MainMenu:
                        //_a.Stream = _menuMusic;
                        //_a.Play();
                        _oldCtx = GameContext.MainMenu;
                        CrossFade(_menuMusic, -8, 2);
                        break;
                    case GameContext.CampaignMap:
                        //_a.Stream = _gamePlaceholder;
                        //_a.Play();
                        _oldCtx = GameContext.CampaignMap;
                        CrossFade(_gamePlaceholder, -16, 5);
                        break;
                    case GameContext.MenuFadeoutToNewGame:
                        _oldCtx = GameContext.MenuFadeoutToNewGame;
                        FadeOut(7);
                        break;
                    */
                }
            }
        }

        private void FadeOut(float duration)
        {
            _tween.InterpolateProperty(_a, "volume_db", _a.VolumeDb, _silenceDb, duration, Tween.TransitionType.Sine,
                Tween.EaseType.InOut);
            _tween.InterpolateProperty(_b, "volume_db", _b.VolumeDb, _silenceDb, duration, Tween.TransitionType.Sine,
                Tween.EaseType.InOut);
        }

        private void CrossFade(AudioStream target, float targetDb, float duration)
        {
            if (_isAPlaying)
            {
                _b.Stream = target;
                _b.VolumeDb = _silenceDb;
                _b.Play();
            
                _tween.InterpolateProperty(_a, "volume_db", _a.VolumeDb, _silenceDb, duration, Tween.TransitionType.Sine,
                    Tween.EaseType.InOut);
                _tween.InterpolateProperty(_b, "volume_db", _b.VolumeDb, targetDb, duration, Tween.TransitionType.Sine,
                    Tween.EaseType.InOut);
                
                _isAPlaying = false;

            }
            else
            {
                _a.Stream = target;
                _a.VolumeDb = _silenceDb;
                _a.Play();
            
                _tween.InterpolateProperty(_b, "volume_db", _b.VolumeDb, _silenceDb, duration, Tween.TransitionType.Sine,
                    Tween.EaseType.InOut);
                _tween.InterpolateProperty(_a, "volume_db", _a.VolumeDb, targetDb, duration, Tween.TransitionType.Sine,
                    Tween.EaseType.InOut);
                
                _isAPlaying = true;

            }
        
            _tween.Start();
        }
    }
}
