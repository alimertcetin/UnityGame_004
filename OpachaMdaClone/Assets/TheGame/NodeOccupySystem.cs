using TheGame.Extensions;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct OccupiedNodeComp : IComponent
    {
        public Entity unitEntity;
    }

    public class NodeOccupySystem : XIV.Ecs.System
    {
        readonly Filter<NodeOccupyEventComp> occupyFilter = null;

        public override void Update()
        {
            occupyFilter.ForEach(OccupyNode);
        }

        void OccupyNode(Entity entity, ref NodeOccupyEventComp nodeOccupyEventComp)
        {
            entity.Destroy();
            world.NewEntity().AddComponent(new UpdateVisualLineConnectionEventComp
            {
                targetEntity = nodeOccupyEventComp.nodeEntity,
            });
            if (nodeOccupyEventComp.nodeEntity.HasComponent<OccupiedNodeComp>())
            {
                ref var occupiedNodeComp = ref nodeOccupyEventComp.nodeEntity.GetComponent<OccupiedNodeComp>();
                occupiedNodeComp.unitEntity.GetComponent<UnitComp>().occupiedNodeEntities.Remove(ref nodeOccupyEventComp.nodeEntity);
            }
            ref var attackerUnitComp = ref nodeOccupyEventComp.unitEntity.GetComponent<UnitComp>();
            attackerUnitComp.occupiedNodeEntities.Add() = nodeOccupyEventComp.nodeEntity;

            nodeOccupyEventComp.nodeEntity.RemoveComponent<SendResourceContinuouslyComp>();
            nodeOccupyEventComp.nodeEntity.RemoveComponent<NodeHelpFrontierComp>();
            
            ref var nodeComp = ref nodeOccupyEventComp.nodeEntity.GetComponent<NodeComp>();
            nodeComp.isChangingType = nodeComp.configIdx != 0;
            
            nodeOccupyEventComp.nodeEntity.AddComponent(new OccupiedNodeComp
            {
                unitEntity = nodeOccupyEventComp.unitEntity,
            });
            
            world.NewEntity().AddComponent(new NodeChangeTypeEventComp
            {
                nodeEntity = nodeOccupyEventComp.nodeEntity,
                unitEntity = nodeOccupyEventComp.unitEntity,
                penalty = 0f,
                newConfig = 0,
            });
            
            if (attackerUnitComp.unitType == UnitIdLookup.UnitType.Green)
            {
                nodeOccupyEventComp.nodeEntity.RemoveComponent<NodeDecisionComp>();
            }
            else
            {
                var nodeDecisionComp = new NodeDecisionComp();
                nodeDecisionComp.decisionDelay = new Timer(XIVMathf.Max(1f - attackerUnitComp.smartness01, 0.1f));
                nodeOccupyEventComp.nodeEntity.AddComponent(nodeDecisionComp);
            }

            nodeOccupyEventComp.nodeEntity.GetComponent<ResourceComp>().isGeneratingResource = attackerUnitComp.unitType != UnitIdLookup.UnitType.Black;
        }

    }
}