using System;
using TheGame;
using UnityEngine;
using XIV.Core.XIVMath;
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
        public LevelGenerationSettings levelGenerationSettings = new LevelGenerationSettings(MapSize.Giant, XIVRandom.seed, 0, 0.8f, 0.5f);
        [Range(0f, 10f), OnValueChanged(nameof(ChangeTimeScale), true)]
        public float timeScale = 1f;
        [Range(1, UnitIdLookup.MAX_UNIT_ID_LENGTH - 2)]
        public int hostileUnits = 1;

        void ChangeTimeScale()
        {
            XTime.timeScale = timeScale;
        }

        [Button]
        void RandomSeed()
        {
            levelGenerationSettings.seed = (int)(XIVRandom.value * 100000);
        }
    }
    
    public class LevelSettingsMono : MonoBehaviour
    {
        public LevelSettings levelSettings;

        void Start()
        {
            XTime.timeScale = levelSettings.timeScale;
        }
    }
}