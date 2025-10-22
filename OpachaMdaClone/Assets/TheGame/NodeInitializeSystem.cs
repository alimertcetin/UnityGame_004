using System;
using System.Buffers;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class NodeInitializeSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp> nodeCompFilter = null;
        readonly Filter<UnitComp> unitFilter = null;
        readonly Filter<TransformComp, NodeComp, NodeOccupyComp> nodeOccupyFilter = null;
        
        public override void Start()
        {
            InitializeNodes();

            var bufferLen = XIVMathInt.NextPowerOfTwo(nodeCompFilter.NumberOfEntities);
            using IDisposable dispose = ArrayUtils.GetBuffer(out Entity[] buffer, bufferLen);
            var nodeEntityCount = nodeCompFilter.EntitiesNonAlloc(buffer);
            if (nodeEntityCount < 3) throw new InvalidOperationException();

            var unitCount = nodeCompFilter.EntitiesNonAlloc(buffer);
            using var temp = ArrayUtils.GetBuffer(out Entity[] tempEntityBuffer, unitCount);
            tempEntityBuffer[0] = buffer.XIVGetClosest(unitCount, new Vec3(1, 1, 0) * 200f, p => p.GetTransform().position);
            tempEntityBuffer[1] = buffer.XIVGetClosest(unitCount, new Vec3(-1, 1, 0) * 200f, p => p.GetTransform().position);
            tempEntityBuffer[2] = buffer.XIVGetClosest(unitCount, new Vec3(-1, -1, 0) * 200f, p => p.GetTransform().position);
            tempEntityBuffer[3] = buffer.XIVGetClosest(unitCount,  new Vec3(1, -1, 0) * 200f, p => p.GetTransform().position);
            
            int index = 0;
            unitFilter.ForEach((Entity e, ref UnitComp unitComp) =>
            {
                unitComp.occupiedNodeEntities = new DynamicArray<Entity>();
                var entity = tempEntityBuffer[index++];
                entity.AddComponent(new NodeOccupyComp
                {
                    unitEntity = e,
                });
            });
        }

        void InitializeNodes()
        {
            // Initialize all nodes with default values
            nodeCompFilter.ForEach(((ref TransformComp transformComp, ref NodeComp nodeComp) =>
            {
                nodeComp.resourceQuantity = 3;
                nodeComp.txt_quantity.text = ((int)nodeComp.resourceQuantity).ToString();
                nodeComp.shieldPoints = 7;
                nodeComp.totalShieldPoints = 7;
                nodeComp.unitType = UnitIdLookup.UnitType.Black;
                var renderer = transformComp.transform.GetComponent<Renderer>();
                renderer.material.color = UnitIdLookup.GetColor(nodeComp.unitType);
            }));
        }

        public override void Update()
        {
            nodeOccupyFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp, ref NodeOccupyComp nodeOccupyComp) =>
            {
                var unitEntity = nodeOccupyComp.unitEntity;
                ref var unitComp = ref unitEntity.GetComponent<UnitComp>();
                unitComp.occupiedNodeEntities.Add() = nodeEntity;
                nodeComp.unitType = unitComp.unitType;
                nodeEntity.AddComponent(new OccupiedNodeComp
                {
                    unitEntity = unitEntity,
                    resourceGenerationSpeed = unitComp.configs[0].generationSpeed, // the default config on unitComp
                });
                var renderer = transformComp.transform.GetComponent<Renderer>();
                var ca = renderer.material.color;
                var cb = UnitIdLookup.GetColor(nodeComp.unitType);
                renderer.XIVTween()
                    .ScaleBounceOnce()
                    .And()
                    .RendererColor(ca, cb, 0.25f, EasingFunction.EaseInOutBack)
                    .Start();
                nodeEntity.RemoveComponent<NodeOccupyComp>();
            });
        }
    }
}