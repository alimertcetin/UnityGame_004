using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using XIV.Ecs;

namespace TheGame
{
    public class NodeLevelGeneratorSystem : XIV.Ecs.System
    {
        readonly LevelSettings levelSettings = null;
        readonly AssetReferences assetReferences = null;
        readonly ConnectionDB connectionDB = null;

        public override void Update()
        {
            manager.ChangeState(EasyLevelController.States.Game);
            new LevelGenerator(levelSettings.levelGenerationSettings, world, assetReferences,connectionDB).GenerateLevel();
            SaveSeed(levelSettings.levelGenerationSettings.seed);
        }

        void SaveSeed(int seedInt)
        {
#if UNITY_EDITOR
            var seedStr = DateTime.Now + " - Seed:" + seedInt.ToString() + Environment.NewLine;
            var path = Path.Combine("Assets", "GenerationSeeds");
            Directory.CreateDirectory(path);
            File.AppendAllText(Path.Combine(path, "Seed.txt"), seedStr);
            AssetDatabase.Refresh();
            Debug.Log("seed = " + seedInt);
#endif
        }
    }
}