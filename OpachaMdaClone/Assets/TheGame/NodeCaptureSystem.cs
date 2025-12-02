using XIV.Core.DataStructures;
using XIV.Ecs;

namespace TheGame
{
    public class NodeCaptureSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<ResourceComp, OccupiedNodeComp, NodeCaptureComp> nodeCaptureFilter;

        public override void Update()
        {
            nodeCaptureFilter.ForEach(CaptureTarget);
            nodeCaptureFilter.RemoveComponentAll<NodeCaptureComp>();
        }

        void CaptureTarget(Entity entity, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeCaptureComp nodeCaptureComp)
        {
            var targetEntity = nodeCaptureComp.targetEntity;
            if (targetEntity.IsAlive() == false || connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, targetEntity)) return;

            ref var otherNodeComp = ref targetEntity.GetComponent<NodeComp>();
            ref var otherResourceComp = ref targetEntity.GetComponent<ResourceComp>();

            GenerationStepSO config = assetReferences.generationConfigs[otherNodeComp.configIdx];
            float resourceTravelTime = GetResourceTravelTime();
            float generatedQuantityAtArrival = config.resourceGenerationSpeed / resourceTravelTime;
            
            var otherNodeShieldPoints = 0f;
            if (targetEntity.HasComponent<ShieldComp>()) otherNodeShieldPoints = targetEntity.GetComponent<ShieldComp>().current;

            var requiredResource = otherResourceComp.resourceQuantity + otherNodeShieldPoints + (generatedQuantityAtArrival * occupiedNodeComp.unitEntity.GetComponent<UnitComp>().smartness01);
            var diff = resourceComp.resourceQuantity - requiredResource;
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
                resourceQuantity = (int)resourceComp.resourceQuantity,
                toEntity = targetEntity,
            });
            // entity.AddTag<ReevaluateDecisionTag>();

            float GetResourceTravelTime()
            {
                var p0 = entity.GetComponent<PositionComp>().position;
                var p1 = targetEntity.GetComponent<PositionComp>().position;
                var distance = Vec3.Distance(p0, p1);
                return distance / GameConstants.RESOURCE_MOVEMENT_SPEED;
            }
        }
    }
}