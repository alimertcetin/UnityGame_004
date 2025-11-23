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
        readonly AssetReferences assetReferences = null;
        readonly ConnectionDB connectionDB = null;

        public override void Start()
        {
            new LevelGenerator(levelSettings.levelGenerationSettings, world, assetReferences,connectionDB).GenerateLevel();
            SaveSeed(levelSettings.levelGenerationSettings.seed);
        }

        void SaveSeed(int seedInt)
        {
            var seedStr = DateTime.Now + " - Seed:" + seedInt.ToString() + Environment.NewLine;
            var path = Path.Combine("Assets", "GenerationSeeds");
            Directory.CreateDirectory(path);
            File.AppendAllText(Path.Combine(path, "Seed.txt"), seedStr);
            AssetDatabase.Refresh();
        }
    }
}