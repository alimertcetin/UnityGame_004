using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct OccupiedNodeComp : IComponent
    {
        public Entity unitEntity;
    }

    public class NodeResourceGenerateSystem : XIV.Ecs.System
    {
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, OccupiedNodeComp> occupiedNodeCompFilter = null;
        const float SHIELD_GENERATION_SPEED = 0.5f;

        public override void Update()
        {
            occupiedNodeCompFilter.ForEach((Entity e, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp) =>
            {
                nodeComp.resourceQuantity += (XTime.deltaTime * assetReferences.generationConfigs[nodeComp.configIdx].generationSpeed);
                nodeComp.txt_quantity.WriteScoreText((int)nodeComp.resourceQuantity);
                nodeComp.shieldPoints = XIVMathf.Clamp(nodeComp.shieldPoints + SHIELD_GENERATION_SPEED * XTime.deltaTime, 0, assetReferences.generationConfigs[nodeComp.configIdx].shieldPoints);
            });
        }
    }
}
