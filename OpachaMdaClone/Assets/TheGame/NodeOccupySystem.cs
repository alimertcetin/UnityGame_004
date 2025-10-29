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
        readonly PrefabReferences prefabReferences = null;
        readonly ConnectionDB connectionDB = null;

        public override void Update()
        {
            nodeOccupyFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp, ref NodeOccupyComp nodeOccupyComp) =>
            {
                var unitEntity = nodeOccupyComp.unitEntity;
                ref var unitComp = ref unitEntity.GetComponent<UnitComp>();
                unitComp.occupiedNodeEntities.Add() = nodeEntity;
                nodeComp.unitType = unitComp.unitType;
                nodeEntity.AddComponent(new OccupiedNodeComp
                {
                    unitEntity = unitEntity,
                    resourceGenerationSpeed = prefabReferences.generationConfigs[0].generationSpeed, // the default config on unitComp
                });
                var renderer = transformComp.transform.GetComponent<SpriteRenderer>();
                var ca = renderer.color;
                var cb = UnitIdLookup.GetColor(nodeComp.unitType);
                renderer.CancelTween();
                renderer.XIVTween()
                    .ScaleBounceOnce()
                    .And()
                    .SpriteRendererColor(ca, cb, 0.5f, EasingFunction.SmoothStop3)
                    .Start();
                
                // Handle line renderer visuals
                using var dispose = ArrayUtils.GetBuffer(out ConnectionPair[] buffer, 16);
                int len = connectionDB.GetPairs(nodeEntity, buffer);
                for (int i = 0; i < len; i++)
                {
                    ref var connectionPair = ref buffer[i];
                    var e2 = connectionPair.GetOpposite(nodeEntity);
                    ref var e2NodeComp = ref e2.GetComponent<NodeComp>();
                    if (e2NodeComp.unitType == nodeComp.unitType)
                    {
                        connectionPair.lineRenderer.XIVSetColor(UnitIdLookup.GetColor(nodeComp.unitType));
                        continue;
                    }

                    if (nodeEntity == connectionPair.entity1)
                    {
                        connectionPair.lineRenderer.startColor = UnitIdLookup.GetColor(nodeComp.unitType);
                        connectionPair.lineRenderer.endColor = UnitIdLookup.GetColor(e2NodeComp.unitType);
                    }
                    else
                    {
                        connectionPair.lineRenderer.startColor = UnitIdLookup.GetColor(e2NodeComp.unitType);
                        connectionPair.lineRenderer.endColor = UnitIdLookup.GetColor(nodeComp.unitType);
                    }
                }
            });
            
            nodeOccupyFilter.RemoveComponentAll<NodeOccupyComp>();
        }
    }
}