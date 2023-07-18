using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.HubLocation;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugActionsUI : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ResourceController.Instance.IncreaseResource(ResourceTypes.ParalyzingArrows,10);
            Debug.Log($"Increase ParalyzingArrows resource amount: 10. Now: {ResourceController.Instance.GetResourceReactiveData(ResourceTypes.ParalyzingArrows)?.Amount.Value}");
        }
       
    }
}
