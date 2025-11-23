using System;
using System.Collections.Generic;
using TheGame.Extensions;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct NodeDecisionComp : IComponent
    {
        public DecisionType decisionType;
        public float dangerScore;
        public float defendScore;
        public float captureScore;
        public float helpFrontierScore;
        public float idleScore;
        public Timer decisionDelay;
    }
    
    public struct NodeDefendComp : IComponent{}

    public struct NodeCaptureComp : IComponent
    {
        public Entity targetEntity;
    }

    public struct NodeHelpFrontierComp : IComponent
    {
        public Entity closestNeighborEntity;
        public Entity frontierEntity;
        public Entity targetEntity;
    }
    public struct NodeDecidedTag : ITag { }
    
    public enum DecisionType
    {
        Defend,
        Capture,
        HelpFrontier,
        Idle
    }

    public struct DecisionScore
    {
        public DecisionType decisionType;
        public float score;
    }
    
    public class NodeDecisionSystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp> nodeDecisionFilter = new Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp>().ExcludeTag<NodeDecidedTag>();
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            nodeDecisionFilter.ForEach(MakeDecision);
        }
        
        void MakeDecision(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp)
        {
            if (nodeDecisionComp.decisionDelay.Update(XTime.deltaTime) == false) return;
            nodeDecisionComp.decisionDelay.Restart();
            var unitEntity = occupiedNodeComp.unitEntity;
            ref var unitComp = ref unitEntity.GetComponent<UnitComp>();
            float smartness = unitComp.smartness01;

            // ---------------------------
            // DATA GATHERING
            // ---------------------------

            // normalized 0–1
            float dangerScore = connectionDB.GetHostileNeighborResourceQuantity(entity, unitEntity) / GameConstants.MAX_RESOURCE_QUANTITY;
            // float captureRawScore = GetCaptureScore(entity, ref nodeComp, ref occupiedNodeComp); // already normalized 0–1

            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            int allPairs = connectionDB.GetAllConnectionPairs(entity, indexBuffer);

            float hostileRatio = connectionDB.GetHostileConnectionPairs(entity, unitEntity, indexBuffer) / (float)allPairs;
            float allyRatio = connectionDB.GetAlliedConnectionPairs(entity, unitEntity, indexBuffer) / (float)allPairs;
            float neutralRatio = connectionDB.GetNeutralConnectionPairs(entity, indexBuffer) / (float)allPairs;

            // ---------------------------
            // 1. DEFEND SCORE
            // ---------------------------
            // smartness reduces defensive paranoia
            // smartness increases defensive paranoia if there is hostile resource transfer going on towards entity
            float defendQuantity = nodeComp.resourceQuantity + nodeComp.shieldPoints;
            float hostileResourceTransferToNode = connectionDB.GetHostileResourceTransfer(entity, unitEntity);
            float defendFeasibility = hostileResourceTransferToNode / defendQuantity;
            
            float defendBase = Mathf.Max(dangerScore, hostileRatio);
            float defendSmartness = 1f - smartness; // smart = less defensive
            float defendScore = defendFeasibility * defendBase * (1f + defendSmartness); // 1–2 multiplier


            // ---------------------------
            // 2. CAPTURE SCORE
            // ---------------------------
            float captureScore = 0f;

            // Evaluate capture feasibility relative to resource advantage
            {
                using var entityBuffer = ArrayUtils.GetBuffer<Entity>(16);
                var unit = occupiedNodeComp.unitEntity;
                int neighborCount = connectionDB.GetNeighbors(entity, entityBuffer,
                    (opp) => connectionDB.IsTargetHostile(unit, opp) || connectionDB.IsNeutralNode(opp));

                float bestCapture = 0f;
                float have = nodeComp.resourceQuantity;

                for (int i = 0; i < neighborCount; i++)
                {
                    var opposite = entityBuffer[i];
                    ref var oppositeNodeComp = ref opposite.GetComponent<NodeComp>();
                    // var hostileNeighborResourceQuantity = connectionDB.GetHostileNeighborResourceQuantity(opposite, unitEntity);

                    // float required = oppositeNodeComp.resourceQuantity + oppositeNodeComp.shieldPoints + (hostileNeighborResourceQuantity * smartness);
                    float required = oppositeNodeComp.resourceQuantity + oppositeNodeComp.shieldPoints;

                    // SMARTNESS modifies required advantage
                    float requiredAdvantage = Mathf.Lerp(required * 2f, required * 0.9f, smartness);

                    float advantage = have - requiredAdvantage;

                    // Convert to 0–1
                    float feasibility = Mathf.InverseLerp(0f, required * 2f, advantage);
                    feasibility = Mathf.Clamp01(feasibility);

                    // Desire to capture grows when fewer allies & more neutral
                    // float desire = (1f - hostileRatio) * neutralRatio;
                    float desire = 1f - allyRatio;

                    float score = feasibility * desire * (requiredAdvantage * XIVMathf.Max(0.1f, smartness));

                    if (score > bestCapture)
                        bestCapture = score;
                }

                captureScore = bestCapture;
            }


            // ---------------------------
            // 3. HELP FRONTIER SCORE
            // ---------------------------
            float helpBase = allyRatio * (1f - hostileRatio) * (1f - neutralRatio);

            // smart nodes help less (more selective)
            float helpSmartness = Mathf.Lerp(1f, 0.4f, smartness);

            // Do not help if a capture is available
            // Do not help if got hostile neighbor
            float helpFrontierScore = helpBase * helpSmartness * (1f - captureScore) * (hostileRatio > 0 ? 0 : 1);


            // ---------------------------
            // 4. IDLE SCORE (not used directly, but saved if needed)
            // ---------------------------
            float idleScore =
                smartness *
                (1f - defendScore) *
                (1f - captureScore) *
                (1f - helpFrontierScore);

            // float typeChangeScore = 0f;
            // {
            //     float adcBase = XIVMathf.Max(idleScore, helpFrontierScore) * (nodeComp.configIdx == 1 ? 0f : 1f);
            //     float tankBase = defendScore * (nodeComp.configIdx == 2 ? 0f : 1f);
            // }

            // ---------------------------
            // WRITE OUT SCORES
            // ---------------------------
            nodeDecisionComp.dangerScore = dangerScore;
            nodeDecisionComp.defendScore = defendScore;
            nodeDecisionComp.captureScore = captureScore;
            nodeDecisionComp.helpFrontierScore = helpFrontierScore;
            nodeDecisionComp.idleScore = idleScore;
            // idleScore not stored since NodeDecisionApplySystem sets idle = 0

            // Done
            // entity.RemoveTag<ReevaluateDecisionTag>();
            unsafe
            {
                // Bundle and select best
                const int len = 4;
                DecisionScore* scores = stackalloc DecisionScore[len]
                {
                    new() { decisionType = DecisionType.Defend, score = nodeDecisionComp.defendScore },
                    new() { decisionType = DecisionType.Capture, score = nodeDecisionComp.captureScore },
                    // new() { Type = DecisionType.HelpAlly, Score = nodeDecisionComp.allyHelpScore },
                    new() { decisionType = DecisionType.HelpFrontier, score = nodeDecisionComp.helpFrontierScore },
                    new() { decisionType = DecisionType.Idle, score = nodeDecisionComp.idleScore },
                };

                var best = scores[0];
                for (int i = 1; i < len; i++)
                {
                    if (scores[i].score > best.score) best = scores[i];
                }

                var currentDecision = nodeDecisionComp.decisionType;
                if (best.decisionType != currentDecision)
                {
                   entity.AddTag<NodeDecidedTag>();
                }
                nodeDecisionComp.decisionType = best.decisionType;
            }
        }

    }

    public class NodeDecisionApplySystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB;
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp> nodeApplyDecisionFilter = new Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp>().Tag<NodeDecidedTag>();

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
            entity.RemoveComponent<NodeDefendComp>();
            entity.RemoveComponent<NodeCaptureComp>();
            entity.RemoveComponent<NodeHelpFrontierComp>();
            entity.RemoveComponent<SendResourceContinuouslyComp>();
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>();

            switch (nodeDecisionComp.decisionType)
            {
                case DecisionType.Defend:
                    entity.AddComponent(new NodeDefendComp());
                    entity.AddComponent(new NodeTypeChangeComp());
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
                        Debug.LogWarning("NodeDecisionApplySystem: path is too short");
                        break;
                    }

                    var target = path[pathLength - 1];
                    var frontier = path[pathLength - 2];
                    // var self = path[0]; // current entity
                    var neighbor = path[1];

                    entity.AddComponent(new NodeHelpFrontierComp
                    {
                        closestNeighborEntity = neighbor, // first is current entity, second is the closest neighbor
                        frontierEntity = frontier,
                        targetEntity = target,
                    });
                    entity.AddComponent(new NodeTypeChangeComp());
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
                ref var oppositeNodeComp = ref opposite.GetComponent<NodeComp>();
                var requiredResourceQuantity = oppositeNodeComp.resourceQuantity + oppositeNodeComp.shieldPoints;
                if (captureScore > requiredResourceQuantity)
                {
                    targetEntity = opposite;
                    captureScore = requiredResourceQuantity;
                }
            }

            return targetEntity;
        }
    }

    public class NodeCaptureSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp, NodeCaptureComp> nodeCaptureFilter;

        public override void Update()
        {
            nodeCaptureFilter.ForEach(CaptureTarget);
            nodeCaptureFilter.RemoveComponentAll<NodeCaptureComp>();
        }

        void CaptureTarget(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp, ref NodeCaptureComp nodeCaptureComp)
        {
            var targetEntity = nodeCaptureComp.targetEntity;
            if (targetEntity.IsAlive() == false || connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, targetEntity)) return;

            ref var otherNodeComp = ref targetEntity.GetComponent<NodeComp>();

            GenerationStepSO config = assetReferences.generationConfigs[otherNodeComp.configIdx];
            float resourceTravelTime = GetResourceTravelTime();
            float generatedQuantityAtArrival = config.quantity / resourceTravelTime;

            var requiredResource = otherNodeComp.resourceQuantity + otherNodeComp.shieldPoints + (generatedQuantityAtArrival * occupiedNodeComp.unitEntity.GetComponent<UnitComp>().smartness01);
            var diff = nodeComp.resourceQuantity - requiredResource;
            // we don't have enough resource to takeover
            if (diff < 0) return;
            
            // int connectionIndex = connectionDB.GetConnectionIndex(entity, nodeCaptureComp.targetEntity);
            var allyResourceTransfer = connectionDB.GetAllyResourceTransfer(targetEntity, occupiedNodeComp.unitEntity);
            var hostileResourceTransfer = connectionDB.GetHostileResourceTransfer(targetEntity, occupiedNodeComp.unitEntity);
            var totalAlly = allyResourceTransfer - hostileResourceTransfer;
            // we already send required amount of resource
            if (totalAlly >= requiredResource) return;

            entity.AddComponent(new SendResourceComp
            {
                resourceQuantity = (int)nodeComp.resourceQuantity,
                toEntity = targetEntity,
            });
            // entity.AddTag<ReevaluateDecisionTag>();

            float GetResourceTravelTime()
            {
                var p0 = entity.GetComponent<PositionComp>().position;
                var p1 = targetEntity.GetComponent<PositionComp>().position;
                var distance = Vec3.Distance(p0, p1);
                return distance / GameConstants.RESOURCE_TRAVEL_SPEED;
            }
        }
    }
    
    public class NodeHelpFrontierSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp, NodeHelpFrontierComp> nodeHelpFrontierSendResourceFilter = new Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp, NodeHelpFrontierComp>().Exclude<SendResourceContinuouslyComp>().Exclude<NodeTypeChangeComp>();
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp, NodeHelpFrontierComp> nodeHelpFrontierFilter = null;
        readonly Queue<LineRenderer> inactiveLineRenderers = new Queue<LineRenderer>();
    
        public override void Update()
        {
            nodeHelpFrontierSendResourceFilter.ForEach(SendResourceToFrontier);
            nodeHelpFrontierFilter.ForEach(CheckValidity);
        }

        void SendResourceToFrontier(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            entity.AddComponent(new SendResourceContinuouslyComp
            {
                currentDuration = 0f,
                duration = assetReferences.generationConfigs[nodeComp.configIdx].duration,
                toEntity = nodeHelpFrontierComp.closestNeighborEntity,
            });
        }

        void CheckValidity(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            if (connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, nodeHelpFrontierComp.frontierEntity) == false 
                || connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, nodeHelpFrontierComp.closestNeighborEntity) == false
                || connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, nodeHelpFrontierComp.targetEntity))
            {
                entity.RemoveComponent<SendResourceContinuouslyComp>();
                nodeDecisionComp.decisionType = DecisionType.Idle;
            }
        }
    
        bool IsRouteSafe(XIVMemory<Entity> path, Entity unitEntity, out float normalizedHostilesOnPath)
        {
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>();
            int totalNeighborCount = 0;
            int totalHostileCount = 0;
            int len = path.Length - 1; // last is target
            for (int i = 0; i < len; i++)
            {
                totalNeighborCount += connectionDB.GetAllNeighbors(path[i], entityBuffer);
                totalHostileCount += connectionDB.GetHostileNeighbors(path[i], unitEntity, entityBuffer);
            }
            normalizedHostilesOnPath = totalHostileCount / (float)totalNeighborCount;
            return normalizedHostilesOnPath < 0.25f && len > 0;
        }
    
        XIVMemory<Entity> CreatePathAndRenderer(Entity entity, Entity targetEntity, Entity unitEntity)
        {
            var path = NodePathFinder.GetPathToTarget(entity, targetEntity, connectionDB, GameConstants.MAX_RESOURCE_QUANTITY);
            int len = path.Length - 1; // last is target
            if (len == 0) return path;
            
            if (inactiveLineRenderers.TryDequeue(out var renderer) == false)
            {
                renderer = UnityEngine.Object.Instantiate(assetReferences.connectionLineRendererPrefab).GetComponent<LineRenderer>();
                renderer.XIVSetWidth(1f);
            }
            renderer.startColor = UnitIdLookup.GetColor(unitEntity.GetComponent<UnitComp>().unitType);
            renderer.endColor = Color.black;
            renderer.gameObject.SetActive(true);
            renderer.positionCount = len;
            for (int i = 0; i < len; i++)
            {
                renderer.SetPosition(i, path[i].GetComponent<TransformComp>().transform.position);
            }
                
            world.NewEntity().AddComponent(new CallLaterComp
            {
                timer = 1f,
                action = (e)=>
                {
                    inactiveLineRenderers.Enqueue(renderer);
                    renderer.gameObject.SetActive(false);
                    e.Destroy();
                },
            });
            // Debug.Log($"Path created for {entity}");
            return path;
        }
    }

    public struct NodeTypeChangeComp : IComponent
    {
        
    }

    public struct PlayParticleComp : IComponent
    {
    }

    public class NodeTypeChangeSystem : XIV.Ecs.System
    {
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp, NodeTypeChangeComp> aiTypeChangeFilter = null;

        public override void Update()
        {
            aiTypeChangeFilter.ForEach(EvaluateTypeChange);
        }

        void EvaluateTypeChange(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp, ref NodeTypeChangeComp nodeTypeChangeComp)
        {
            const float penalty = 10f;
            var diff = nodeComp.resourceQuantity - penalty;
            // We can't change type yet
            if (diff <= 0f) return;

            int configIdx = assetReferences.GetConfigIndex(nodeDecisionComp.decisionType);
            if (configIdx == -1)
            {
                UnityEngine.Debug.LogWarning("Can't change type for node, type: " + nodeDecisionComp.decisionType);
                entity.RemoveComponent<NodeTypeChangeComp>();
                return;
            }
            nodeComp.resourceQuantity -= penalty;
            nodeComp.configIdx = configIdx;
            entity.RemoveComponent<NodeTypeChangeComp>();
            var particleEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeTypeChangeParticle, entity.GetComponent<PositionComp>().position, Quaternion.identity);
            particleEntity.AddComponent(new PlayParticleComp());
            var particleSystem = particleEntity.GetTransform().GetComponent<ParticleSystem>().main;
            particleSystem.startColor = new ParticleSystem.MinMaxGradient(UnitIdLookup.GetColor(occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType));
            particleEntity.AddComponent(new CallLaterComp
            {
                timer = particleSystem.duration,
                action = (e) => e.Destroy(),
            });
        }
    }

    public static class GameConstants
    {
        public const float MAX_RESOURCE_QUANTITY = 10_000f;
        public const float RESOURCE_TRAVEL_SPEED = 2f;
    }
}