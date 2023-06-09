﻿using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Editor.Scripts.Animation
{
    public class LightWarriorAnimationEvents : MonoBehaviour
    {
        public event Action OnGettingBomb;
        public event Action OnReleaseBomb;
        
        public event Action OnEquipDagger;
        public Action ActionTeleportCallback { get; set; }
        public Action ActionEffectCallback { get; set; }
        public Action ActionFinishCallback { get; set; }
        public Action BackStepCutWasMadeCallback { get; set; }

        public void GetBomb()
        {
            OnGettingBomb?.Invoke();
        }
        
        public void ReleaseBomb()
        {
            OnReleaseBomb?.Invoke();
        }
        
        public void EquipDagger()
        {
            OnEquipDagger?.Invoke();
        }
        public void Teleport()
        {
            ActionTeleportCallback?.Invoke();   
        }

        public void BackStabEffectAction()
        {
            ActionEffectCallback?.Invoke();   
        }

        public void BackStabFinishAction()
        {
            ActionFinishCallback?.Invoke();   
        }

        public void BackStepCutWasMadeAction()
        {
           BackStepCutWasMadeCallback?.Invoke();   
        }
    }
}