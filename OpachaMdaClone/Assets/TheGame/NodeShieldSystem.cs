using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ShieldGenerationDelayEventComp : IComponent
    {
        public Entity shieldEntity;
        public Timer timer;
    }
    
    public struct ShieldComp : IComponent
    {
        public Entity shieldRendererEntity;
        public bool isGeneratingShield;
        public float max;
        public float current;
    }
    
    public class NodeShieldSystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, ShieldComp> shieldGeneratorFilter = null;
        readonly Filter<ShieldGenerationDelayEventComp> shieldGenerationDelayFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            shieldGenerationDelayFilter.ForEach((Entity entity, ref ShieldGenerationDelayEventComp shieldGenerationDelayEventComp) =>
            {
                if (shieldGenerationDelayEventComp.shieldEntity.HasComponent<ShieldComp>() == false)
                {
                    entity.Destroy();
                    return;
                }
                ref var shieldComp = ref shieldGenerationDelayEventComp.shieldEntity.GetComponent<ShieldComp>();
                shieldComp.isGeneratingShield = false;
                if (shieldGenerationDelayEventComp.timer.Update(XTime.deltaTime) == false) return;
                entity.Destroy();
                shieldComp.isGeneratingShield = true;
            });
            shieldGeneratorFilter.ForEach(GenerateShieldPoints);
        }

        void GenerateShieldPoints(Entity entity, ref NodeComp nodeComp, ref ShieldComp shieldComp)
        {
            if (shieldComp.isGeneratingShield == false) return;
            if (XIVMathf.Abs(shieldComp.current - shieldComp.max) > XIVMathf.Epsilon) shieldComp.isGeneratingShield = true;
            
            var speed = assetReferences.generationConfigs[nodeComp.configIdx].shieldGenerationSpeed;
            if (XIVMathf.Abs(shieldComp.current - shieldComp.max) < XIVMathf.Epsilon)
            {
                shieldComp.current = shieldComp.max;
                shieldComp.isGeneratingShield = false;
                return;
            }

            shieldComp.current = XIVMathf.Min(shieldComp.current + speed * XTime.deltaTime, shieldComp.max);
        }
    }
}