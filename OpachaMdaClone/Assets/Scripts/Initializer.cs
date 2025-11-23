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
            asyncOperation!.allowSceneActivation = true;
        }
    }
}
