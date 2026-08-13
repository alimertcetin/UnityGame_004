using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneListSO sceneListSO;
        public static SceneLoader instance { get; private set; }

        public Action<SceneSO, SceneLoadSettings> sceneLoadStarted;
        public Action<SceneSO, SceneLoadSettings, float> sceneLoadProgress;
        public Action<SceneSO, SceneLoadSettings> sceneLoadCompleted;
        
        public Action<SceneSO, SceneUnloadSettings> sceneUnloadStarted;
        public Action<SceneSO, SceneUnloadSettings, float> sceneUnloadProgress;
        public Action<SceneSO, SceneUnloadSettings> sceneUnloadedCompleted;
        
        public SceneSO lastLoadedScene { get; private set; }

        void Awake()
        {
            if (instance != this) Destroy(instance);
            instance = this;
        }

        public void LoadScene(SceneSO sceneToLoad, SceneLoadSettings sceneLoadSettings)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad.sceneBuildIndex, sceneLoadSettings.loadSceneMode);
            loadOperation.allowSceneActivation = false;
            StartCoroutine(LoadNewScene(sceneToLoad, sceneLoadSettings, loadOperation));
            sceneLoadStarted?.Invoke(sceneToLoad, sceneLoadSettings);
        }

        public void UnloadScene(SceneSO sceneToUnload, SceneUnloadSettings sceneUnloadSettings)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(sceneToUnload.sceneBuildIndex), sceneUnloadSettings.unloadSceneOptions);
            StartCoroutine(UnloadScene(sceneToUnload, sceneUnloadSettings, unloadOperation));
            sceneUnloadStarted?.Invoke(sceneToUnload, sceneUnloadSettings);
        }

        IEnumerator LoadNewScene(SceneSO sceneToLoad, SceneLoadSettings sceneLoadSettings, AsyncOperation obj)
        {
            while (obj.isDone == false)
            {
                if (obj.progress >= 0.9f) obj.allowSceneActivation = true;
                sceneLoadProgress?.Invoke(sceneToLoad, sceneLoadSettings, obj.progress);
                yield return null;
            }
            sceneLoadProgress?.Invoke(sceneToLoad, sceneLoadSettings, obj.progress);
            lastLoadedScene = sceneToLoad;
            
            if (sceneLoadSettings.activateSceneAfterLoad)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(lastLoadedScene.sceneBuildIndex));
            }
            
            sceneLoadCompleted?.Invoke(sceneToLoad, sceneLoadSettings);
        }

        IEnumerator UnloadScene(SceneSO sceneToUnload, SceneUnloadSettings sceneUnloadSettings, AsyncOperation obj)
        {
            while (obj.isDone == false)
            {
                sceneUnloadProgress?.Invoke(sceneToUnload, sceneUnloadSettings, obj.progress);
                yield return null;
            }
            sceneUnloadProgress?.Invoke(sceneToUnload, sceneUnloadSettings, obj.progress);
            sceneUnloadedCompleted?.Invoke(sceneToUnload, sceneUnloadSettings);
        }
    }
}