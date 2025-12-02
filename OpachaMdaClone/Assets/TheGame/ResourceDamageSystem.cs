using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ResourceDamageComp : IComponent
    {
        public Entity attackerUnitEntity;
        public float amount;
    }
    
    public class ResourceDamageSystem : XIV.Ecs.System
    {
        readonly Filter<ShieldComp, ResourceDamageComp> shieldDamageFilter = null;
        readonly Filter<ResourceComp, ResourceDamageComp> resourceDamageFilter = null;

        public override void Update()
        {
            shieldDamageFilter.ForEach(HandleShieldImpact);
            resourceDamageFilter.ForEach(ResourceTakeDamage);
        }

        void HandleShieldImpact(Entity entity, ref ShieldComp shieldComp, ref ResourceDamageComp resourceDamageComp)
        {
            var prev = shieldComp.current;
            var remainingShield = shieldComp.current - resourceDamageComp.amount;
            shieldComp.current = XIVMathf.Max(remainingShield, 0f);
            var diff = prev - shieldComp.current;
            resourceDamageComp.amount -= diff;
            
            if (resourceDamageComp.amount <= 0) entity.RemoveComponent<ResourceDamageComp>();
            if (diff > 0f)
            {
                entity.AddComponent(new ShieldGenerationDelayComp
                {
                    timer = new Timer(0.8f),
                });
            }
        }

        void ResourceTakeDamage(Entity entity, ref ResourceComp resourceComp, ref ResourceDamageComp resourceDamageComp)
        {
            entity.RemoveComponent<ResourceDamageComp>();

            float resourceImpact = resourceDamageComp.amount;
            var currentResourceQuantity = resourceComp.resourceQuantity;
            var targetQuantity = currentResourceQuantity - resourceImpact;
            if (XIVMathf.Abs(currentResourceQuantity - targetQuantity) < XIVMathf.Epsilon) return;
            
            if (targetQuantity <= 0f)
            {
                targetQuantity = -targetQuantity;
                entity.AddComponent(new NodeOccupyComp
                {
                    unitEntity = resourceDamageComp.attackerUnitEntity,
                });
            }
            resourceComp.resourceQuantity = targetQuantity;
            entity.AddTag<UpdateResourceQuantityTextTag>();
        }
    }
}