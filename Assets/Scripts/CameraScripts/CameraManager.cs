using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
                if (!IfWillTurnOnByRandom()) break;
                Unit shooterUnit = shootAction.ActiveUnit;
                Unit targetUnit = shootAction.TargetUnit;
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

    private static bool IfWillTurnOnByRandom()
    {
        return Random.Range(0, 10) < 3;
    }
}
