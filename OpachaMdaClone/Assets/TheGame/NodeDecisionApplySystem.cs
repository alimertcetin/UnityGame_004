using System;
using System.Threading;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Ecs;
using Timer = XIV.Core.Utils.Timer;

namespace TheGame
{
    public class NodeDecisionApplySystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp> nodeApplyDecisionFilter = null;
        readonly ConnectionDB connectionDB = null;

        public override void Awake()
        {
            NodePathFinder.Init();
        }

        public override void Update()
        {
            nodeApplyDecisionFilter.ForEach(ApplyDecision);
        }

        void ApplyDecision(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp)
        {
            if (nodeDecisionComp.decisionChanged)
            {
                entity.RemoveComponent<NodeDefendComp>();
                entity.RemoveComponent<NodeCaptureComp>();
            }
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>();

            switch (nodeDecisionComp.decisionType)
            {
                case DecisionType.Defend:
                    entity.AddComponent(new NodeDefendComp());
                    if (nodeDecisionComp.decisionChanged)
                    {
                        entity.RemoveComponent<SendResourceContinuouslyComp>();
                        // world.NewEntity().AddComponent(new RemoveResourceTransferIndicatorEventComp
                        // {
                        //     ownerEntity = entity,
                        // });
                    }

                    if (nodeDecisionComp.decisionChanged)
                    {
                        if (nodeComp.configIdx != AssetReferences.DEFEND_CONFIG)
                        {
                            entity.AddComponent(new NodeChangeTypeComp
                            {
                                penalty = 10f,
                                newConfig = AssetReferences.DEFEND_CONFIG,
                            });
                        }
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
                    entity.AddComponent(new FrontierHelperComp());

                    if (nodeDecisionComp.decisionChanged)
                    {
                        if (nodeComp.configIdx != AssetReferences.RESOURCE_GENERATOR_CONFIG)
                        {
                            entity.AddComponent(new NodeChangeTypeComp
                            {
                                penalty = 10f,
                                newConfig = AssetReferences.RESOURCE_GENERATOR_CONFIG,
                            });
                        }
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

    public struct PathFinderComp : IComponent
    {
        public DynamicArray<Entity> path;
    }

    public class NodePathFindSystem : XIV.Ecs.System
    {
        readonly Filter<OccupiedNodeComp, PathFinderComp> pathFinderFilter = null;
        readonly ConnectionDB connectionDB = null;
        Timer pathFindTimer = new Timer(2f);

        public override void Update()
        {
            if (pathFindTimer.Update(XTime.deltaTime) == false) return;
            pathFindTimer.Restart();
            
            pathFinderFilter.ForEach((Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref PathFinderComp pathFinderComp) =>
            {
                NodePathFinder.GetPathToFirstTarget(entity, connectionDB, GameConstants.MAX_RESOURCE_QUANTITY, ref pathFinderComp.path);
            });
        }
    }
}