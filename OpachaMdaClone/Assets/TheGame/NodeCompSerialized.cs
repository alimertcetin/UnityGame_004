using System;
using TMPro;
using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public enum NodeType
    {
        Default,
        ADC,
        Tank,
    }

    [Serializable]
    public struct NodeComp : IComponent
    {
        public TMP_Text txt_quantity;
        public int configIdx;
        public float resourceQuantity;
        public float shieldPoints;
    }
    
    public class NodeCompSerialized : SerializedComponent<NodeComp>
    {
        [Button]
        void AddReevaluateTag()
        {
            // GetComponent<GameObjectEntity>().entity.AddTag<ReevaluateDecisionTag>();
        }
    }

    public struct ResourceGeneratorComp : IComponent
    {
        public float resourceGenerationSpeed;
        public float resourceQuantity;
    }

    public struct ShieldGeneratorComp : IComponent
    {
        public float shieldGenerationSpeed;
        public float shieldPoints;
        public float totalShieldPoints;
    }
}
