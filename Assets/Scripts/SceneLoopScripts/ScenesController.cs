using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.Scripts.SceneLoopScripts
{
    public class ScenesController : MonoBehaviour
    {
        public static ScenesController Instance { get; set; }
        
        [SerializeField] private List<SceneWithEnum> _sceneWithEnums;

        private LoadingProgressBarUI _progressBar;

        private bool _alreadyLoaded;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        public bool CheckCurrentScene(ScenesEnum sceneEnumToCheck)
        {
            string sceneToCheck = GetSceneNameFromSceneEnum(sceneEnumToCheck);
            return SceneManager.GetActiveScene().name == sceneToCheck;
        }


        public void LoadScene(ScenesEnum sceneEnumToLoad)
        {
            string sceneToLoad = GetSceneNameFromSceneEnum(sceneEnumToLoad);
            LoadWithLoadingScreen(sceneToLoad);
        }

        public void LoadScene(string sceneName)
        {
            LoadWithLoadingScreen(sceneName);
        }

        private string GetSceneNameFromSceneEnum(ScenesEnum sceneEnumToFind)
        {
            var sceneWithEnum = _sceneWithEnums.Find(item => item.sceneEnumName == sceneEnumToFind);
            return sceneWithEnum.sceneName;
        }


        private void LoadWithLoadingScreen(string targetSceneName)
        {
            _alreadyLoaded = false;
            StartCoroutine(LoadingSceneLoadAsync());
            StartCoroutine(TargetSceneLoadASync(targetSceneName));
        }

        private IEnumerator TargetSceneLoadASync(string sceneName)
        {
            yield return null;
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            while (!loadOperation.isDone)
            {
                if (!_alreadyLoaded)
                {
                    if (_progressBar != null) _progressBar.SetLoadProgressAmount(loadOperation.progress);
                    yield return null;
                }


                if (loadOperation.progress >= 0.9f)
                {
                    _alreadyLoaded = true;
                    _progressBar.SetLoadProgressAmount(1f);
                    yield return null;
                    loadOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        private IEnumerator LoadingSceneLoadAsync()
        {
            yield return null;
            string sceneName = GetSceneNameFromSceneEnum(ScenesEnum.Loading);
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            
            while (!loadOperation.isDone)
            {
                yield return null;
            }
            
            _progressBar = FindObjectOfType<LoadingProgressBarUI>();
        }

        public void ReloadCurrentScene()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            LoadWithLoadingScreen(currentScene);
        }
    }
}