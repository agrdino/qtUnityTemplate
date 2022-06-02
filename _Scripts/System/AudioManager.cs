using System.Collections.Generic;
using _Scripts.Helper;
using UnityEngine;
using static _Scripts.Helper.qtLogging;

namespace _Scripts.System
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : qtSingleton<AudioManager>
    {
        public const string BackgroundVolumeKey = "BackgroundVolume";
        public const string SFXVolumeKey = "SFXVolume";
        
        private AudioSource _ausBackground;
        private List<AudioSource> _ausSFX;
        private Dictionary<string, AudioClip> _sfxClip;

        private void Start()
        {
            _ausBackground = GetComponent<AudioSource>();
            _ausSFX = new List<AudioSource>();

            _ausBackground.volume = PlayerPrefs.GetFloat(BackgroundVolumeKey, 1);
        }

        public float backgroundVolume
        {
            get => _ausBackground.volume;
            set
            {
                var x = Mathf.Clamp(value, 0, 1);
                _ausBackground.volume = x;
                PlayerPrefs.SetFloat(BackgroundVolumeKey, x);
            }
        }

        public float sfxVolume
        {
            get
            {
                if (_ausSFX == null || _ausSFX.Count == 0)
                {
                    return PlayerPrefs.GetFloat(SFXVolumeKey, 1);
                }

                return _ausSFX[0].volume;
            }
            set
            {
                var x = Mathf.Clamp(value, 0, 1);
                _ausSFX.ForEach(source => source.volume = x);
                PlayerPrefs.SetFloat(SFXVolumeKey, x);
            }
        }

        public void PlayBackgroundMusic(string name)
        {
            var audio = Resources.Load<AudioClip>($"_Audio/{name}");
            if (audio != null)
            {
                _ausBackground.clip = audio;
                _ausBackground.Play();
            }
            else
            {
                LogError($"Cant not find sound {name} in Resource/_Audio");
            }
        }

        public void PlaySfx(string name)
        {
            _sfxClip ??= new Dictionary<string, AudioClip>();
            var temp = _ausSFX.Find(x => !x.isPlaying);

            if (temp == null)
            {
                temp = gameObject.AddComponent<AudioSource>();
                temp.volume = sfxVolume;
                temp.loop = false;
                temp.playOnAwake = false;
                _ausSFX.Add(temp);
            }

            if (_sfxClip.ContainsKey(name))
            {
                temp.clip = _sfxClip[name];
            }
            else
            {
                var x = Resources.Load<AudioClip>($"_Audio/{name}");
                if (x == null)
                {
                    LogError($"Cant not find sound {name} in Resource/_Audio");
                }
                else
                {
                    _sfxClip.Add(name, x);
                    temp.clip = x;
                    temp.Play();
                }
            }
        }
    }
}
