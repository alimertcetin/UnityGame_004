using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    public struct SceneLoadSettings
    {
        public bool showLoadingScreen;
        public bool unloadActiveScene;
        public bool activateSceneAfterLoad;
        public LoadSceneMode loadSceneMode;

        public static SceneLoadSettings GetDefault()
        {
            return new SceneLoadSettings
            {
                showLoadingScreen = true,
                unloadActiveScene = true,
                activateSceneAfterLoad = true,
                loadSceneMode = LoadSceneMode.Additive,
            };
        }
    }
}