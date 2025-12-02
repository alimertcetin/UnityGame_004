using System;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public class NodeDecisionApplySystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp> nodeApplyDecisionFilter = new Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp>().Tag<NodeDecidedTag>();
        readonly ConnectionDB connectionDB = null;

        public override void Awake()
        {
            NodePathFinder.Init();
        }

        public override void Update()
        {
            nodeApplyDecisionFilter.ForEach(ApplyDecision);
            nodeApplyDecisionFilter.RemoveTagAll<NodeDecidedTag>();
        }

        void ApplyDecision(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp)
        {
            if (nodeDecisionComp.decisionChanged)
            {
                entity.RemoveComponent<NodeDefendComp>();
                entity.RemoveComponent<NodeCaptureComp>();
                entity.RemoveComponent<NodeHelpFrontierComp>();
            }
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>();

            switch (nodeDecisionComp.decisionType)
            {
                case DecisionType.Defend:
                    entity.AddComponent(new NodeDefendComp());
                    if (nodeDecisionComp.decisionChanged)
                    {
                        if (entity.HasComponent<SendResourceContinuouslyComp>()) entity.AddTag<StopContinuousResourceTransferTag>();
                    }
                    if (nodeComp.configIdx != AssetReferences.DEFEND_CONFIG)
                    {
                        entity.AddComponent(new NodeChangeTypeComp
                        {
                            penalty = 10f,
                            newConfig = AssetReferences.DEFEND_CONFIG,
                        });
                    }
                    break;
                case DecisionType.Capture:
                    var entityToCapture = GetEntityToCapture(entity, ref nodeComp, ref occupiedNodeComp);
                    if (entityToCapture.IsAlive() == false) break;
                    entity.AddComponent(new NodeCaptureComp
                    {
                        targetEntity = entityToCapture,
                    });
                    break;
                case DecisionType.HelpFrontier:

                    var path = NodePathFinder.GetPathToFirstTarget(entity, connectionDB, GameConstants.MAX_RESOURCE_QUANTITY);
                    var pathLength = path.Length;
                    if (pathLength < 2)
                    {
                        // TODO: Add new component for path calculation
                        Debug.LogWarning("NodeDecisionApplySystem: path is too short");
                        break;
                    }

                    var target = path[pathLength - 1];
                    var frontier = path[pathLength - 2];
                    // var self = path[0]; // current entity  // first is current entity, second is the closest neighbor
                    var neighbor = path[1];

                    if (entity.HasComponent<NodeHelpFrontierComp>())
                    {
                        entity.AddComponent(new HelpFrontierTargetChangeComp
                        {
                            newNeighborEntity = neighbor,
                            newFrontierEntity = frontier,
                            newTargetEntity = target,
                        });
                    }
                    else
                    {
                        entity.AddComponent(new NodeHelpFrontierComp
                        {
                            neighborEntity = neighbor,
                            frontierEntity = frontier,
                            targetEntity = target,
                            targetChangeDelayTimer = new Timer((1f - occupiedNodeComp.unitEntity.GetComponent<UnitComp>().smartness01) * 5f),
                        });
                    }

                    if (nodeComp.configIdx != AssetReferences.RESOURCE_GENERATOR_CONFIG)
                    {
                        entity.AddComponent(new NodeChangeTypeComp
                        {
                            penalty = 10f,
                            newConfig = AssetReferences.RESOURCE_GENERATOR_CONFIG,
                        });
                    }
                    break;
                case DecisionType.Idle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Entity GetEntityToCapture(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp)
        {
            // Only consider Hostile/Neutral nodes for occupation
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            int connectionPairsLen = connectionDB.GetHostileAndNeutralPairs(entity, occupiedNodeComp.unitEntity, indexBuffer);
            Entity targetEntity = Entity.Invalid;
            float captureScore = float.MaxValue;
            for (int i = 0; i < connectionPairsLen; i++)
            {
                ref var pair = ref connectionDB[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                ref var oppositeResourceComp = ref opposite.GetComponent<ResourceComp>();
                    
                var oppositeShieldPoints = 0f;
                if (opposite.HasComponent<ShieldComp>()) oppositeShieldPoints = opposite.GetComponent<ShieldComp>().current;
                
                var requiredResourceQuantity = oppositeResourceComp.resourceQuantity + oppositeShieldPoints;
                if (captureScore > requiredResourceQuantity)
                {
                    targetEntity = opposite;
                    captureScore = requiredResourceQuantity;
                }
            }

            return targetEntity;
        }
    }
}