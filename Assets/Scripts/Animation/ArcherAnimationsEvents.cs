using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAnimationsEvents : ActionAnimationEventInvoker
{
    
    internal Action DefaultShotCallback;
    internal Action LongShotCallback;
    internal Action EndDefaultShotCallback;
    internal Action EndLongShotCallback;
    public event Action OnGettingArrow;
    public event Action OnReleaseArrow;
    
   
    public void GetArrow()
    {
        OnGettingArrow?.Invoke();
    }

    public void ReleaseArrow()
    {
        OnReleaseArrow?.Invoke();
    }

    public void MakeDefaultShot()
    {
        DefaultShotCallback?.Invoke();
    }
    
    public void MakeLongtShot()
    {
        LongShotCallback?.Invoke();
    }
    
    public void EndDefaultShot()
    {
        EndDefaultShotCallback?.Invoke();
    }
    
    public void EndLongtShot()
    {
        EndLongShotCallback?.Invoke();
    }
    
    
}
