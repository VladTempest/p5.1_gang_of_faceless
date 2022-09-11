using System;
using UnityEngine;

public class WarriorAnimationEvents : MonoBehaviour
{
    internal Action ActionStartCallback;
    internal Action ActionEffectCallback;
    internal Action ActionFinishCallback;
    internal Action PushingCallback;
    internal Action PushingFinish;
    public Action DualSwordCutWasMadeCallback { get; set; }



    private void StartMeleeAction()
    {
        ActionStartCallback?.Invoke();
    }

    private void FinishMeleeAction()
    {
        ActionFinishCallback?.Invoke();
    }

    private void EffectMeleeAction()
    {
        ActionEffectCallback?.Invoke();
    }

    private void EffectPushingAction()
    {
        PushingCallback?.Invoke();
    }

    private void FinishPushingAction()
    {
        PushingFinish?.Invoke();
    }

    public void DualSwordCutWasMadeAction()
    {
        DualSwordCutWasMadeCallback?.Invoke();   
    }
}
