using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAnimationsEvents : MonoBehaviour
{
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
}
