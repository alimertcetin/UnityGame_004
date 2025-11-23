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
        public GenerationStepSO[] generationConfigs; // 0 = default, 1 = adc, 2 = tank
        public GameObject nodeTypeChangeParticle;

        public int GetConfigIndex(DecisionType decisionType)
        {
            return decisionType switch
            {
                DecisionType.Defend => 2,
                DecisionType.HelpFrontier => 1,
                _ => -1
            };
        }
    }
    
    public class AssetReferencesMono : MonoBehaviour
    {
        public AssetReferences assetReferences;
    }
}