using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchCameraController : MonoBehaviour //TODO: Integrate this script into InputManager
{
	[SerializeField] private float dragSpeed = 0.005f;
	[SerializeField] private float pinchZoomSpeed = 0.1f;

	private InputAction dragAction;
	private InputAction pinchAction;
	
	private bool _isPinching = false;
	
	private float _lastPinchDistance;

	private void Awake()
	{
		var touchControls = new PlayerInputActions();
		dragAction = touchControls.Touch.Drag;
		pinchAction = touchControls.Touch.Pinch;

		dragAction.performed += ctx => OnDrag(ctx);
		pinchAction.performed += ctx => OnPinch(ctx);
	}


	private void OnDrag(InputAction.CallbackContext context)
	{
		if (_isPinching)
		{
			return;
		}
		
		
		Vector2 delta = context.ReadValue<Vector2>();
		transform.Translate(-delta.x * dragSpeed, -delta.y * dragSpeed, 0);
	
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