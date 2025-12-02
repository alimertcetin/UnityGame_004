using System;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct AddResourceComp : IComponent
    {
        public float amount;
    }

    public class ResourceIncreaseQuantitySystem : XIV.Ecs.System
    {
        readonly Filter<ResourceComp, AddResourceComp> addResourceQuantityFilter = null;

        public override void Update()
        {
            addResourceQuantityFilter.ForEach(AddResource);
        }

        void AddResource(Entity entity, ref ResourceComp resourceComp, ref AddResourceComp addResourceComp)
        {
            entity.RemoveComponent<AddResourceComp>();
            
            var prev = resourceComp.resourceQuantity;
            resourceComp.resourceQuantity = XIVMathf.Min(prev + addResourceComp.amount, GameConstants.MAX_RESOURCE_QUANTITY);
            if (XIVMathf.Abs(prev - resourceComp.resourceQuantity) > XIVMathf.Epsilon)
            {
                entity.AddTag<UpdateResourceQuantityTextTag>();
            }
        }
    }
}