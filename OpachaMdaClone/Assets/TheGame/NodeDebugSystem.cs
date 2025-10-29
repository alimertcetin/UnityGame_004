using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class NodeDebugSystem : XIV.Ecs.System
    {
        // readonly Filter<TransformComp, NodeComp, OccupiedNodeComp> occupiedNodeFilter = new Filter<TransformComp, NodeComp, OccupiedNodeComp>().Exclude<SendResourceContinuouslyComp>();
        readonly Filter<TransformComp, NodeComp, OccupiedNodeComp> occupiedNodeFilter = null;
        readonly ConnectionDB connectionDB = null;
        readonly PrefabReferences prefabReferences = null;

        public override void Update()
        {
            int count = connectionDB.Count;
            for (int i = 0; i < count; i++)
            {
                ref var connectionPair = ref connectionDB[i];
                if (CanSendResource(connectionPair.entity1, connectionPair.entity2))
                {
                    SendResource(connectionPair.entity1, connectionPair.entity2);
                }
                if (CanSendResource(connectionPair.entity2, connectionPair.entity1))
                {
                    SendResource(connectionPair.entity2, connectionPair.entity1);
                }
            }

            void SendResource(Entity ent1, Entity ent2)
            {
                var duration = prefabReferences.generationConfigs[0].duration;
                ent1.AddComponent(new SendResourceContinuouslyComp
                {
                    currentDuration = duration,
                    duration = duration,
                    toEntity = ent2,
                });
            }

            bool ShouldSwitchTarget(Entity e, ref SendResourceContinuouslyComp sendResourceContinuouslyComp)
            {
                if (sendResourceContinuouslyComp.toEntity.HasComponent<OccupiedNodeComp>() == false) return false;
                
                ref var other = ref sendResourceContinuouslyComp.toEntity.GetComponent<OccupiedNodeComp>();
                return other.unitEntity.GetComponent<UnitComp>().unitType == e.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().unitType;
                // we already occupied the target node
            }

            bool CanSendResource(Entity ent1, Entity ent2)
            {
                if (ent1.HasComponent<SendResourceContinuouslyComp>() && ShouldSwitchTarget(ent1, ref ent1.GetComponent<SendResourceContinuouslyComp>()) == false) return false;

                if (ent1.HasComponent<OccupiedNodeComp>() == false) return false;
                ref var ent1UnitComp = ref ent1.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>();
                // if (ent1UnitComp.unitType == UnitIdLookup.UnitType.Green) return false; // if this is player
                if (ent2.HasComponent<OccupiedNodeComp>() == false) return true;
                ref var ent2UnitComp = ref ent2.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>();
                return ent1UnitComp.unitType != ent2UnitComp.unitType;
            }
        }
    }
}