using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    [CreateAssetMenu(fileName = nameof(SceneSO), menuName = ScriptableObjectPaths.XIVSceneManagementBaseMenu + nameof(SceneSO))]
    public class SceneSO : ScriptableObject
    {
        public string sceneName;
        public int sceneBuildIndex;
        public string scenePath;
        
#if UNITY_EDITOR
        [SerializeField]
        SceneAsset sceneAsset;

        public void Init(SceneAsset sceneAsset)
        {
            sceneName = sceneAsset.name;
            sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
            scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            this.sceneAsset = sceneAsset;
        }

        public override string ToString()
        {
            return $"{sceneName}, {sceneBuildIndex}";
        }
#endif
    }
}