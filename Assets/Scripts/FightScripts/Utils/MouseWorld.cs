using UnityEngine;
using UnityEngine.EventSystems;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask _mousePlaneLayerMask;

    private static MouseWorld _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static Vector3 GetPointerInWorldPosition()
    {
        var ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out var raycastHit, float.MaxValue, _instance._mousePlaneLayerMask);
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject()) return Vector3.zero;
#else
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return Vector3.zero;
#endif
        
        return raycastHit.point;
    }
}