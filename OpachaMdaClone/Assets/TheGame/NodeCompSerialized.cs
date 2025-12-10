using System;
using XIV.Core.Extensions;
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
        public bool isChangingType;
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
            var entity = GetComponent<GameObjectEntity>().entity;
            entity.world.NewEntity().AddComponent(new NodeChangeTypeEventComp
            {
                nodeEntity = entity,
                unitEntity = entity.GetComponent<OccupiedNodeComp>().unitEntity,
                penalty = 0f,
                newConfig = AssetReferences.DEFEND_CONFIG,
            });
        }
        
        [Button]
        void ChangeTypeToAdc()
        {
            var entity = GetComponent<GameObjectEntity>().entity;
            entity.world.NewEntity().AddComponent(new NodeChangeTypeEventComp
            {
                nodeEntity = entity,
                unitEntity = entity.GetComponent<OccupiedNodeComp>().unitEntity,
                penalty = 0f,
                newConfig = AssetReferences.RESOURCE_GENERATOR_CONFIG,
            });
        }
        
        [Button]
        void ChangeTypeToDefault()
        {
            var entity = GetComponent<GameObjectEntity>().entity;
            entity.world.NewEntity().AddComponent(new NodeChangeTypeEventComp
            {
                nodeEntity = entity,
                unitEntity = entity.GetComponent<OccupiedNodeComp>().unitEntity,
                penalty = 0f,
                newConfig = 0,
            });
        }

        [Button]
        void OccupyForPlayer()
        {
            var entity = GetComponent<GameObjectEntity>().entity;
            var units = FindObjectsOfType<UnitCompSerialized>().AsXIVMemory();
            var unit = units.FilterBy(p => p.GetComponent<GameObjectEntity>().entity.GetComponent<UnitComp>().unitType == UnitIdLookup.UnitType.Green)[0].GetComponent<GameObjectEntity>().entity;
            entity.world.NewEntity().AddComponent(new NodeOccupyEventComp
            {
                nodeEntity = entity,
                unitEntity = unit,
            });
        }
    }
    
    public struct SendResourceToPlayerTag : ITag { }
    public struct SendResourceToAllNeighborsTag : ITag { }
}
