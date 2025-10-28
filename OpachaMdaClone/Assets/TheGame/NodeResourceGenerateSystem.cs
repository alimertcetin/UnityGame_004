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
        public float resourceGenerationSpeed;
    }

    public class NodeResourceGenerateSystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, OccupiedNodeComp> occupiedNodeCompFilter = null;

        public override void Update()
        {
            occupiedNodeCompFilter.ForEach((Entity e, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp) =>
            {
                nodeComp.resourceQuantity += (XTime.deltaTime * occupiedNodeComp.resourceGenerationSpeed);
                nodeComp.txt_quantity.WriteScoreText((int)nodeComp.resourceQuantity);
                // TODO : NodeResourceGenerateSystem -> add occupiedNodeComp.shieldGenerationSpeed
                var distance = XIVMathf.Abs(nodeComp.totalShieldPoints - nodeComp.shieldPoints);
                nodeComp.shieldPoints += distance * XTime.deltaTime + 0.01f;
                nodeComp.shieldPoints = XIVMathf.Clamp(nodeComp.shieldPoints, 0, nodeComp.totalShieldPoints);
            });
        }
    }
}
