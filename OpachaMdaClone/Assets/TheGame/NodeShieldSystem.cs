using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct ShieldGenerationDelayComp : IComponent
    {
        public Timer timer;
    }
    
    public struct ShieldComp : IComponent
    {
        public float max;
        public float current;
    }
    
    public class NodeShieldSystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, ShieldComp> nodeWithoutShieldGeneratorFilter = new Filter<NodeComp, ShieldComp>().Exclude<ShieldGeneratorComp>();
        readonly Filter<NodeComp, ShieldComp, ShieldGeneratorComp> shieldGeneratorFilter = new Filter<NodeComp, ShieldComp, ShieldGeneratorComp>().Exclude<ShieldGenerationDelayComp>();
        readonly Filter<ShieldGenerationDelayComp> shieldGenerationDelayFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            shieldGenerationDelayFilter.ForEach((Entity entity, ref ShieldGenerationDelayComp shieldGenerationDelayComp) =>
            {
                if (shieldGenerationDelayComp.timer.Update(XTime.deltaTime) == false) return;
                entity.RemoveComponent<ShieldGenerationDelayComp>();
            });
            nodeWithoutShieldGeneratorFilter.ForEach(CheckShieldGenerationRequirements);
            shieldGeneratorFilter.ForEach(GenerateShieldPoints);
        }

        void CheckShieldGenerationRequirements(Entity entity, ref NodeComp nodeComp, ref ShieldComp shieldComp)
        {
            if (XIVMathf.Abs(shieldComp.current - shieldComp.max) > XIVMathf.Epsilon)
            {
                entity.AddComponent(new ShieldGeneratorComp
                {
                    shieldGenerationSpeed = assetReferences.generationConfigs[nodeComp.configIdx].shieldGenerationSpeed,
                });
            }
        }

        void GenerateShieldPoints(Entity entity, ref NodeComp nodeComp, ref ShieldComp shieldComp, ref ShieldGeneratorComp shieldGeneratorComp)
        {
            shieldGeneratorComp.shieldGenerationSpeed = assetReferences.generationConfigs[nodeComp.configIdx].shieldGenerationSpeed;
            
            if (XIVMathf.Abs(shieldComp.current - shieldComp.max) < XIVMathf.Epsilon)
            {
                shieldComp.current = shieldComp.max;
                entity.RemoveComponent<ShieldGeneratorComp>();
                return;
            }

            shieldComp.current = XIVMathf.Min(shieldComp.current + shieldGeneratorComp.shieldGenerationSpeed * XTime.deltaTime, shieldComp.max);
        }
    }
}