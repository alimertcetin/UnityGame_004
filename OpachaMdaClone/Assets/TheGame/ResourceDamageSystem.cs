using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ResourceDamageEventComp : IComponent
    {
        public Entity damagedEntity;
        public Entity attackerUnitEntity;
        public float amount;
    }
    
    public class ResourceDamageSystem : XIV.Ecs.System
    {
        readonly Filter<ResourceDamageEventComp> resourceDamageFilter = null;

        public override void Update()
        {
            resourceDamageFilter.ForEach(HandleShieldImpact);
            resourceDamageFilter.ForEach(HandleResourceImpact);
        }

        void HandleShieldImpact(Entity entity, ref ResourceDamageEventComp resourceDamageEventComp)
        {
            if (resourceDamageEventComp.damagedEntity.HasComponent<ShieldComp>() == false) return;
            
            ref var shieldComp = ref resourceDamageEventComp.damagedEntity.GetComponent<ShieldComp>();
            var prev = shieldComp.current;
            var remainingShield = shieldComp.current - resourceDamageEventComp.amount;
            shieldComp.current = XIVMathf.Max(remainingShield, 0f);
            var diff = prev - shieldComp.current;
            resourceDamageEventComp.amount -= diff;
            
            if (resourceDamageEventComp.amount <= 0) entity.Destroy();
            if (diff > 0f)
            {
                resourceDamageEventComp.damagedEntity.AddComponent(new ShieldGenerationDelayComp
                {
                    timer = new Timer(0.8f),
                });
            }
        }

        void HandleResourceImpact(Entity e, ref ResourceDamageEventComp resourceDamageEventComp)
        {
            e.Destroy();
            if (resourceDamageEventComp.damagedEntity.HasComponent<ResourceComp>() == false) return;
            
            ref var resourceComp = ref resourceDamageEventComp.damagedEntity.GetComponent<ResourceComp>();
            float resourceImpact = resourceDamageEventComp.amount;
            var currentResourceQuantity = resourceComp.resourceQuantity;
            var targetQuantity = currentResourceQuantity - resourceImpact;
            if (XIVMathf.Abs(currentResourceQuantity - targetQuantity) < XIVMathf.Epsilon) return;
            
            if (targetQuantity <= 0f)
            {
                targetQuantity = -targetQuantity;
                world.NewEntity().AddComponent(new NodeOccupyEventComp
                {
                    nodeEntity = resourceDamageEventComp.damagedEntity,
                    unitEntity = resourceDamageEventComp.attackerUnitEntity,
                });
            }
            resourceComp.resourceQuantity = targetQuantity;
        }
    }
}