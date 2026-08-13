using System;
using XIV.Core.Extensions;
using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    [Serializable]
    public struct NodeComp : IComponent
    {
        public int configIdx;
        public int unitEpoch;
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
            ChangeType(GetComponent<GameObjectEntity>().entity, 0f, AssetReferences.DEFEND_CONFIG);
        }

        [Button]
        void ChangeTypeToAdc()
        {
            ChangeType(GetComponent<GameObjectEntity>().entity, 0f, AssetReferences.RESOURCE_GENERATOR_CONFIG);
        }
        
        [Button]
        void ChangeTypeToDefault()
        {
            ChangeType(GetComponent<GameObjectEntity>().entity, 0f, 0);
        }
        
        static void ChangeType(Entity entity, float penalty, int config)
        {
            entity.world.NewEntity().AddComponent(new NodeChangeTypeEventComp
            {
                nodeEntity = entity,
                unitEntity = entity.GetComponent<OccupiedNodeComp>().unitEntity,
                penalty = penalty,
                newConfig = config,
                unitEpoch = entity.GetComponent<NodeComp>().unitEpoch,
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
