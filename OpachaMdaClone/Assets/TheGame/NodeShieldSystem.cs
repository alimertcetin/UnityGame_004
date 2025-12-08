using System;
using System.Collections.Generic;
using UnityEngine;
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
                shieldGenerationDelayEventComp.shieldEntity.GetComponent<ShieldComp>().isGeneratingShield = false;
                if (shieldGenerationDelayEventComp.timer.Update(XTime.deltaTime) == false) return;
                entity.Destroy();
                shieldGenerationDelayEventComp.shieldEntity.GetComponent<ShieldComp>().isGeneratingShield = true;
            });
            shieldGeneratorFilter.ForEach(GenerateShieldPoints);
        }

        void GenerateShieldPoints(Entity entity, ref NodeComp nodeComp, ref ShieldComp shieldComp)
        {
            if (XIVMathf.Abs(shieldComp.current - shieldComp.max) > XIVMathf.Epsilon) shieldComp.isGeneratingShield = true;
            if (shieldComp.isGeneratingShield == false) return;
            
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