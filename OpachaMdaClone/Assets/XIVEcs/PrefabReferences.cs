using TheGame;
using UnityEngine;

namespace XIV.Ecs
{
    public class PrefabReferences : MonoBehaviour
    {
        public GameObject resourceEntity;
        public GameObject nodeHighlightEntity;
        public GameObject nodeEntity;
        public GameObject connectionLineRendererPrefab;
        public Material shieldLineRendererMaterial;
        public GenerationStepSO[] generationConfigs;
    }
}