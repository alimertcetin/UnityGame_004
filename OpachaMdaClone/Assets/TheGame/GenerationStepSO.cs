using UnityEngine;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    [CreateAssetMenu(menuName = "TheGame/GenerationStepSO")]
    public class GenerationStepSO : ScriptableObject
    {
        [OnValueChanged(nameof(UpdateGenerationSpeed))]
        public int quantity;
        [OnValueChanged(nameof(UpdateGenerationSpeed))]
        public int duration;
        public int shieldPoints; // default = 5, adc = 0, tank = 14
        public float resourceGenerationSpeed;
        public float shieldGenerationSpeed = 0.5f;
        
        void UpdateGenerationSpeed() => resourceGenerationSpeed = (float)quantity / duration;
    }
}