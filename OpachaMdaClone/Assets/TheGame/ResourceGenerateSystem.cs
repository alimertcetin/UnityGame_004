using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ResourceGenerationDelayComp : IComponent
    {
        public Entity resourceEntity;
        public Timer timer;
    }

    public class ResourceGenerateSystem : XIV.Ecs.System
    {
        readonly Filter<ResourceGenerationDelayComp> generationDelayFilter = null;
        readonly Filter<NodeComp, ResourceComp, InstancedRendererComp> resourceGeneratorFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            generationDelayFilter.ForEach((Entity e, ref ResourceGenerationDelayComp resourceGenerationDelayComp) =>
            {
                resourceGenerationDelayComp.resourceEntity.GetComponent<ResourceComp>().isGeneratingResource = false;
                if (resourceGenerationDelayComp.timer.Update(XTime.deltaTime) == false) return;
                e.Destroy();
                resourceGenerationDelayComp.resourceEntity.GetComponent<ResourceComp>().isGeneratingResource = true;
            });
            resourceGeneratorFilter.ForEach(GenerateResource);
        }

        void GenerateResource(Entity entity, ref NodeComp nodeComp, ref ResourceComp resourceComp, ref InstancedRendererComp instancedRendererComp)
        {
            if (resourceComp.isGeneratingResource == false) return;
            
            var speed = assetReferences.generationConfigs[nodeComp.configIdx].resourceGenerationSpeed;
            resourceComp.resourceQuantity = XIVMathf.Min(resourceComp.resourceQuantity + XTime.deltaTime * speed, GameConstants.MAX_RESOURCE_QUANTITY);
            float t = resourceComp.resourceQuantity % 1f; // a little offset for shader
            instancedRendererComp.renderer.GetPropertyBlock(instancedRendererComp.materialPropertyBlock);
            instancedRendererComp.materialPropertyBlock.SetFloat("_AnimTime", t);
            instancedRendererComp.renderer.SetPropertyBlock(instancedRendererComp.materialPropertyBlock);
            if (XIVMathf.Abs(resourceComp.resourceQuantity - GameConstants.MAX_RESOURCE_QUANTITY) < XIVMathf.Epsilon)
            {
                resourceComp.isGeneratingResource = false;
            }
        }
    }
}
