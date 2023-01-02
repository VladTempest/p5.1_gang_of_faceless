using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace SoundSystemScripts
{
    public class AudioVolumeChangerUI : MonoBehaviour
    {
        [SerializeField] private AudioMixer _masterMixer;

        [SerializeField] private AudioMixerExposeParams _parameterToChange;
        private Slider _volumeSlider;

        private void Awake()
        {
            _volumeSlider = GetComponent<Slider>();
        }

        private void Start()
        {
            _volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        private void SetVolume(float volume)
        {
            var resultVolume = -80f + 80 * volume;
            _masterMixer.SetFloat(Enum.GetName(typeof(AudioMixerExposeParams),_parameterToChange), resultVolume);
        }
        
    }
}