using XIV.Core.DataStructures;
using XIV.Ecs;

namespace TheGame
{
    public struct NodeCaptureEventComp : IComponent
    {
        public Entity nodeEntity;
        public Entity targetNodeEntity;
    }
    
    public class NodeCaptureSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeCaptureEventComp> nodeCaptureFilter = null;

        public override void Update()
        {
            nodeCaptureFilter.ForEach(CaptureTarget);
        }

        void CaptureTarget(Entity entity, ref NodeCaptureEventComp nodeCaptureEventComp)
        {
            entity.Destroy();
            var nodeEntity = nodeCaptureEventComp.nodeEntity;
            var targetEntity = nodeCaptureEventComp.targetNodeEntity;
            if (nodeEntity.HasComponent<OccupiedNodeComp>() == false) return;
            
            ref var occupiedNodeComp = ref nodeEntity.GetComponent<OccupiedNodeComp>();
            if (connectionDB.IsTargetAlly(occupiedNodeComp.unitEntity, targetEntity)) return;

            ref var resourceComp = ref nodeEntity.GetComponent<ResourceComp>();
            ref var otherNodeComp = ref targetEntity.GetComponent<NodeComp>();
            ref var otherResourceComp = ref targetEntity.GetComponent<ResourceComp>();

            GenerationStepSO config = assetReferences.generationConfigs[otherNodeComp.configIdx];
            float resourceTravelTime = GetResourceTravelTime(nodeEntity, targetEntity);
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

            world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromEntity = nodeEntity,
                toEntity = targetEntity,
                resourceQuantity = (int)resourceComp.resourceQuantity,
            });

            float GetResourceTravelTime(Entity nodeA, Entity nodeB)
            {
                var p0 = nodeA.GetComponent<PositionComp>().position;
                var p1 = nodeB.GetComponent<PositionComp>().position;
                var distance = Vec3.Distance(p0, p1);
                return distance / GameConstants.RESOURCE_MOVEMENT_SPEED;
            }
        }
    }
}