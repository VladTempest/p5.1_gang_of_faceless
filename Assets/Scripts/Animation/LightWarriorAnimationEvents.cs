using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Editor.Scripts.Animation
{
    public class LightWarriorAnimationEvents : MonoBehaviour
    {
        public Action ActionTeleportCallback { get; set; }
        public Action ActionEffectCallback { get; set; }
        public Action ActionFinishCallback { get; set; }

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
    }
}