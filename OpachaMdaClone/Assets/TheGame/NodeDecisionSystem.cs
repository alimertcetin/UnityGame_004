using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct NodeDecisionComp : IComponent
    {
        public DecisionType decisionType
        {
            get => _decisionType;
            set
            {
                decisionChanged = value != _decisionType;
                _decisionType = value;
            }
        }

        public bool decisionChanged;
        public float dangerScore;
        public float defendScore;
        public float captureScore;
        public float helpFrontierScore;
        public float idleScore;
        public Timer decisionDelay;

        DecisionType _decisionType;
    }
    
    public struct NodeDefendComp : IComponent{}

    public struct NodeCaptureComp : IComponent
    {
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
        readonly Filter<ResourceComp, OccupiedNodeComp, NodeDecisionComp> nodeDecisionFilter = new Filter<ResourceComp, OccupiedNodeComp, NodeDecisionComp>().ExcludeTag<NodeDecidedTag>();
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            nodeDecisionFilter.ForEach(MakeDecision);
        }
        
        void MakeDecision(Entity entity, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp)
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
                    
            var shieldPoints = 0f;
            if (entity.HasComponent<ShieldComp>()) shieldPoints = entity.GetComponent<ShieldComp>().current;
            float defendQuantity = resourceComp.resourceQuantity + shieldPoints;
            float hostileResourceTransferToNode = connectionDB.GetHostileResourceTransfer(entity, unitEntity);
            float defendFeasibility = XIVMathf.Max(dangerScore, hostileResourceTransferToNode / defendQuantity);
            
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
                int neighborCount = connectionDB.GetNeighbors(entity, entityBuffer,
                    (opp) => connectionDB.IsTargetHostile(unitEntity, opp) || connectionDB.IsNeutralNode(opp));

                float bestCapture = 0f;
                float have = resourceComp.resourceQuantity;

                for (int i = 0; i < neighborCount; i++)
                {
                    var opposite = entityBuffer[i];
                    ref var oppositeResourceComp = ref opposite.GetComponent<ResourceComp>();
                    
                    var oppositeShieldPoints = 0f;
                    if (opposite.HasComponent<ShieldComp>()) oppositeShieldPoints = opposite.GetComponent<ShieldComp>().current;
                    
                    // var hostileNeighborResourceQuantity = connectionDB.GetHostileNeighborResourceQuantity(opposite, unitEntity);

                    // float required = oppositeNodeComp.resourceQuantity + oppositeNodeComp.shieldPoints + (hostileNeighborResourceQuantity * smartness);
                    float required = oppositeResourceComp.resourceQuantity + oppositeShieldPoints;

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

            // Done
            unsafe
            {
                // Bundle and select best
                const int len = 4;
                DecisionScore* scores = stackalloc DecisionScore[len]
                {
                    new() { decisionType = DecisionType.Defend, score = nodeDecisionComp.defendScore },
                    new() { decisionType = DecisionType.Capture, score = nodeDecisionComp.captureScore },
                    new() { decisionType = DecisionType.HelpFrontier, score = nodeDecisionComp.helpFrontierScore },
                    new() { decisionType = DecisionType.Idle, score = nodeDecisionComp.idleScore },
                };

                var best = scores[0];
                for (int i = 1; i < len; i++)
                {
                    if (scores[i].score > best.score) best = scores[i];
                }

                // var currentDecision = nodeDecisionComp.decisionType;
                entity.AddTag<NodeDecidedTag>();
                nodeDecisionComp.decisionType = best.decisionType;
            }
        }

    }
}