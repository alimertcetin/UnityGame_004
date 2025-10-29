using System;
using System.Buffers;
using TMPro;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class NodeInitializeSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp> nodeCompFilter = null;
        readonly Filter<UnitComp> unitFilter = null;

        public override void Start()
        {
            void CreateUnit(UnitIdLookup.UnitType unitType) => world.NewEntity().AddComponent(new UnitComp { unitType = unitType, });
            
            InitializeNodes();

            using var dispose = ArrayUtils.GetBuffer(out Entity[] nodeEntityBuffer, nodeCompFilter.NumberOfEntities);
            var nodeEntityCount = nodeCompFilter.EntitiesNonAlloc(nodeEntityBuffer);
            if (nodeEntityCount < 3) throw new InvalidOperationException();

            using var temp = ArrayUtils.GetBuffer(out Entity[] startingUnitNodeEntityBuffer, nodeEntityCount);
            startingUnitNodeEntityBuffer[0] = nodeEntityBuffer.XIVGetClosest(nodeEntityCount, new Vec3(1, 1, 0) * 200f, p => p.GetComponent<TransformComp>().transform.position);
            startingUnitNodeEntityBuffer[1] = nodeEntityBuffer.XIVGetClosest(nodeEntityCount, new Vec3(-1, 1, 0) * 200f, p => p.GetComponent<TransformComp>().transform.position);
            startingUnitNodeEntityBuffer[2] = nodeEntityBuffer.XIVGetClosest(nodeEntityCount, new Vec3(-1, -1, 0) * 200f, p => p.GetComponent<TransformComp>().transform.position);
            startingUnitNodeEntityBuffer[3] = nodeEntityBuffer.XIVGetClosest(nodeEntityCount, new Vec3(1, -1, 0) * 200f, p => p.GetComponent<TransformComp>().transform.position);

            CreateUnit(UnitIdLookup.UnitType.Blue);
            CreateUnit(UnitIdLookup.UnitType.Red);

            int index = 0;
            unitFilter.ForEach((Entity e, ref UnitComp unitComp) =>
            {
                var entity = startingUnitNodeEntityBuffer[index++];
                unitComp.occupiedNodeEntities = new DynamicArray<Entity>();
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
                nodeComp.txt_quantity.WriteScoreText((int)nodeComp.resourceQuantity);
                nodeComp.shieldPoints = 7;
                nodeComp.totalShieldPoints = 7;
                nodeComp.unitType = UnitIdLookup.UnitType.Black;
                var renderer = transformComp.transform.GetComponent<SpriteRenderer>();
                renderer.color = UnitIdLookup.GetColor(nodeComp.unitType);
            }));
        }
    }
}