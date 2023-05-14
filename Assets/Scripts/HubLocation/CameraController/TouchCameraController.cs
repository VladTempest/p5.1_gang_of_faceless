using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchCameraController : MonoBehaviour //TODO: Integrate this script into InputManager
{
	[SerializeField] private float dragSpeed = 0.005f;
	[SerializeField] private float pinchZoomSpeed = 0.1f;


	private int _defaultFromCameraDistance = -392; 
	private InputAction dragAction;
	private InputAction pinchAction;
	
	private bool _isPinching = false;
	
	private float _lastPinchDistance;
	private float _multiplier=1;

	private void Awake()
	{
		var touchControls = new PlayerInputActions();
		dragAction = touchControls.Touch.Drag;
		pinchAction = touchControls.Touch.Pinch;

		dragAction.performed += ctx => OnDrag(ctx);
		pinchAction.performed += ctx => OnPinch(ctx);
	}


	private float GetMultiplierForCameraDragFromCurrentDistance()
	{
		float currentDistance = transform.position.z;
		var difference = currentDistance - _defaultFromCameraDistance;
		if (difference>0)
		{
			_multiplier = 1 - Mathf.Abs(difference / _defaultFromCameraDistance);
		}
		else if (difference<0)
		{
			_multiplier = 1 + Mathf.Abs(difference / _defaultFromCameraDistance);
		}
		else if (difference <= 0.01f)
		{
			return _multiplier;
		}
		
		if (_multiplier <= 0)
		{
			_multiplier = 0.01f;
		}
		return _multiplier;
	}
	
	private void OnDrag(InputAction.CallbackContext context)
	{
		if (_isPinching)
		{
			return;
		}
		
		
		Vector2 delta = context.ReadValue<Vector2>();
		var multiplier = GetMultiplierForCameraDragFromCurrentDistance();
		var relativeDragSpeed = dragSpeed * multiplier;
		transform.Translate(-delta.x * relativeDragSpeed, -delta.y * relativeDragSpeed, 0);
	
	}

	private void OnPinch(InputAction.CallbackContext ctx)
	{
		var touch = ctx.ReadValue<TouchState>();
		
		if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			_isPinching = false;
			return;
		}
		else
		{
			_isPinching = true;
		}
		Vector2 currentPinchPosition = touch.position;
		Vector2 primaryTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

		float currentPinchDistance = Vector2.Distance(primaryTouchPosition, currentPinchPosition);
		
		if (touch.phase == TouchPhase.Began)
		{
			_lastPinchDistance = currentPinchDistance;
		}
		
		float delta = _lastPinchDistance - currentPinchDistance;

		Debug.Log($"Delta: {delta}");


		transform.position -= transform.forward * delta * pinchZoomSpeed;
		_lastPinchDistance = currentPinchDistance;
	}

	private void OnEnable()
	{
		dragAction.Enable();
		pinchAction.Enable();
	}

	private void OnDisable()
	{
		dragAction.Disable();
		pinchAction.Disable();
	}
}