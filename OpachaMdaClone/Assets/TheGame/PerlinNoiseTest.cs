using System;
using UnityEngine;
using XIV.Core.Algorithm;
using XIV.Core.XIVMath;
using XIV.UnityEngineIntegration;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    [Serializable]
    public struct NoiseSettings
    {
        public int seed;
        public int x;
        public int y;
        [Range(-1f, 1f)]
        public float frequency;
        [Range(1, 4)]
        public int octaves;
        [Range(0f, 1f)]
        public float persistence;
        public float scale;
    }
    
    public class PerlinNoiseTest : MonoBehaviour
    {
        public NoiseSettings settings1d = new NoiseSettings
        {
            seed = 1,
            x = 0,
            y = 0,
            octaves = 4,
            persistence = 0.5f,
        };
        public NoiseSettings settings2d = new NoiseSettings
        {
            seed = 1,
            x = 0,
            y = 0,
            octaves = 4,
            persistence = 0.5f,
        };
        [Range(1, 2), Header("Select the settings that will be used in calculations. 1 is 1d and 2 is 2d")]
        public int mode = 1;

        public bool saturate;
        [Range(0f, 1f)] public float saturation = 0.5f;
        [Min(1)] public int gridSizeX = 256;
        [Min(1)] public int gridSizeY = 256;
        public bool initialized => perlinNoise1d != null && perlinNoise2d != null;
        
        PerlinNoise1d perlinNoise1d;
        PerlinNoise2d perlinNoise2d;

        void Start()
        {
            perlinNoise1d = new PerlinNoise1d(settings1d.seed);
            perlinNoise2d = new PerlinNoise2d(settings2d.seed);
        }

        void OnDrawGizmosSelected()
        {
            if (this.enabled == false) return;
            if (initialized == false) Start();
            
            if (mode == 1) Draw1DPerlinNoise(perlinNoise1d,settings1d);
            else if (mode == 2) Draw2DPerlinNoise(perlinNoise2d, settings2d);
        }
        
        void Draw1DPerlinNoise(PerlinNoise1d perlinNoise1d, NoiseSettings settings)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                var noise = (float)perlinNoise1d.OctaveNoise(settings.x + x, settings.frequency, settings.octaves, settings.persistence, settings.scale);
                if (saturate) noise = noise >= saturation ? 1 : 0;
                Gizmos.color = new Color(noise, noise, noise, 1f);
                var yRange = XIVMathf.Remap(noise, 0f, 1f, -settings.y, settings.y);
                Gizmos.DrawCube(new Vector3(x, yRange, 0), Vector3.one.SetZ(0));
            }
        }

        void Draw2DPerlinNoise(PerlinNoise2d perlinNoise2d, NoiseSettings settings)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    var noise = (float)perlinNoise2d.OctaveNoise(settings.x + x, settings.y + y, settings.frequency, settings.octaves, settings.persistence, settings.scale);
                    if (saturate) noise = noise >= saturation ? 1 : 0;
                    Gizmos.color = new Color(noise, noise, noise, 1f);
                    Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one.SetZ(0));
                }
            }
        }
    }
}