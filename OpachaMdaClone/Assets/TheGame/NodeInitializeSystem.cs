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
        readonly AssetReferences assetReferences = null;
        readonly LevelSettings levelSettings;

        public override void Start()
        {
            void CreateUnit(UnitIdLookup.UnitType unitType) => world.NewEntity().AddComponent(new UnitComp { unitType = unitType, });
            
            // Initialize all nodes with default values
            nodeCompFilter.ForEach(InitializeNodes);

            using var nodeEntityBuffer = ArrayUtils.GetBuffer<Entity>(nodeCompFilter.NumberOfEntities);
            var nodeEntityCount = nodeCompFilter.EntitiesNonAlloc(nodeEntityBuffer);
            if (nodeEntityCount < 3) throw new InvalidOperationException();

            Array values = Enum.GetValues(typeof(UnitIdLookup.UnitType));
            for (int i = 0; i < levelSettings.hostileUnits; i++)
            {
                var v = (UnitIdLookup.UnitType)values.GetValue(i) + (int)UnitIdLookup.UnitType.Black + 1;
                CreateUnit(v);
            }

            var unitEntityCount = unitFilter.NumberOfEntities;
            using var startingUnitNodeEntityBuffer = ArrayUtils.GetBuffer<Entity>(nodeEntityCount);
            int order = 6; // max 6
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(1, 1, 0) * 200f, p => p.GetComponent<PositionComp>().position);
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(-1, 1, 0) * 200f, p => p.GetComponent<PositionComp>().position);
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(-1, -1, 0) * 200f, p => p.GetComponent<PositionComp>().position);
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(1, -1, 0) * 200f, p => p.GetComponent<PositionComp>().position);
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(1, 0, 0) * 200f, p => p.GetComponent<PositionComp>().position);
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(0, 1, 0) * 200f, p => p.GetComponent<PositionComp>().position);
            startingUnitNodeEntityBuffer[order--] = ((Entity[])nodeEntityBuffer).XIVGetClosest(nodeEntityCount, new Vec3(0, 0, 0) * 200f, p => p.GetComponent<PositionComp>().position);

            int index = 0;
            unitFilter.ForEach((Entity e, ref UnitComp unitComp) =>
            {
                var entity = startingUnitNodeEntityBuffer[unitEntityCount - 1 - index++];
                unitComp.occupiedNodeEntities = new DynamicArray<Entity>();
                unitComp.smartness01 = unitComp.unitType == UnitIdLookup.UnitType.Green ? unitComp.smartness01 : (float)unitComp.unitType / (float)(UnitIdLookup.UnitType.NumberOfItems - 1);
                entity.AddComponent(new NodeOccupyComp
                {
                    unitEntity = e,
                });
            });
        }

        void InitializeNodes(Entity entity, ref TransformComp transformComp, ref NodeComp nodeComp)
        {
            nodeComp.resourceQuantity = 3;
            nodeComp.configIdx = 0;
            nodeComp.txt_quantity.WriteScoreText((int)nodeComp.resourceQuantity);
            nodeComp.shieldPoints = assetReferences.generationConfigs[nodeComp.configIdx].shieldPoints;
            // nodeComp.totalShieldPoints = 7;
            // nodeComp.resourceGenerationSpeed = 0;
            var renderer = transformComp.transform.GetComponent<SpriteRenderer>();
            renderer.color = UnitIdLookup.GetColor(UnitIdLookup.UnitType.Black);
            entity.AddComponent(new NodeDecisionComp());
        }
    }
}