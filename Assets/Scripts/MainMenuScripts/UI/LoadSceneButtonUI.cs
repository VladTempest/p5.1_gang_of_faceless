using System;
using Editor.Scripts.SceneLoopScripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Editor.Scripts.MainMenuScripts.UI
{
    public class LoadSceneButtonUI : ButtonBehaviourUI
    {
       
       [SerializeField] private ScenesEnum _sceneToLoad;
       
       private void Start()
        {
            _button.onClick.AddListener(LoadScene);
            
        }

        private void LoadScene()
        {
            ScenesController.Instance.LoadScene(_sceneToLoad);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(LoadScene);
        }
    }
    
    
}