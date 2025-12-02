using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ResourceGenerationDelayComp : IComponent
    {
        public Timer timer;
    }
    
    public struct UpdateResourceQuantityTextTag : ITag { }

    public class ResourceGenerateSystem : XIV.Ecs.System
    {
        readonly Filter<ResourceGenerationDelayComp> generationDelayFilter = null;
        readonly Filter<NodeComp, ResourceComp, ResourceGeneratorComp> resourceGeneratorFilter = new Filter<NodeComp, ResourceComp, ResourceGeneratorComp>().Exclude<ResourceGenerationDelayComp>();
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            generationDelayFilter.ForEach((Entity e, ref ResourceGenerationDelayComp resourceGeneratorFilter) =>
            {
                if (resourceGeneratorFilter.timer.Update(XTime.deltaTime) == false) return;
                e.RemoveComponent<ResourceGenerationDelayComp>();
            });
            resourceGeneratorFilter.ForEach(GenerateResource);
        }

        void GenerateResource(Entity entity, ref NodeComp nodeComp, ref ResourceComp resourceComp, ref ResourceGeneratorComp resourceGeneratorComp)
        {
            resourceGeneratorComp.resourceGenerationSpeed = assetReferences.generationConfigs[nodeComp.configIdx].resourceGenerationSpeed;
            var prev = resourceComp.resourceQuantity;
            resourceComp.resourceQuantity = XIVMathf.Min(prev + XTime.deltaTime * resourceGeneratorComp.resourceGenerationSpeed, GameConstants.MAX_RESOURCE_QUANTITY);
            if (XIVMathf.Abs(resourceComp.resourceQuantity - GameConstants.MAX_RESOURCE_QUANTITY) > XIVMathf.Epsilon)
            {
                entity.AddTag<UpdateResourceQuantityTextTag>();
            }
            else
            {
                entity.RemoveComponent<ResourceGeneratorComp>();
            }
        }
    }
}
