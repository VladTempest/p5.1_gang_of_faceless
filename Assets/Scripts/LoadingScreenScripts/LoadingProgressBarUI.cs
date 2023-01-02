using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image _progressBar;

    public void SetLoadProgressAmount(float progress)
    {
        if (Math.Abs(progress - _progressBar.fillAmount) > 0.001f) _progressBar.fillAmount = progress;
    }
}
