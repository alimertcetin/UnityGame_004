using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ResourceDamageEventComp : IComponent
    {
        public Entity damagedEntity;
        public Entity attackerUnitEntity;
        public int amount;
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
            var prev = (int)shieldComp.current;
            var remainingShield = prev - resourceDamageEventComp.amount;
            shieldComp.current = XIVMathf.Max(remainingShield, 0f);
            var diff = prev - (int)shieldComp.current;
            resourceDamageEventComp.amount -= diff;
            
            if (resourceDamageEventComp.amount <= 0) entity.Destroy();
            if (diff > 0f)
            {
                world.NewEntity().AddComponent(new ShieldGenerationDelayEventComp
                {
                    shieldEntity = resourceDamageEventComp.damagedEntity,
                    timer = new Timer(0.8f),
                });
            }
        }

        void HandleResourceImpact(Entity e, ref ResourceDamageEventComp resourceDamageEventComp)
        {
            e.Destroy();
            if (resourceDamageEventComp.amount == 0) return;
            if (resourceDamageEventComp.damagedEntity.HasComponent<ResourceComp>() == false) return;
            
            ref var resourceComp = ref resourceDamageEventComp.damagedEntity.GetComponent<ResourceComp>();
            var currentResourceQuantity = (int)resourceComp.resourceQuantity;
            var targetQuantity = currentResourceQuantity - resourceDamageEventComp.amount;
            
            if (targetQuantity <= 0f)
            {
                targetQuantity = -targetQuantity;
                world.NewEntity().AddComponent(new NodeOccupyEventComp
                {
                    nodeEntity = resourceDamageEventComp.damagedEntity,
                    unitEntity = resourceDamageEventComp.attackerUnitEntity,
                });
            }

            // var remainder = resourceComp.resourceQuantity - (int)resourceComp.resourceQuantity;
            resourceComp.resourceQuantity = targetQuantity;
        }
    }
}