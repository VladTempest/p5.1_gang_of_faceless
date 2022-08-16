using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }
    
    private CinemachineImpulseSource _cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are many singletonss");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }
    
    public void Shake(float intensity = 1f)
    {
        _cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
