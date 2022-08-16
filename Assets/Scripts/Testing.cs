using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ScreenShake.Instance.Shake();
        }
    }
}
