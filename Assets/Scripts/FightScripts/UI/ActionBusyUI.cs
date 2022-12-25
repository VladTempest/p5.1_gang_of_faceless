using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _unitActionUI; 
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        gameObject.SetActive(false);
    }
    
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        gameObject.SetActive(isBusy);
        foreach (var uiElement in _unitActionUI)
        {
            uiElement.SetActive(!isBusy);
        }
    }
    
}
