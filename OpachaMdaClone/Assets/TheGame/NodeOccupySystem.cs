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
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
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

            nodeOccupyEventComp.nodeEntity.RemoveComponent<SendResourceContinuouslyComp>();
            nodeOccupyEventComp.nodeEntity.RemoveComponent<NodeChangeTypeComp>();
            
            ref var attackerUnitComp = ref nodeOccupyEventComp.unitEntity.GetComponent<UnitComp>();
            attackerUnitComp.occupiedNodeEntities.Add() = nodeOccupyEventComp.nodeEntity;
            nodeOccupyEventComp.nodeEntity.AddComponent(new OccupiedNodeComp
            {
                unitEntity = nodeOccupyEventComp.unitEntity,
            });
            nodeOccupyEventComp.nodeEntity.AddComponent(new NodeChangeTypeComp
            {
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

            if (attackerUnitComp.unitType == UnitIdLookup.UnitType.Black)
            {
                nodeOccupyEventComp.nodeEntity.GetComponent<ResourceComp>().isGeneratingResource = false;
            }
            else
            {
                nodeOccupyEventComp.nodeEntity.GetComponent<ResourceComp>().isGeneratingResource = true;
            }
        }

    }
}