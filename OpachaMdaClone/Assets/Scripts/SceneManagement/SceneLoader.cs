using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneListSO sceneListSO;
        public static Action<SceneSO, SceneLoadSettings> loadScene;
        public static event Action<SceneSO, SceneLoadSettings> onSceneLoadingStarted;
        public static event Action<float> onSceneLoading;
        public static event Action<SceneSO, SceneLoadSettings> onSceneLoadComplete;
        
        SceneSO currentScene;
        SceneSO sceneToLoad;
        SceneLoadSettings sceneLoadSettings;

        void Start()
        {
            OnLoadSceneRequested(sceneListSO.GetSceneByContainingSceneName("mainMenu"), SceneLoadSettings.GetDefault());
        }

        void OnEnable()
        {
            loadScene += OnLoadSceneRequested;
        }

        void OnDisable()
        {
            loadScene -= OnLoadSceneRequested;
        }

        void OnLoadSceneRequested(SceneSO sceneToLoad, SceneLoadSettings sceneLoadSettings)
        {
            this.sceneToLoad = sceneToLoad;
            this.sceneLoadSettings = sceneLoadSettings;
            var previousScene = sceneListSO.GetSceneByContainingSceneName(SceneManager.GetActiveScene().name);
            
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad.sceneBuildIndex, sceneLoadSettings.loadSceneMode);
            loadOperation.allowSceneActivation = false;
            StartCoroutine(LoadNewScene(loadOperation));
            onSceneLoadingStarted?.Invoke(sceneToLoad, sceneLoadSettings);
            
            if (sceneLoadSettings.unloadActiveScene)
            {
                if (previousScene)
                {
                    var unloadOperation = SceneManager.UnloadSceneAsync(previousScene.sceneBuildIndex);
                    StartCoroutine(UnloadScene(unloadOperation));
                }
            }
        }

        IEnumerator LoadNewScene(AsyncOperation obj)
        {
            while (obj.isDone == false)
            {
                if (obj.progress >= 0.9f) obj.allowSceneActivation = true;
                onSceneLoading?.Invoke(obj.progress);
                yield return null;
            }
            onSceneLoading?.Invoke(obj.progress);
            currentScene = sceneToLoad;
            if (sceneLoadSettings.activateSceneAfterLoad)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentScene.sceneBuildIndex));
            }
            onSceneLoadComplete?.Invoke(currentScene, sceneLoadSettings);
        }

        IEnumerator UnloadScene(AsyncOperation obj)
        {
            while (obj.isDone == false)
            {
                yield return null;
            }
        }
    }
}