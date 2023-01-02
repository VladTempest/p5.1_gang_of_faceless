using System;
using Scripts.Unit;
using UnityEngine;

namespace SoundSystemScripts
{
    public static class SoundtrackPlayerWrapper
    {
        public static void PlayStepSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Concrete_Step_Run, localSourcePosition);
        }
        
        public static void PlaySlideSound( Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Concrete_Slide, localSourcePosition);
        }
        
        public static void PlayLandSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Concrete_Land, localSourcePosition);
        }
        
        public static void PlayArmorFoleySound(Unit unit, Transform localSourcePosition)
        {
            switch (unit.UnitType)
            {
                case UnitType.None:
                    break;
                case UnitType.Archer:
                    PlaySound(TypeOfSFXByItsNature.Archer_armor_foley, localSourcePosition);
                    break;
                case UnitType.HeavyWarrior:
                    PlaySound(TypeOfSFXByItsNature.Heavy_armor_foley, localSourcePosition);
                    break;
                case UnitType.LightWarrior:
                    PlaySound(TypeOfSFXByItsNature.Light_armor_foley, localSourcePosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        
            
        public static void PlayWeaponSwooshSound(Unit unit, Transform localSourcePosition)
        {
            switch (unit.UnitType)
            {
                case UnitType.HeavyWarrior:
                    PlaySound(TypeOfSFXByItsNature.Sword_swoosh, localSourcePosition);
                    break;
                case UnitType.LightWarrior:
                    PlaySound(TypeOfSFXByItsNature.Dagger_swoosh, localSourcePosition);
                    break;
            }
        }

        public static void PlayBloodSplatter(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Blood_spaltter, localSourcePosition);
        }
        
        public static void PlayDismemberingSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Dismembering, localSourcePosition);
        }
        
        public static void PlayOnHitSound(Unit unit, Transform localSourcePosition)
                 {
                     switch (unit.UnitType)
                     {
                         case UnitType.HeavyWarrior:
                             PlaySound(TypeOfSFXByItsNature.Armor_hit, localSourcePosition);
                             break;
                         case UnitType.Archer:
                         case UnitType.LightWarrior:
                             PlaySound(TypeOfSFXByItsNature.Without_Armor_hit, localSourcePosition);
                             break;
                     }
                 }
        
        public static void PlayGruntSound(Unit unit, Transform localSourcePosition)
        {
            switch (unit.UnitType)
            {
                case UnitType.Archer:
                case UnitType.HeavyWarrior:
                    PlaySound(TypeOfSFXByItsNature.Male_grunt, localSourcePosition);
                    break;
                case UnitType.LightWarrior:
                    PlaySound(TypeOfSFXByItsNature.Female_grunt, localSourcePosition);
                    break;
            }
        }

        public static void PlayDeathSounds(Unit unit, Transform localSourcePosition)
        {
            PlayDismemberingSound(localSourcePosition);
            PlayArmorFoleySound(unit, localSourcePosition);
            PlayGruntSound(unit,localSourcePosition);
        }
        
        private static void PlaySound(TypeOfSFXByItsNature typeOfSfxByItsNature, Transform localSourcePosition = null)
        {
            SoundtrackPlayer.Instance.PlaySoundtrack(typeOfSfxByItsNature:typeOfSfxByItsNature,transformOfPlayPoint:localSourcePosition);
        }
        
        
        public static void PlayUISoundtrack(TypeOfSFXByItsNature soundtrack)
        {
            SoundtrackPlayer.Instance.PlaySoundtrack(typeOfSfxByItsNature: soundtrack);
        }

        public static void PlayOST(TypeOfOSTByItsNature typeOfOstByItsNature)
        {
            SoundtrackPlayer.Instance.PlaySoundtrack(typeOfOstByItsNature: typeOfOstByItsNature);
        }

        public static void PlaySwordHitSound(HeavyWarriorActionEnum action, Transform localSourcePosition)
        {
            switch (action)
            {
                case HeavyWarriorActionEnum.DefaultAttack:
                    PlaySound(TypeOfSFXByItsNature.Hit_bone, localSourcePosition);
                    break;
                case HeavyWarriorActionEnum.Push:
                    PlaySound(TypeOfSFXByItsNature.Push_hit, localSourcePosition);
                    break;
                case HeavyWarriorActionEnum.Knockdown:
                    PlaySound(TypeOfSFXByItsNature.Knockdown_hit, localSourcePosition);
                    break;
            }
        }

        public static void PLayHitArrowSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.ArrowHit, localSourcePosition);
        }

        public static void PlayParalyseArrowLaunchSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Paralyze_Arrow_Launch, localSourcePosition);
        }

        public static void PlayParalyseArrowSetupSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Paralyze_Arrow_Setup, localSourcePosition);
        }

        public static void PlayArrowSetupSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Arrow_take_new, localSourcePosition);
        }

        public static void PlayArrowLaunchSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Bow_launch, localSourcePosition);
        }

        public static void PlayBowStretchSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Bow_stretch, localSourcePosition);
        }

        public static void PlayUIActionChooseSound()
        {
            PlaySound(TypeOfSFXByItsNature.UI_CLick_OnActionChoose);
        }
        
        public static void PlayUITargetChooseSound()
        {
            PlaySound(TypeOfSFXByItsNature.UI_Click_OnTargetChoose);
        }
        
        public static void PlayUIUnitChooseSound()
        {
            PlaySound(TypeOfSFXByItsNature.UI_Click_OnUnitChoose);
        }

        public static void PlayBackstabSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Backstab_sound, localSourcePosition);
        }
        
        public static void PlayThrowBombSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Throw_bomb, localSourcePosition);
        } 
        
        public static void PlayTakeOutBombSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Take_out_bomb, localSourcePosition);
        }
        
        public static void PlayBombExplosionSound(Transform localSourcePosition)
        {
            PlaySound(TypeOfSFXByItsNature.Bomb_explosion, localSourcePosition);
        }
    }
}