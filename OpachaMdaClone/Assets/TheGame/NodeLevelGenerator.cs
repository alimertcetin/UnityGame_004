using System;
using System.IO;
using TheGame.Extensions;
using UnityEditor;
using UnityEngine;
using XIV.Core.Algorithm;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIV.PoolSystem;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct LineRendererComp : IComponent
    {
        public LineRenderer lineRenderer;
        public Vector3[] positions;
    }
    
    public class NodeLevelGenerator : XIV.Ecs.System
    {
        const float NODE_RADIUS = 0.5f;
        const float MULTIPLIER = 4;
        const float TARGET_DISTANCE_BETWEEN_NODES = MULTIPLIER * NODE_RADIUS;
        const float LINK_DISTANCE = (MULTIPLIER * 2) * NODE_RADIUS;
        readonly LevelSettings levelSettings = null;
        readonly PrefabReferences prefabReferences = null;
        readonly ConnectionDB connectionDB = null;

        public override void Start()
        {
            // TODO : NodeLevelGenerator -> add seed variable
            int seedInt = 790;
            // int seedInt = DateTime.Now.Millisecond;
            XIVRandom.InitState(seedInt);
            var seedStr = DateTime.Now.ToShortTimeString() + " - Seed:" + seedInt.ToString() + Environment.NewLine;
            var path = Path.Combine("Assets", "GenerationSeeds");
            Directory.CreateDirectory(path);
            File.AppendAllText(Path.Combine(path, "Seed.txt"), seedStr);
            AssetDatabase.Refresh();
            
            // TODO : NodeLevelGenerator -> add map size variable
            int bufferLen = GetNodeQuantity(4);
            using var entityBufferDispose = ArrayUtils.GetBuffer(out Entity[] entityBuffer, bufferLen);
            using var positionBufferDispose = ArrayUtils.GetBuffer(out Vector2[] positionBuffer, bufferLen);
            var center = Vector2.zero;
            FillPositions(positionBuffer, bufferLen);
            MovePositionsToCenter(positionBuffer, bufferLen, center);
            CreateNodes(entityBuffer, bufferLen, positionBuffer, bufferLen);
            LinkNodes(entityBuffer, bufferLen, positionBuffer, bufferLen);
        }

        void FillPositions(Vector2[] positionBuffer, int bufferLen)
        {
            PoissonDiscSampler poissonDiscSampler = new PoissonDiscSampler();
            var regionSize = new Vec2(levelSettings.gridSizeX, levelSettings.gridSizeY);
            var points = poissonDiscSampler.GeneratePoints(TARGET_DISTANCE_BETWEEN_NODES, 100, regionSize);
            while (points.Length < bufferLen)
            {
                regionSize += Vec2.one;
                points = poissonDiscSampler.GeneratePoints(TARGET_DISTANCE_BETWEEN_NODES, 100, regionSize);
            }

            for (int i = 0; i < bufferLen; i++)
            {
                positionBuffer[i] = points[i].ToVector2();
            }
        }

        void MovePositionsToCenter(Vector2[] positionBuffer, int bufferLen, Vector2 center)
        {
            GetMinAndMax(positionBuffer, bufferLen, out Vector2 min, out Vector2 max);

            float px = (min.x + max.x) / 2;
            float py = (min.y + max.y) / 2;
            float dx = center.x - px;
            float dy = center.y - py;
            for (int i = 0; i < bufferLen; i++)
            {
                var pos = positionBuffer[i];
                pos.x += dx;
                pos.y += dy;
                positionBuffer[i] = pos;
            }
        }

        void CreateNodes(Entity[] entityBuffer, int entityBufferLen, Vector2[] positionBuffer, int positionBufferLen)
        {
            for (int i = 0; i < entityBufferLen && i < positionBufferLen; i++)
            {
                var pos = new Vector3(positionBuffer[i].x, positionBuffer[i].y, 0f);
                entityBuffer[i] = GameObjectEntity.CreateEntity(world, prefabReferences.nodeEntity, pos, Quaternion.identity);
            }
        }

        void GetMinAndMax(Vector2[] positionBuffer, int bufferLen, out Vector2 min, out Vector2 max)
        {
            min = Vector2.zero;
            max = Vector2.zero;
            for (int i = 0; i < bufferLen; i++)
            {
                var entityPos = positionBuffer[i];
                min.x = XIVMathf.Min(min.x, entityPos.x);
                min.y = XIVMathf.Min(min.y, entityPos.y);
                max.x = XIVMathf.Max(max.x, entityPos.x);
                max.y = XIVMathf.Max(max.y, entityPos.y);
            }
        }

        void LinkNodes(Entity[] entityBuffer, int entityBufferLen, Vector2[] positionBuffer, int positionBufferLen)
        {
            var conList1 = XIVPoolSystem.GetItem<DynamicArray<int>>();
            var conList2 = XIVPoolSystem.GetItem<DynamicArray<int>>();
            int connectionCount = 0;
            void AddConnection(int i, int j, ref int connectionCount)
            {
                conList1.Add() = i;
                conList2.Add() = j;
                connectionCount++;
            }
            void RemoveConnection(int index, ref int connectionCount)
            {
                conList1.RemoveAt(index);
                conList2.RemoveAt(index);
                connectionCount--;
            }
            
            // gather possible links
            for (int i = 0; i < entityBufferLen && i < positionBufferLen; i++)
            {
                var currentNodeEntity = entityBuffer[i];
                var currentNodeEntityPos = positionBuffer[i];
                for (int j = 0; j < entityBufferLen; j++)
                {
                    var nextNodeEntity = entityBuffer[j];
                    if (currentNodeEntity == nextNodeEntity) continue;
                    var nextNodeEntityPos = positionBuffer[j];

                    var distance = Vector3.Distance(currentNodeEntityPos, nextNodeEntityPos);
                    if (distance > LINK_DISTANCE) continue;
                    AddConnection(i, j, ref connectionCount);
                }
            }

            // remove intersecting links
            for (int i = connectionCount - 1; i >= 0; i--)
            {
                var currEnt1Idx = conList1[i];
                var currEnt2Idx = conList2[i];
                var currentEntity1 = entityBuffer[currEnt1Idx];
                var currentEntity2 = entityBuffer[currEnt2Idx];
                var p0 = positionBuffer[currEnt1Idx].ToVec2();
                var p1 = positionBuffer[currEnt2Idx].ToVec2();
                for (var j = connectionCount - 1; j >= 0; j--)
                {
                    var otherEnt1Idx = conList1[j];
                    var otherEnt2Idx = conList2[j];
                    var otherEntity1 = entityBuffer[otherEnt1Idx];
                    var otherEntity2 = entityBuffer[otherEnt2Idx];
                    
                    if ((otherEntity1 == currentEntity1 || otherEntity2 == currentEntity1) || (otherEntity1 == currentEntity2 || otherEntity2 == currentEntity2)) continue;
                    
                    var p3 = positionBuffer[otherEnt1Idx].ToVec2();
                    var p4 = positionBuffer[otherEnt2Idx].ToVec2();
                    if (LineMath.IsIntersect(p0, p1, p3, p4))
                    {
                        RemoveConnection(j, ref connectionCount);
                    }
                }
            }
            const float DOT_THRESHOLD = 0.8f;

            for (int i = connectionCount - 1; i >= 0; i--)
            {
                var nodeAIdx = conList1[i];
                var nodeBIdx = conList2[i];
                var posA = positionBuffer[nodeAIdx];
                var posB = positionBuffer[nodeBIdx];
                var dirAb = (posB - posA);
                var dirAbNormalized = dirAb.normalized;
                var distAB = dirAb.sqrMagnitude;

                for (int j = connectionCount - 1; j >= 0; j--)
                {
                    if (i == j) continue;

                    // compare only links starting from the same node
                    if (conList1[j] != nodeAIdx) continue;

                    var nodeCIdx = conList2[j];
                    var posC = positionBuffer[nodeCIdx];
                    var dirAc = (posC - posA);
                    var dirAcNormalized = dirAc.normalized;
                    var distAC = dirAc.sqrMagnitude;

                    float dot = Vector2.Dot(dirAbNormalized, dirAcNormalized);
                    if (dot > DOT_THRESHOLD)
                    {
                        // remove the longer link
                        int index = distAB > distAC ? i : j;
                        RemoveConnection(index, ref connectionCount);
                        break;
                    }
                }
            }
            
            const int LINERENDERER_POSITION_COUNT = 64;
            for (int i = 0, j = 0; i < connectionCount; i++)
            {
                var p0 = positionBuffer[conList1[i]];
                var p1 = positionBuffer[conList2[i]];
                var ent1 = entityBuffer[conList1[i]];
                var ent2 = entityBuffer[conList2[i]];
                
                if (connectionDB.TryAddConnection(ent1, ent2))
                {
                    var lineRenderer = new GameObject(j++ + "_" + ent1 + " - " + ent2).AddComponent<LineRenderer>();
                    lineRenderer.material = prefabReferences.connectionLineRendererMaterial;
                    
                    lineRenderer.positionCount = LINERENDERER_POSITION_COUNT;
                    lineRenderer.XIVStraightLine(p0, p1);
                    lineRenderer.XIVSetWidth(0.05f);
                    var positions = new Vector3[LINERENDERER_POSITION_COUNT];
                    lineRenderer.GetPositions(positions);
                    var lineRendererEntity = world.NewEntity();
                    lineRendererEntity.AddComponent<LineRendererComp>(new LineRendererComp
                    {
                        lineRenderer = lineRenderer,
                        positions = positions,
                    });
                    connectionDB.AssignLineRenderer(ent1, ent2, lineRendererEntity);
                }
            }
            
            XIVPoolSystem.ReleaseItem(conList1);
            XIVPoolSystem.ReleaseItem(conList2);
        }
        
        static int GetNodeQuantity(int mapSize)
        {
            switch (mapSize)
            {
                case 0: return 7;
                case 1: return 12;
                case 2: return 17;
                case 3: return 25;
                case 4: return 61;
                default: return 7;
            }
        }
    }
}