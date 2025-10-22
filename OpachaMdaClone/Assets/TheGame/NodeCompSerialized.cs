using System;
using TMPro;
using XIV.Core.Collections;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

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
        public UnitIdLookup.UnitType unitType;
        public TMP_Text txt_quantity;
        public float resourceQuantity;
        public float shieldPoints;
        public float totalShieldPoints;
    }
    
    public class NodeCompSerialized : SerializedComponent<NodeComp>
    {
    }
}
