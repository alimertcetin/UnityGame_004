using TheGame.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] SceneListSO sceneListSO;
        
        void Start()
        {
            var sceneSO = sceneListSO.GetSceneByContainingSceneName("persistant");
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneSO.sceneBuildIndex, new LoadSceneParameters(LoadSceneMode.Additive));
            asyncOperation.completed += OnPersistentSceneLoaded;
            asyncOperation!.allowSceneActivation = true;
        }

        void OnPersistentSceneLoaded(AsyncOperation obj)
        {
            SceneLoader.instance.UnloadScene(sceneListSO.GetSceneByContainingSceneName("initialization"), new SceneUnloadSettings
            {
                showLoadingScreen = false,
                unloadSceneOptions = UnloadSceneOptions.None,
            });
            SceneLoader.instance.sceneUnloadedCompleted -= OnInitializationSceneUnloaded;
            SceneLoader.instance.sceneUnloadedCompleted += OnInitializationSceneUnloaded;
        }

        void OnInitializationSceneUnloaded(SceneSO arg1, SceneUnloadSettings arg2)
        {
            SceneLoader.instance.sceneUnloadedCompleted -= OnInitializationSceneUnloaded;
            SceneLoader.instance.LoadScene(sceneListSO.GetSceneByContainingSceneName("MainMenu"), SceneLoadSettings.GetDefault());
        }
    }
}
