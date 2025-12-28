using System;
using UnityEngine;
using XIV.Ecs;

namespace TheGame
{

    public enum NodeButtonBehaviourType
    {
        CHANGE_TYPE_DEFAULT = 0,
        CHANGE_TYPE_ADC = 1,
        CHANGE_TYPE_TANK = 2,
        SEND_HALF_RESOURCE = 4,
    }

    [Serializable]
    public struct NodeBehaviourButtonComp : IComponent
    {
        public NodeButtonBehaviourType buttonBehaviourType;
    }
    
    public class SerializedNodeBehaviourButtonComp : SerializedComponent<NodeBehaviourButtonComp>
    {
        
    }
}