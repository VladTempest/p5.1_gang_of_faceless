using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using GridSystems;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Collider _cameraBorders;
    [SerializeField] private GameObject _cameraPointerVisuals;   

    private Coroutine _moveToSelectedUnitCoroutine;

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 20f;
    
    private const float MOVE_SPEED = 9f;

    private Vector3 _targetFollowOffset;
    private CinemachineTransposer _cinemachineTransposer;

    private void Start()
    {
       _cinemachineTransposer = _cinemachineCamera?.GetCinemachineComponent<CinemachineTransposer>();
       _targetFollowOffset = _cinemachineTransposer.m_FollowOffset;
       UnitActionSystem.Instance.OnSelectedPositionChanged += UnitAction_OnSelectedUnitChanged;
       TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
       UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged;
    }

    private void OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            _cameraPointerVisuals.SetActive(false);
        }
        else
        {
            _cameraPointerVisuals.SetActive(true);
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        _cameraPointerVisuals.SetActive(TurnSystem.Instance.IsPlayerTurn);
    }

    private void UnitAction_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        var selectedPosition = UnitActionSystem.Instance.GetSelectedPosition();
        if (selectedPosition == null) return;
        var worldPosition = LevelGrid.Instance.GetWorldPosition((GridPosition) selectedPosition);
        StartMovingToTargetUnit(worldPosition);
    }

    public void StartMovingToTargetUnit(Vector3 positionOfTargetUnit)
    {
        StopMovingToUnit();
        _moveToSelectedUnitCoroutine = StartCoroutine(MoveToTargetUnit(positionOfTargetUnit));
    }

    private void StopMovingToUnit()
    {
        if (_moveToSelectedUnitCoroutine != null) StopCoroutine(_moveToSelectedUnitCoroutine);
    }

    void Update()
    {
        HandleTheMovement();

        HandleTheRotation();

        HandleTheZoom();

    }

    private void HandleTheZoom()
    {
        float zoomSpeed = 5f;

        _targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();
        if (InputManager.Instance.GetCameraZoomAmount() != 0) StopMovingToUnit();
        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        _cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

    private void HandleTheMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
        if (!inputMoveDir.Equals(Vector3.zero)) StopMovingToUnit(); 
        if (inputMoveDir.Equals(Vector3.zero)) return; 
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        var newPosition = transform.position + moveVector * MOVE_SPEED * Time.deltaTime;
        if (_cameraBorders.bounds.Contains(newPosition)) transform.position += moveVector * MOVE_SPEED * Time.deltaTime;

        ChooseGridAccordingly();
    }

    private void ChooseGridAccordingly()
    {
        if (!TurnSystem.Instance.IsPlayerTurn) return;
        var positionOnGridLevel = new Vector3(transform.position.x, 0, transform.position.z);
        var currentGridPosition = LevelGrid.Instance.GetGridPosition(positionOnGridLevel);
        
        if (UnitActionSystem.Instance.GetSelectedPosition() == currentGridPosition) return;
        if (UnitActionSystem.Instance.IfCurrentGridPositionFromCachedValidPositions(currentGridPosition))
        {
            UnitActionSystem.Instance.SetSelectedPosition(currentGridPosition);
        }
        else
        {
            UnitActionSystem.Instance.ClearSelectedPosition();
        }
    }

    private void HandleTheRotation()
    {
        float rotationSpeed = 100f;
        var rotateAmount = InputManager.Instance.GetCameraRotateAmount();
        if (rotateAmount != 0) StopMovingToUnit();
        var rotationVector = new Vector3(0,rotateAmount,0); 
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    IEnumerator MoveToTargetUnit(Vector3 positionOfTargetUnit)
    {
        var positionAboveSelectedUnit =
            new Vector3(positionOfTargetUnit.x, transform.position.y, positionOfTargetUnit.z);
        while (Vector3.Distance(transform.position, positionAboveSelectedUnit) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, positionAboveSelectedUnit, Time.deltaTime * MOVE_SPEED/7);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitAction_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        UnitActionSystem.Instance.OnBusyChanged -= OnBusyChanged;
    }
}
