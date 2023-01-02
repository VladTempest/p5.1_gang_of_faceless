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

        public void PlayHitSound(HeavyWarriorActionEnum action)
        {
            SoundtrackPlayerWrapper.PlaySwordHitSound(action, transform);
        }
        
        public void PlayBowStretchSound()
        {
            SoundtrackPlayerWrapper.PlayBowStretchSound(transform);
        }
        
        public void PlayArrowLaunchSound()
        {
            SoundtrackPlayerWrapper.PlayArrowLaunchSound(transform);
        }
        
        public void PlayArrowSetupSound()
        {
            SoundtrackPlayerWrapper.PlayArrowSetupSound(transform);
        }
        
        public void PlayParalyseArrowSetupSound()
        {
            SoundtrackPlayerWrapper.PlayParalyseArrowSetupSound(transform);
        }
        
        public void PlayParalyseArrowLaunchSound()
        {
            SoundtrackPlayerWrapper.PlayParalyseArrowLaunchSound(transform);
        }
        
        public void PlayBackstabSound()
        {
            SoundtrackPlayerWrapper.PlayBackstabSound(transform);
        }
        
        public void PlayThrowBombSound()
        {
            SoundtrackPlayerWrapper.PlayThrowBombSound(transform);
        }

        public void PlayTakeOutBombSound()
        {
            SoundtrackPlayerWrapper.PlayTakeOutBombSound(transform);
        }
    }
}