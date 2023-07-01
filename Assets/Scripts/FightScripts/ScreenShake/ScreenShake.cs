using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Editor.Scripts.GlobalUtils;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }
    
    private CinemachineImpulseSource _cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            ConvenientLogger.Log(name, GlobalLogConstant.IsSingltonsLogEnabled, $"There are many singletonss");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }
    
    public void Shake(float intensity = 0.2f)
    {
        _cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
