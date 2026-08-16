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
#if UNITY_EDITOR
                showLoadingScreen = false,
#else          
                showLoadingScreen = true,
#endif
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
                
#if UNITY_EDITOR
                showLoadingScreen = false,
#else          
                showLoadingScreen = true,
#endif
                unloadSceneOptions = UnloadSceneOptions.None,
            };
        }
    }
}