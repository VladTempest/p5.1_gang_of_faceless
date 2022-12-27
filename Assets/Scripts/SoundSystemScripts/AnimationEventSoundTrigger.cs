using System;
using Scripts.Unit;
using UnityEngine;

namespace SoundSystemScripts
{
    public class AnimationEventSoundTrigger : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        public void PlayStepSound()
        {
            SoundtrackPlayerWrapper.PlayStepSound(transform);
        }
        
        public void PlaySlideSound()
        {
            SoundtrackPlayerWrapper.PlaySlideSound(transform);
        }
        
        public void PlayLandSound()
        {
            SoundtrackPlayerWrapper.PlayLandSound(transform);
        }
        
        public void PlayArmorFoleySound()
        {
            SoundtrackPlayerWrapper.PlayArmorFoleySound(_unit,transform);
        }
        
        
            
        public void PlayWeaponSwooshSound()
        {
            SoundtrackPlayerWrapper.PlayWeaponSwooshSound(_unit, transform);
        }

        public void PlayBloodSplatter()
        {
            SoundtrackPlayerWrapper.PlayBloodSplatter(transform);
        }
        
        public void PlayOnHitSound()
        {
            SoundtrackPlayerWrapper.PlayOnHitSound(_unit, transform);
        }
        
        public void PlayGruntSound()
        {
            SoundtrackPlayerWrapper.PlayGruntSound(_unit,transform);
        }
    }
}