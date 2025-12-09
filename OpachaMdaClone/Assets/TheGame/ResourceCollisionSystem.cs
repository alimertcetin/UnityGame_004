using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct ResourceTransferCollisionEventComp : IComponent
    {
        public Entity resourceEntity1;
        public Entity resourceEntity2;
    }
    
    public struct ReturnToPoolTag : ITag { }
    
    public class ResourceCollisionSystem : XIV.Ecs.System
    {
        readonly Filter<ResourceTransferCollisionEventComp> resourceCollisionEventFilter = null;
        readonly Filter<NodeResourceCollisionEventComp> nodeResourceCollisionFilter = null;
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            nodeResourceCollisionFilter.ForEach(EvaluateResourceToNodeCollision);
            resourceCollisionEventFilter.ForEach(EvaluateResourceToResourceCollision);
        }

        void EvaluateResourceToNodeCollision(Entity entity, ref NodeResourceCollisionEventComp nodeResourceCollisionEventComp)
        {
            entity.Destroy();
            var nodeEntity = nodeResourceCollisionEventComp.receiver;
            ref var resourceComp = ref nodeEntity.GetComponent<ResourceComp>();
            var attackerUnitEntity = nodeResourceCollisionEventComp.senderUnitEntity;
            if (connectionDB.IsTargetAlly(attackerUnitEntity, nodeEntity))
            {
                resourceComp.resourceQuantity += nodeResourceCollisionEventComp.quantity;
            }
            else
            {
                world.NewEntity().AddComponent(new ResourceDamageEventComp
                {
                    damagedEntity = nodeEntity,
                    attackerUnitEntity = nodeResourceCollisionEventComp.senderUnitEntity,
                    amount = nodeResourceCollisionEventComp.quantity,
                });
            }

            if (nodeEntity.HasTween() == false)
            {
                var scale = nodeEntity.GetComponent<ScaleComp>().scale.ToVector3();
                nodeEntity.XIVTween()
                    .Scale(scale, scale * 1.1f, 0.5f, EasingFunction.EaseOutCubic, true)
                    .UseCustomDeltaTime(() => XTime.deltaTime)
                    .Start();
            }
        }

        // Resource x Resource collision
        void EvaluateResourceToResourceCollision(Entity entity, ref ResourceTransferCollisionEventComp resourceTransferCollisionEventComp)
        {
            entity.Destroy();
            ref var ent1TransferableResourceComp = ref resourceTransferCollisionEventComp.resourceEntity1.GetComponent<TransferableResourceComp>();
            ref var ent2TransferableResourceComp = ref resourceTransferCollisionEventComp.resourceEntity2.GetComponent<TransferableResourceComp>();
            
            // TODO: Play particle
            if (ent1TransferableResourceComp.quantity == ent2TransferableResourceComp.quantity)
            {
                // destroy both
                resourceTransferCollisionEventComp.resourceEntity1.AddTag<ReturnToPoolTag>();
                resourceTransferCollisionEventComp.resourceEntity2.AddTag<ReturnToPoolTag>();
            }
            else if (ent1TransferableResourceComp.quantity > ent2TransferableResourceComp.quantity)
            {
                ent1TransferableResourceComp.quantity -= ent2TransferableResourceComp.quantity;
                resourceTransferCollisionEventComp.resourceEntity1.GetComponent<TextComp>().txt.WriteScoreText(ent1TransferableResourceComp.quantity);
                resourceTransferCollisionEventComp.resourceEntity2.AddTag<ReturnToPoolTag>();
            }
            else if (ent1TransferableResourceComp.quantity < ent2TransferableResourceComp.quantity)
            {
                ent2TransferableResourceComp.quantity -= ent1TransferableResourceComp.quantity;
                resourceTransferCollisionEventComp.resourceEntity2.GetComponent<TextComp>().txt.WriteScoreText(ent2TransferableResourceComp.quantity);
                resourceTransferCollisionEventComp.resourceEntity1.AddTag<ReturnToPoolTag>();
            }
        }
    }
}