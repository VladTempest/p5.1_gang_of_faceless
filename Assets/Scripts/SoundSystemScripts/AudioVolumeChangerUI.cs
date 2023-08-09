using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace SoundSystemScripts
{
    public class AudioVolumeChangerUI : MonoBehaviour, ISaveable
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
            DataPersistenceManager.Instance.RegisterDataPersistence(this);
            _volumeSlider.onValueChanged.AddListener(SetVolume);
            RestoreData();
        }

        private void SetVolume(float volume)
        {
            var resultVolume = -80f + 80 * volume;
            _masterMixer.SetFloat(Enum.GetName(typeof(AudioMixerExposeParams),_parameterToChange), resultVolume);
        }

        public (object, object) CaptureData()
        {
            return (_parameterToChange, _volumeSlider.value);
        }

        public void RestoreData()
        {
            var persistentDataValue = DataPersistenceManager.Instance.GetState(_parameterToChange);

            if (persistentDataValue == null) return;
            
            _volumeSlider.value = (float) persistentDataValue;
        }
    }
}