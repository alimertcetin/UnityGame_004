using TheGame.Extensions;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public class NodeOccupySystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp, NodeOccupyComp> nodeOccupyFilter = null;
        readonly Filter<OccupiedNodeComp> occupiedNodeCompFilter = null;
        readonly ConnectionDB connectionDB = null;

        public override void Update()
        {
            bool occupiedAny = false;
            nodeOccupyFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp, ref NodeOccupyComp nodeOccupyComp) =>
            {
                occupiedAny = true;
                var unitEntity = nodeOccupyComp.unitEntity;
                if (nodeEntity.HasComponent<OccupiedNodeComp>())
                {
                    nodeEntity.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().occupiedNodeEntities.Remove(ref nodeEntity);
                }

                ref var attackerUnitComp = ref unitEntity.GetComponent<UnitComp>();

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
                
                attackerUnitComp.occupiedNodeEntities.Add() = nodeEntity;
                nodeEntity.AddComponent(new OccupiedNodeComp
                {
                    unitEntity = unitEntity,
                });
                
                var renderer = transformComp.transform.GetComponent<SpriteRenderer>();
                var ca = renderer.color;
                var cb = UnitIdLookup.GetColor(attackerUnitComp.unitType);
                renderer.CancelTween();
                renderer.XIVTween()
                    .ScaleBounceOnce()
                    .And()
                    .SpriteRendererColor(ca, cb, 0.5f, EasingFunction.SmoothStop3)
                    .Start();
                
                // Handle line renderer visuals and add reevaluate decision tag
                // nodeEntity.AddTag<ReevaluateDecisionTag>();
                
                using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
                int len = connectionDB.GetAllConnectionPairs(nodeEntity, indexBuffer);
                for (int i = 0; i < len; i++)
                {
                    ref var connectionPair = ref connectionDB[indexBuffer[i]];
                    var neighborEntity = connectionPair.GetOpposite(nodeEntity);
                    UnitIdLookup.UnitType neighborUnitType = UnitIdLookup.UnitType.Black;
                    if (neighborEntity.HasComponent<OccupiedNodeComp>())
                    {
                        // neighborEntity.AddTag<ReevaluateDecisionTag>();
                        neighborUnitType = neighborEntity.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().unitType;
                    }
                    if (neighborUnitType == attackerUnitComp.unitType)
                    {
                        connectionPair.lineRenderer.XIVSetColor(UnitIdLookup.GetColor(attackerUnitComp.unitType));
                        continue;
                    }

                    if (nodeEntity == connectionPair.entity1)
                    {
                        connectionPair.lineRenderer.startColor = UnitIdLookup.GetColor(attackerUnitComp.unitType);
                        connectionPair.lineRenderer.endColor = UnitIdLookup.GetColor(neighborUnitType);
                    }
                    else
                    {
                        connectionPair.lineRenderer.startColor = UnitIdLookup.GetColor(neighborUnitType);
                        connectionPair.lineRenderer.endColor = UnitIdLookup.GetColor(attackerUnitComp.unitType);
                    }
                }
            });
            
            nodeOccupyFilter.RemoveComponentAll<SendResourceContinuouslyComp>();
            nodeOccupyFilter.RemoveComponentAll<NodeOccupyComp>();
            if (occupiedAny)
            {
                // occupiedNodeCompFilter.AddTagAll<ReevaluateDecisionTag>();
            }
        }
    }
}