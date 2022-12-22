using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.Utils;
using UnityEngine;

public class ConstantsProvider : MonoBehaviour
{
    public static ConstantsProvider Instance { get; private set; }

    [SerializeField] public ActionsParamtersSO actionsParametersSO; 
    [SerializeField] public ClassesParamtersSO classesParametersSO;
    [SerializeField] public GridEstimationWeightsSO gridEstimationWeightsSO;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are many singletonss", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
