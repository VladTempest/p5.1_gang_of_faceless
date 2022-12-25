using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image _progressBar;

    public void SetLoadProgressAmount(float progress)
    {
        Debug.Log("loading!!!!!!!!!");
        _progressBar.fillAmount = progress;
    }
}
