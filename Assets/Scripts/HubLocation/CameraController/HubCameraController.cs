using System;
using Cinemachine;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Editor.Scripts.HubLocation.CameraController
{
	public enum HubCameraState
	{
		FreeLook,
		Room
	}

	public class HubCameraController : MonoBehaviour
	{
		private const string LayerNames = "RoomView";
		[SerializeField] private CinemachineVirtualCamera _freeLookCamera;
		[SerializeField] private CinemachineVirtualCamera _roomCamera;

		private HubCameraState _hubCameraState = HubCameraState.FreeLook;

		public bool IsFreeLook => _hubCameraState == HubCameraState.FreeLook;

		[SerializeField] private Transform _cameraTransform;
		[SerializeField] private Transform _centerPoint;

		[SerializeField] private float dragSpeed = 0.005f;
		[SerializeField] private float pinchZoomSpeed = 0.1f;


		private int _defaultFromCameraDistance = -392;
		private InputAction dragAction;
		private InputAction pinchAction;
		private InputAction touchAction;

		private bool _isPinching = false;

		private float _lastPinchDistance;
		private float _multiplier = 1;
		private RoomView _currentFocusedRoom;

		private void Awake()
		{
			var touchControls = new PlayerInputActions();
			dragAction = touchControls.Touch.Drag;
			pinchAction = touchControls.Touch.Pinch;
			touchAction = touchControls.Touch.Touch;

			dragAction.performed += ctx => OnDrag(ctx);
			pinchAction.performed += ctx => OnPinch(ctx);
			touchAction.performed += ctx => OnTouch(ctx);
		}
		
		private void OnEnable()
		{
			dragAction.Enable();
			pinchAction.Enable();
			touchAction.Enable();
		}

		private void OnDisable()
		{
			dragAction.Disable();
			pinchAction.Disable();
			touchAction.Disable();
		}

		private void Start()
		{
			_defaultFromCameraDistance = (int) _cameraTransform.position.z;
		}


		private float GetMultiplierForCameraDragFromCurrentDistance()
		{
			float currentDistance = _cameraTransform.position.z;
			var difference = currentDistance - _defaultFromCameraDistance;
			if (difference > 0)
			{
				_multiplier = 1 - Mathf.Abs(difference / _defaultFromCameraDistance);
			}
			else if (difference < 0)
			{
				_multiplier = 1 + Mathf.Abs(difference / _defaultFromCameraDistance);
			}
			else if (difference <= 0.01f)
			{
				ConvenientLogger.Log(nameof(HubCameraController), GlobalLogConstant.IsTouchLogEnabled,
					$"_multiplier 1 is {_multiplier}");
				return _multiplier;
			}

			if (_multiplier <= 0)
			{
				_multiplier = 0.01f;
			}

			ConvenientLogger.Log(nameof(HubCameraController), GlobalLogConstant.IsTouchLogEnabled,
				$"_multiplier 2 is {_multiplier}");
			return _multiplier;
		}

		private void OnDrag(InputAction.CallbackContext context)
		{
			if (!IsFreeLook) return;

			if (_isPinching) return;

			Vector2 delta = context.ReadValue<Vector2>();
			var multiplier = GetMultiplierForCameraDragFromCurrentDistance();
			var relativeDragSpeed = dragSpeed * multiplier;
			_centerPoint.Translate(-delta.x * relativeDragSpeed, -delta.y * relativeDragSpeed, 0);
		}

		private void OnPinch(InputAction.CallbackContext ctx)
		{
			var touch = ctx.ReadValue<TouchState>();

			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				_isPinching = false;
				return;
			}

			_isPinching = true;

			Vector2 currentPinchPosition = touch.position;
			Vector2 primaryTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

			float currentPinchDistance = Vector2.Distance(primaryTouchPosition, currentPinchPosition);


			if (!IsFreeLook)
			{
				Debug.Log($"Pinch in room {currentPinchDistance}");
				_currentFocusedRoom?.HandlePinch();
				_currentFocusedRoom = null;
			}
			else
			{
				if (touch.phase == TouchPhase.Began)
				{
					_lastPinchDistance = currentPinchDistance;
				}

				float delta = _lastPinchDistance - currentPinchDistance;
				
				_centerPoint.position -= _centerPoint.forward * delta * pinchZoomSpeed;
				_lastPinchDistance = currentPinchDistance;
			}
		}

		private void OnTouch(InputAction.CallbackContext ctx)
		{
			if (!IsFreeLook) return;
			if (_isPinching) return;
			
			var touch = ctx.ReadValue<TouchState>();
			if (touch.phase == TouchPhase.Ended)
			{
				var ray = Camera.main.ScreenPointToRay(touch.position);

				if (Physics.Raycast(ray, out var hit, 1000, LayerMask.GetMask(LayerNames)))
				{
					ConvenientLogger.Log(nameof(HubCameraController), GlobalLogConstant.IsTouchLogEnabled,
						$"Hit {hit.transform.name}");
					var room = hit.transform.GetComponent<RoomView>();
					if (room != null)
					{
						room.HandleTouch();
						_currentFocusedRoom = room;
					}
				}
			}
		}

		public void FocusOnRoom(Transform roomTransform)
		{
			_roomCamera.Follow = roomTransform;
			_roomCamera.LookAt = roomTransform;
			
			_freeLookCamera.Priority = 0;
			_roomCamera.Priority = 1;
			
			_hubCameraState = HubCameraState.Room;
		}
		
		public void UnfocusOnRoom()
		{
			_freeLookCamera.Priority = 1;
			_roomCamera.Priority = 0;
			
			_hubCameraState = HubCameraState.FreeLook;
		}
	}
}