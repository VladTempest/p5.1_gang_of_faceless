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
            PlayBloodSplatter(localSourcePosition);
            PlayArmorFoleySound(unit, localSourcePosition);
            PlayGruntSound(unit,localSourcePosition);
        }
        
        private static void PlaySound(TypeOfSFXByItsNature typeOfSfxByItsNature, Transform localSourcePosition)
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
    }
}