using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionCameraGameObject;

    private void ShowActionCamera()
    {
        _actionCameraGameObject.SetActive(true);
    }
    
    private void HideActionCamera()
    {
        _actionCameraGameObject.SetActive(false);
    }

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCpmpleted;
    }

    private void BaseAction_OnAnyActionCpmpleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetActiveUnit();
                Unit targetUnit = shootAction.GetTargetUnit();
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                Vector3 shootDirection = (targetUnit.WorldPosition - shooterUnit.WorldPosition).normalized;
                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;

                Vector3 positionForActionCamera = shooterUnit.WorldPosition + cameraCharacterHeight + shoulderOffset + shootDirection * (-1);

                _actionCameraGameObject.transform.position = positionForActionCamera;
                _actionCameraGameObject.transform.LookAt(targetUnit.transform.position + cameraCharacterHeight);
                ShowActionCamera();
                break;
        }
    }
}
