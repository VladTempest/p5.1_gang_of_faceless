using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAnimationEventInvoker : MonoBehaviour
{
    
    internal Action ActionStartCallback;
    internal Action ActionEffectCallback;
    internal Action ActionFinishCallback;
    

    private void StartAction()
    {
        ActionStartCallback?.Invoke();
    }
    
    private void FinishAction()
    {
        ActionFinishCallback?.Invoke();
    }
    
    private void EffectAction()
    {
        ActionEffectCallback?.Invoke();
    }
}
