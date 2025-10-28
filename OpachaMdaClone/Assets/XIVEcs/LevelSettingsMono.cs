using System;
using UnityEngine;
using XIV.UnityEngineIntegration;

namespace XIV.Ecs
{
    public enum MapSize
    {
        Small = 0,
        Medium = 1,
        Big = 2,
        Large = 3,
        Giant = 4,
        NumberOfItems
    }
    [Serializable]
    public class LevelSettings
    {
        [Range(0f, 10f), OnValueChanged(nameof(ChangeTimeScale), true)]
        public float timeScale = 1f;
        public MapSize mapSize = MapSize.Giant;
        [Range(0f, 1f), Tooltip("Higher values generates more strict nodes")]
        public float tightness = 0f;
        [Range(0f, 1f), Tooltip("Cuts the generated links in similar directions. Higher the value lesser the link")]
        public float sameDirectionCutThreshold = 0.8f;

        void ChangeTimeScale()
        {
            XTime.timeScale = timeScale;
        }
    }
    
    public class LevelSettingsMono : MonoBehaviour
    {
        public LevelSettings levelSettings;
    }
}