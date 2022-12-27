using UnityEngine;
using UnityEngine.UI;

namespace Editor.Scripts.MainMenuScripts.UI
{
    public class ButtonBehaviourUI : MonoBehaviour
    {
        protected Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
    }
}