using System;
using TheGame;
using UnityEngine;

namespace XIV.Ecs
{
    [Serializable]
    public class AssetReferences
    {
        public GameObject resourceEntity;
        public GameObject nodeHighlightEntity;
        public GameObject nodeEntity;
        public GameObject connectionLineRendererPrefab;
        public GameObject shieldLineRendererPrefab;
        public GenerationStepSO[] generationConfigs;
    }
    
    public class AssetReferencesMono : MonoBehaviour
    {
        public AssetReferences assetReferences;
    }
}