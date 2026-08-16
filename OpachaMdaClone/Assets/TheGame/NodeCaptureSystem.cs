using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public class NodeCaptureSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, ResourceComp, OccupiedNodeComp, NodeDecisionComp> nodeDecisionFilter = null;

        public override void Update()
        {
            nodeDecisionFilter.ForEach(CaptureTarget);
        }

        void CaptureTarget(Entity entity, ref NodeComp nodeComp, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp)
        {
            if (nodeDecisionComp.decisionType != DecisionType.Capture) return;
            
            float GetResourceTravelTime(Entity nodeA, Entity nodeB)
            {
                var p0 = nodeA.GetComponent<PositionComp>().position;
                var p1 = nodeB.GetComponent<PositionComp>().position;
                var distance = Vec3.Distance(p0, p1);
                return distance / GameConstants.RESOURCE_MOVEMENT_SPEED;
            }
            
            var entityToCapture = GetEntityToCapture(entity, ref occupiedNodeComp);
            if (entityToCapture.IsAlive() == false) return;
            
            if (connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, entityToCapture)) return;

            ref var otherNodeComp = ref entityToCapture.GetComponent<NodeComp>();
            ref var otherResourceComp = ref entityToCapture.GetComponent<ResourceComp>();

            GenerationStepSO config = assetReferences.generationConfigs[otherNodeComp.configIdx];
            float totalTravelTime = GetResourceTravelTime(entity, entityToCapture);
            float generatedQuantityAtArrival = config.resourceGenerationSpeed * totalTravelTime;
            
            var otherNodeShieldPoints = 0f;
            if (entityToCapture.HasComponent<ShieldComp>()) otherNodeShieldPoints = entityToCapture.GetComponent<ShieldComp>().current;

            var requiredResource = (int)(otherResourceComp.resourceQuantity + otherNodeShieldPoints + (generatedQuantityAtArrival * occupiedNodeComp.unitEntity.GetComponent<UnitComp>().smartness01));
            var quantityToSend = (int)resourceComp.resourceQuantity;
            // we don't have enough resource to takeover
            if (quantityToSend - requiredResource <= 0) return;
            
            // int connectionIndex = connectionDB.GetConnectionIndex(entity, nodeCaptureComp.targetEntity);
            var allyResourceTransfer = connectionDB.GetAllyResourceTransfer(entityToCapture, occupiedNodeComp.unitEntity);
            var hostileResourceTransfer = connectionDB.GetHostileResourceTransfer(entityToCapture, occupiedNodeComp.unitEntity);
            var totalAlly = allyResourceTransfer - hostileResourceTransfer;
            // Unit already send required amount of resource from neighbors of targetEntity
            if (totalAlly >= requiredResource) return;
            
            resourceComp.resourceQuantity -= quantityToSend; // keep the fraction?
            world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromUnitEpoch = nodeComp.unitEpoch,
                fromEntity = entity,
                toEntity = entityToCapture,
                resourceQuantity = quantityToSend,
            });
        }

        Entity GetEntityToCapture(Entity entity, ref OccupiedNodeComp occupiedNodeComp)
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