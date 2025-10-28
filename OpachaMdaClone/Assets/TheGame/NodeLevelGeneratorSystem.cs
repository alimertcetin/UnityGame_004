using System;
using System.IO;
using UnityEditor;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class NodeLevelGeneratorSystem : XIV.Ecs.System
    {
        readonly LevelSettings levelSettings = null;
        readonly PrefabReferences prefabReferences = null;
        readonly ConnectionDB connectionDB = null;

        public override void Start()
        {
            int seed = 790;
            var tightness = XIVMathf.Max(1 - levelSettings.tightness, 0.001f);
            var sameDirectionCutThreshold = XIVMathf.Max(1f - levelSettings.sameDirectionCutThreshold, 0.001f);
            new LevelGenerator(new LevelGenerationSettings(levelSettings.mapSize, seed, tightness, sameDirectionCutThreshold, 0.5f), world, prefabReferences,connectionDB).GenerateLevel();
            SaveSeed(seed);
        }

        void SaveSeed(int seedInt)
        {
            var seedStr = DateTime.Now.ToShortTimeString() + " - Seed:" + seedInt.ToString() + Environment.NewLine;
            var path = Path.Combine("Assets", "GenerationSeeds");
            Directory.CreateDirectory(path);
            File.AppendAllText(Path.Combine(path, "Seed.txt"), seedStr);
            AssetDatabase.Refresh();
        }
    }
}