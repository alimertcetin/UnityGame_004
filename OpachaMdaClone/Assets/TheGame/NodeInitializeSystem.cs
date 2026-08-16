using System;
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

        public override void Update()
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
            Vec3 min = Vec3.zero;
            Vec3 max = Vec3.zero;
            for (int i = 0; i < nodeEntityCount; i++)
            {
                ref var entity = ref arr[i];
                var pos = entity.GetComponent<PositionComp>().position;
                mapCenter += pos.ToVector3();
                
                min.x = XIVMathf.Min(min.x, pos.x);
                min.y = XIVMathf.Min(min.y, pos.y);
                max.x = XIVMathf.Max(max.x, pos.x);
                max.y = XIVMathf.Max(max.y, pos.y);
            }
            mapCenter /= nodeEntityCount;
            var minMaxDiff = max - min;
            var directionLen = minMaxDiff.magnitude / 2f;
            var angleStep = 360f / unitEntityCount;
            int index = 0;
#if UNITY_EDITOR
            XIVDebug.DrawCircle(mapCenter.ToVec3(), 0.25f, XIVColor.green, 10f);
#endif
            var prevSeed = XIVRandom.seed;
            XIVRandom.InitState(levelSettings.levelGenerationSettings.seed);
            var startDir = (Vector3)XIVRandom.insideUnitCircle.ToVector2();
            unitFilter.ForEach((Entity e, ref UnitComp unitComp) =>
            {
                var directionVector = mapCenter + (startDir.RotateAroundZ(angleStep * index, Vector3.zero).normalized * directionLen);
                var nodeEntity = arr.XIVGetClosest(nodeEntityCount, directionVector.ToVec3(), out _, out _, (n) => n.GetComponent<PositionComp>().position, excludeArr, index);
#if UNITY_EDITOR
                XIVDebug.DrawLine(mapCenter.ToVec3(), directionVector.ToVec3(), XIVColor.red, 10f);
                XIVDebug.DrawCircle(nodeEntity.GetComponent<PositionComp>().position, 2f, XIVColor.red, 8f);
#endif
                unitComp.occupiedNodeEntities = new DynamicArray<Entity>();
                unitComp.smartness01 = unitComp.unitType == UnitIdLookup.UnitType.Green ? unitComp.smartness01 : (float)unitComp.unitType / (float)(UnitIdLookup.UnitType.NumberOfItems - 1);
                
                world.NewEntity().AddComponent(new NodeOccupyEventComp
                {
                    nodeEntity = nodeEntity,
                    unitEntity = e,
                });
                excludeArr[index++] = nodeEntity;
            });
            XIVRandom.InitState(prevSeed);
        }

        void InitializeNodes(Entity entity, ref TransformComp transformComp, ref NodeComp nodeComp)
        {
            transformComp.transform.gameObject.name = "Node: " + entity.ToString();
            nodeComp.configIdx = 0;
            ref var instancedRendererComp = ref entity.GetComponent<InstancedRendererComp>();
            instancedRendererComp.renderer.GetPropertyBlock(instancedRendererComp.materialPropertyBlock);
            instancedRendererComp.materialPropertyBlock.SetColor(ShaderConstants.Custom_SpriteWithShadow_Instanced.Color_ColorID, UnitIdLookup.GetColor(UnitIdLookup.UnitType.Black).ToUnityColor());
            instancedRendererComp.renderer.SetPropertyBlock(instancedRendererComp.materialPropertyBlock);
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