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
using XIV.UnityEngineIntegration;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class NodeInitializeSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp> nodeCompFilter = null;
        readonly Filter<UnitComp> unitFilter = null;
        readonly AssetReferences assetReferences = null;
        readonly LevelSettings levelSettings = null;

        public override void Start()
        {
            void CreateUnits(int nodeEntityCount)
            {
                Array values = Enum.GetValues(typeof(UnitIdLookup.UnitType));
                for (int i = 0; i < levelSettings.hostileUnits && i < nodeEntityCount; i++)
                {
                    var v = (UnitIdLookup.UnitType)values.GetValue(i) + (int)UnitIdLookup.UnitType.Black + 1;
                    var newEntity = world.NewEntity();
                    newEntity.AddComponent(new UnitComp { unitType = v, });
                    newEntity.AddComponent(new DebugNameComp { name = v.ToString() });
                }
            }
            
            // Initialize all nodes with default values
            nodeCompFilter.ForEach(InitializeNodes);

            using var nodeEntityBuffer = ArrayUtils.GetBuffer<Entity>(nodeCompFilter.NumberOfEntities);
            var arr = (Entity[])nodeEntityBuffer;
            var nodeEntityCount = nodeCompFilter.EntitiesNonAlloc(arr);
            CreateUnits(nodeEntityCount);

            var unitEntityCount = unitFilter.NumberOfEntities;
            Entity[] excludeArr = new Entity[unitEntityCount];
            Vector3 mapCenter = Vector3.zero;
            for (int i = 0; i < nodeEntityCount; i++)
            {
                ref var entity = ref arr[i];
                var pos = entity.GetComponent<PositionComp>().position;
                mapCenter += pos.ToVector3();
            }
            mapCenter /= nodeEntityCount;
            var angle = 180f / unitEntityCount;

            int index = 0;
            var directionVector = (Vector3)XIVRandom.insideUnitCircle.ToVector2() * (mapCenter.sqrMagnitude * 0.5f);
            unitFilter.ForEach((Entity e, ref UnitComp unitComp) =>
            {
                XIVDebug.DrawLine(mapCenter, directionVector, XIVColor.red, 10f);
                var nodeEntity = arr.XIVGetClosest(nodeEntityCount, directionVector, out _, out _, (n) => n.GetComponent<PositionComp>().position, excludeArr, index);
                directionVector = directionVector.RotateAroundZ(angle, mapCenter);
                XIVDebug.DrawCircle(nodeEntity.GetComponent<PositionComp>().position, 2f, XIVColor.red, 8f);
                unitComp.occupiedNodeEntities = new DynamicArray<Entity>();
                unitComp.smartness01 = unitComp.unitType == UnitIdLookup.UnitType.Green ? unitComp.smartness01 : (float)unitComp.unitType / (float)(UnitIdLookup.UnitType.NumberOfItems - 1);
                
                world.NewEntity().AddComponent(new NodeOccupyEventComp
                {
                    nodeEntity = nodeEntity,
                    unitEntity = e,
                });
                excludeArr[index++] = nodeEntity;
            });
        }

        void InitializeNodes(Entity entity, ref TransformComp transformComp, ref NodeComp nodeComp)
        {
            transformComp.transform.gameObject.name = "Node: " + entity.ToString();
            nodeComp.configIdx = 0;
            var renderer = transformComp.transform.GetComponent<SpriteRenderer>();
            renderer.color = UnitIdLookup.GetColor(UnitIdLookup.UnitType.Black);
            entity.AddComponent(new ResourceComp
            {
                resourceQuantity = 3f,
            });
            entity.AddComponent(new PathFinderComp
            {
                path = new DynamicArray<Entity>(8),
            });
            world.NewEntity().AddComponent(new AddShieldEventComp
            {
                targetEntity = entity,
                max = 7f,
                current = 3f,
            });
        }
    }
}