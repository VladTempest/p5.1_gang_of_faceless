using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 20f;

    private Vector3 _targetFollowOffset;
    private CinemachineTransposer _cinemachineTransposer;

    private void Start()
    {
       _cinemachineTransposer = _cinemachineCamera?.GetCinemachineComponent<CinemachineTransposer>();
       _targetFollowOffset = _cinemachineTransposer.m_FollowOffset;
    }

    void Update()
    {
        HandleTheMovement();

        HandleTheRotation();

        HandleTheZoom();

    }

    private void HandleTheZoom()
    {
        float zoomSpeed = 7f;

        _targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();

        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        _cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

    private void HandleTheMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
       
        float moveSpeed = 12f;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleTheRotation()
    {
        float rotationSpeed = 120f;
        var rotateAmount = InputManager.Instance.GetCameraRotateAmount();
        var rotationVector = new Vector3(0,rotateAmount,0); 
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
}
