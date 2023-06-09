using UnityEngine;

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
        return raycastHit.point;
    }
}