using System.Collections.Generic;
using Godot;
using Wayfarer.Core;

namespace Wayfarer.Systems.Managers
{
    public class AudioManager : Manager
    {
        private AudioStreamPlayer _speech;
        private AudioStreamPlayer _ambient;
        
        private Tween _tween;
        private float _fadeDuration = 0.5f;
        private int _normalDb = -12;
        private int _silenceDb = -86;
        private bool _fadeOutComplete = true;
        private bool _fadeInComplete = true;
        
        public override void _Ready()
        {
            base._Ready();

            _speech = (AudioStreamPlayer) GetNode("./Speech");
            _ambient = (AudioStreamPlayer) GetNode("./Ambient");
            _tween = (Tween) GetNode("./Tween");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_speech.Playing)
            {
                if (_speech.Stream != null)
                {
                    _speech.GetStream().Free();
                }
            }

            if (!_ambient.Playing)
            {
                if (_ambient.Stream != null)
                {
                    _ambient.GetStream().Free();
                }
            }
        }

        public void PlaySpeech(string txtKey)
        {
            int partCount = SpeechManager.TextPartCount(txtKey);
            int currPartIndex = 0;
            
            PlaySpeechPart(txtKey, currPartIndex, partCount);
        }

        private void PlaySpeechPart(string txtKey, int partIndex, int totalPartCount)
        {
            AudioStreamSample audio = ResourceLoader.Load<AudioStreamSample>(SpeechManager.TextPartAudioPath(txtKey, partIndex));
            if (!_speech.Playing)
            {
                _speech.SetStream(audio);
                _speech.Play();
            }
            else
            {
                Iterator.Coroutine.Run(FadeOutAndStartNewStream(_speech, audio));
            }
            
            if ((bool)Game.UserPrefs.GetUserPref(UserPrefCategory.Audio, "SHOW_SUBTITLES"))
            {
                Game.SubtitleManager.Play(SpeechManager.Text(txtKey, partIndex));
            }

            if (partIndex < (totalPartCount - 1))
            {
                Iterator.Coroutine.Run(WaitUntilSpeechFinishedThenPlayNextSpeechPart(txtKey, partIndex + 1, totalPartCount));
            }
        }

        private bool IsSpeechFinishedOrSilent()
        {
            if (!_speech.Playing)
            {
                return true;
            }
            else if (_speech.VolumeDb <= _silenceDb)
            {
                return true;
            }
            else return false;
        }

        private IEnumerator<float> WaitUntilSpeechFinishedThenPlayNextSpeechPart(string txtKey, int lastIndex, int totalPartCount)
        {
            while (true)
            {
                if (IsSpeechFinishedOrSilent())
                {
                    PlaySpeechPart(txtKey, lastIndex, totalPartCount);
                    break;
                }
                else yield return 0.05f;
            }
        }

        public void PlayAmbient(string audioPath)
        {
            AudioStreamSample audio = ResourceLoader.Load<AudioStreamSample>(audioPath);
            if (!_ambient.Playing)
            {
                _ambient.SetStream(audio);
                _ambient.Play();
            }
            else
            {
                Iterator.Coroutine.Run(FadeOutAndStartNewStream(_ambient, audio));
            }
        }

        private IEnumerator<float> FadeOutAndStartNewStream(AudioStreamPlayer player, AudioStream targetStream)
        {
            bool firstIteration = true;
            bool secondIteration = true;
            while (true)
            {
                if (firstIteration)
                {
                    _fadeOutComplete = false;
                    firstIteration = false;
                    FadeOut(player, player.Stream);
                }
                else if (_fadeOutComplete && secondIteration)
                {
                    _fadeInComplete = false;
                    secondIteration = false;
                    FadeIn(player, targetStream);
                }
                else if (_fadeInComplete && !secondIteration)
                {
                    _tween.StopAll();
                    break;
                }
                else yield return 0.1f;
            }
        }
        
        private void FadeOut(AudioStreamPlayer player, AudioStream stream)
        {
            _tween.InterpolateProperty(player, "volume_db", player.VolumeDb, _silenceDb, _fadeDuration, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            _tween.Connect("tween_completed", this, nameof(OnFadeOutCompleted));
            _tween.Start();
            
        }
        
        private void FadeIn(AudioStreamPlayer player, AudioStream stream)
        {
            player.SetVolumeDb(_silenceDb);
            player.SetStream(stream);
            
            _tween.InterpolateProperty(player, "volume_db", player.VolumeDb, _normalDb, _fadeDuration, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            _tween.Connect("tween_completed", this, nameof(OnFadeInCompleted));    
            _tween.Start();
        }

        private void OnFadeOutCompleted(Object Object, NodePath nodePath)
        {
            _fadeOutComplete = true;
        }

        private void OnFadeInCompleted(Object Object, NodePath nodePath)
        {
            _fadeInComplete = true;
        }
    }
}