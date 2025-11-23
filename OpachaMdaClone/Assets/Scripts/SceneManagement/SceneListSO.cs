using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;
using XIV.UnityEngineIntegration;
using XIV.UnityEngineIntegration.XIVEditor.Utils;
#if UNITY_EDITOR
using Directory = UnityEngine.Windows.Directory;
#endif

namespace TheGame.SceneManagement
{
    [CreateAssetMenu(fileName = nameof(SceneListSO), menuName = ScriptableObjectPaths.XIVSceneManagementBaseMenu + nameof(SceneListSO))]
    public class SceneListSO : ScriptableObject
    {
        [SerializeField] List<SceneSO> sceneSOs;
        public IReadOnlyList<SceneSO> SceneSOs => sceneSOs;

        public SceneSO GetSceneByContainingSceneName(string containingSceneName)
        {
            containingSceneName = containingSceneName.ToLower(CultureInfo.InvariantCulture);
            var count = sceneSOs.Count;
            for (var i = 0; i < count; i++)
            {
                var sceneSO = sceneSOs[i];
                if (sceneSO.sceneName.ToLower(CultureInfo.InvariantCulture).Contains(containingSceneName))
                    return sceneSO;
            }

            return null;
        }
        
#if UNITY_EDITOR

        [Button]
        void GatherAllScenes()
        {
            sceneSOs = new List<SceneSO>();
            var basePath = "Assets/ScriptableObjects/Scenes/";
            var sceneAssets = XIVEditorAssetUtils.LoadAssetsOfType<SceneAsset>("Assets");
            Directory.CreateDirectory(basePath);
            foreach (SceneAsset sceneAsset in sceneAssets)
            {
                var sceneSO = CreateAsset<SceneSO>(basePath + sceneAsset.name);
                sceneSO.Init(sceneAsset);
                EditorUtility.SetDirty(sceneSO);
                sceneSOs.Add(sceneSO);
            }
            EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Creates a ScriptableObject of type T and saves it to the given asset path.
        /// </summary>
        /// <typeparam name="T">Type of ScriptableObject to create.</typeparam>
        /// <param name="assetPath">Path under Assets/ where the object should be saved (e.g. "Assets/Data/MyAsset.asset").</param>
        /// <returns>The created ScriptableObject instance.</returns>
        static T CreateAsset<T>(string assetPath, bool highlightNewAsset = false) where T : ScriptableObject
        {
            // Ensure the directory exists
            string directory = Path.GetDirectoryName(assetPath);
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }

            // Make sure the path ends with ".asset"
            if (assetPath.EndsWith(".asset") == false) assetPath += ".asset";

            var so = XIVEditorAssetUtils.GetScriptableObject<T>(assetPath.Split('/', '\\')[^1].Replace(".asset", ""));
            if (so)
            {
                Debug.Log($"Asset already exist: {so} at {assetPath}");
                return so;
            }
            
            // Create instance
            T asset = ScriptableObject.CreateInstance<T>();
            
            // Save to asset database
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (highlightNewAsset)
            {
                // Highlight the new asset
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }

            Debug.Log($"Created {typeof(T).Name} at {assetPath}");
            return asset;
        }
        
        [Button]
        void AddScenesToBuild()
        {
            var sceneSOs = this.sceneSOs.AsXIVMemory();
            if (sceneSOs.Length == 0)
            {
                Debug.LogWarning("No scenes found under Assets/.");
                return;
            }
            ClearScenesInBuildSettings();
            AddScenesToBuildSettings(sceneSOs);
            GatherAllScenes();
            EditorUtility.SetDirty(this);
        }

        [Button]
        void ClearScenesInBuildSettings()
        {
            EditorBuildSettings.scenes = Array.Empty<EditorBuildSettingsScene>();
        }

        static void AddScenesToBuildSettings(XIVMemory<SceneSO> sceneSOs)
        {
            var length = sceneSOs.Length;
            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[length];
            for (int i = 0; i < length; i++)
            {
                newScenes[i] = new EditorBuildSettingsScene(sceneSOs[i].scenePath, true);
            }
            
            // Assign to build settings
            var list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            list.AddRange(newScenes);
            EditorBuildSettings.scenes = list.ToArray();
            Debug.Log($"Added {sceneSOs.Length} scenes to Build Settings.");
        }
        
#endif
        
    }
}