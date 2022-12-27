using System;
using UnityEngine;

namespace SoundSystemScripts
{
    [Serializable]
    public class Soundtrack
    {
        public string Name="audiotrack";
        public AudioClip[] Clip;
        [Range(0f, 1f)]
        public float Volume;

        public TypeOfSoundtrack typeOfSoundtrack;
        public TypeOfOSTByItsNature typeOfOstByItsNature;
        public TypeOfSFXByItsNature typeOfSfxByItsNature;
        public TypeOfSFXByPlace typeOfSfxByPlace;
    }
}
