using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using System.ComponentModel;

public class Testing : MonoBehaviour
{
    [SerializeField] private UIDocument uiDoc;
    private const string resourceCountKey = "ResourceCount";
    public ResourceScriptableObject resourceScriptableObject;
    
    void Start()
    {
        
            var label = uiDoc.rootVisualElement.Q<Label>(resourceCountKey);
            label.bindingPath = nameof(resourceScriptableObject.Value);
            label.Bind(new SerializedObject(resourceScriptableObject));

    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
           
        }

    }
}
