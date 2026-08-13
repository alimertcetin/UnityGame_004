using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    public struct SceneLoadSettings
    {
        public bool showLoadingScreen;
        public bool activateSceneAfterLoad;
        public LoadSceneMode loadSceneMode;

        public static SceneLoadSettings GetDefault()
        {
            return new SceneLoadSettings
            {
                showLoadingScreen = true,
                activateSceneAfterLoad = true,
                loadSceneMode = LoadSceneMode.Additive,
            };
        }
    }
    
    public struct SceneUnloadSettings
    {
        public bool showLoadingScreen;
        public UnloadSceneOptions unloadSceneOptions;

        public static SceneUnloadSettings GetDefault()
        {
            return new SceneUnloadSettings
            {
                showLoadingScreen = true,
                unloadSceneOptions = UnloadSceneOptions.None,
            };
        }
    }
}