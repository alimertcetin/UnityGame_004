using TheGame.Extensions;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct OccupiedNodeComp : IComponent
    {
        public Entity unitEntity;
    }
    
    public struct UpdateVisualLineConnectionTag : ITag { }

    public class NodeOccupySystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeOccupyComp> occupyFilter = null;

        public override void Update()
        {
            occupyFilter.ForEach(OccupyNode);
        }

        void OccupyNode(Entity nodeEntity, ref NodeOccupyComp nodeOccupyComp)
        {
            nodeEntity.AddTag<UpdateVisualLineConnectionTag>();
            nodeEntity.RemoveComponent<NodeOccupyComp>();
            if (nodeEntity.HasComponent<OccupiedNodeComp>())
            {
                ref var occupiedNodeComp = ref nodeEntity.GetComponent<OccupiedNodeComp>();
                occupiedNodeComp.unitEntity.GetComponent<UnitComp>().occupiedNodeEntities.Remove(ref nodeEntity);
            }

            if (nodeEntity.HasComponent<SendResourceContinuouslyComp>()) nodeEntity.AddTag<StopContinuousResourceTransferTag>();
            nodeEntity.RemoveComponent<SendResourceComp>();
            nodeEntity.RemoveComponent<NodeChangeTypeComp>();
            
            ref var nodeComp = ref nodeEntity.GetComponent<NodeComp>();
            ref var attackerUnitComp = ref nodeOccupyComp.unitEntity.GetComponent<UnitComp>();
            attackerUnitComp.occupiedNodeEntities.Add() = nodeEntity;
            nodeEntity.AddComponent(new OccupiedNodeComp
            {
                unitEntity = nodeOccupyComp.unitEntity,
            });
            nodeEntity.AddComponent(new NodeChangeTypeComp
            {
                penalty = 0f,
                newConfig = 0,
            });
            
            if (attackerUnitComp.unitType == UnitIdLookup.UnitType.Green)
            {
                nodeEntity.RemoveComponent<NodeDecisionComp>();
            }
            else
            {
                var nodeDecisionComp = new NodeDecisionComp();
                nodeDecisionComp.decisionDelay = new Timer(1f - attackerUnitComp.smartness01);
                nodeEntity.AddComponent(nodeDecisionComp);
            }

            if (attackerUnitComp.unitType == UnitIdLookup.UnitType.Black)
            {
                nodeEntity.RemoveComponent<ResourceGeneratorComp>();
            }
            else
            {
                nodeEntity.AddComponent(new ResourceGeneratorComp
                {
                    resourceGenerationSpeed = assetReferences.generationConfigs[nodeComp.configIdx].resourceGenerationSpeed,
                });
            }
        }

    }
}