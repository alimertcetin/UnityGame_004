using System;
using UnityEngine;

namespace XIV.Ecs
{
    [Serializable]
    public class LevelSettings
    {
        public int seed = 1;
        public float scale = 0.25f;
        [Min(0.01f)] public float frequency = 0.25f;
        [Range(0, 4)] public int octaves = 2;
        public int gridSizeX = 128;
        public int gridSizeY = 128;
        [Min(0)]
        public float noiseSaturation;

        [Range(0f, 1f)]
        public float animationSpeed = 0.1f;
    }
    
    public class LevelSettingsMono : MonoBehaviour
    {
        public LevelSettings levelSettings;
    }
}