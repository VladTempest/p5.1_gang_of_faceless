using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

namespace Editor.Scripts.FightScripts.UI.JoySticks
{
    public class CustomFloatingJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler 
    {
        [SerializeField] private FloatingOnScreenStick _joystick;

        private void Awake()
        {
            _joystick.gameObject.SetActive(false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _joystick.gameObject.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            _joystick.OnPointerDown(eventData, position);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _joystick.gameObject.SetActive(false);
        }


        public void OnDrag(PointerEventData eventData)
        {
            _joystick.OnDrag(eventData);
        }
    }
}