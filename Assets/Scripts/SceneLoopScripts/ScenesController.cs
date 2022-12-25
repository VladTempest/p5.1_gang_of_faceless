using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.Scripts.SceneLoopScripts
{
    public class ScenesController : MonoBehaviour
    {
        public static ScenesController Instance { get; set; }
        
        [SerializeField] private List<SceneWithEnum> _sceneWithEnums;
        private LoadingProgressBarUI _progressBar;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There are many singletonss", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        
        public void LoadScene(ScenesEnum sceneEnumToLoad)
        {
            string sceneToLoad = GetSceneNameFromSceneEnum(sceneEnumToLoad);
            LoadWithLoadingScreen(sceneToLoad);
        }

        private string GetSceneNameFromSceneEnum(ScenesEnum sceneEnumToFind)
        {
            var sceneWithEnum = _sceneWithEnums.Find(item => item.sceneEnumName == sceneEnumToFind);
            return sceneWithEnum.sceneName;
        }

        private void LoadWithLoadingScreen(string targetSceneName)
        {
            var loadingSceneName = GetSceneNameFromSceneEnum(ScenesEnum.Loading);
            SceneManager.LoadScene(loadingSceneName);
            _progressBar = FindObjectOfType<LoadingProgressBarUI>();
            TargetSceneLoadASync(targetSceneName);
        }

        private async void TargetSceneLoadASync(string sceneName)
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            do
            {
                Debug.Log("10 sec passed");
                await Task.Delay(1000);
                if (_progressBar != null) _progressBar.SetLoadProgressAmount(loadOperation.progress);
            } while (loadOperation.progress < 0.9f);

            loadOperation.allowSceneActivation = true;
        }
    }
}