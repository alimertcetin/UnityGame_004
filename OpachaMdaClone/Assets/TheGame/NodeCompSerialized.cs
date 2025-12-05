using System;
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
        public int configIdx;
    }
    
    public class NodeCompSerialized : SerializedComponent<NodeComp>
    {
        [Button]
        void SendResourceToPlayer()
        {
            GetComponent<GameObjectEntity>().entity.AddTag<SendResourceToPlayerTag>();
        }
        
        [Button]
        void SendResourceToAllNeighbors()
        {
            GetComponent<GameObjectEntity>().entity.AddTag<SendResourceToAllNeighborsTag>();
        }
        
        [Button]
        void ChangeTypeToTank()
        {
            GetComponent<GameObjectEntity>().entity.AddComponent(new NodeChangeTypeComp
            {
                penalty = 0f,
                newConfig = AssetReferences.DEFEND_CONFIG,
            });
        }
        
        [Button]
        void ChangeTypeToAdc()
        {
            GetComponent<GameObjectEntity>().entity.AddComponent(new NodeChangeTypeComp
            {
                penalty = 0f,
                newConfig = AssetReferences.RESOURCE_GENERATOR_CONFIG,
            });
        }
        
        [Button]
        void ChangeTypeToDefault()
        {
            GetComponent<GameObjectEntity>().entity.AddComponent(new NodeChangeTypeComp
            {
                penalty = 0f,
                newConfig = 0,
            });
        }
    }
    
    public struct SendResourceToPlayerTag : ITag { }
    public struct SendResourceToAllNeighborsTag : ITag { }
}
