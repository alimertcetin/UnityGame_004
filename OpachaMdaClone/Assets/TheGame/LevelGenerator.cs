using TheGame.Extensions;
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
    public class LevelGenerator
    {
        /// <summary>
        /// Returns how many tries take to generate the level
        /// </summary>
        public int tryCount { get; private set; }

        LevelGenerationSettings generationSettings;
        readonly World world;
        readonly PrefabReferences prefabReferences;
        readonly ConnectionDB connectionDB;

        public LevelGenerator(LevelGenerationSettings generationSettings, World world, PrefabReferences prefabReferences, ConnectionDB connectionDB)
        {
            this.generationSettings = generationSettings;
            this.world = world;
            this.prefabReferences = prefabReferences;
            this.connectionDB = connectionDB;
        }

        public void GenerateLevel()
        {
            var prevSeed = XIVRandom.seed;
            XIVRandom.InitState(generationSettings.seed);
            
            // TODO : NodeLevelGeneratorSystem -> add map size variable
            int bufferLen = generationSettings.GetNodeQuantity();
            using var entityBufferDispose = ArrayUtils.GetBuffer(out Entity[] entityBuffer, bufferLen);
            using var positionBufferDispose = ArrayUtils.GetBuffer(out Vector2[] positionBuffer, bufferLen);
            var pivot = new Vector2(-5f, -5f);
            FillPositions(positionBuffer, bufferLen);
            MovePositionsToPivot(positionBuffer, bufferLen, pivot);
            CreateNodes(entityBuffer, bufferLen, positionBuffer, bufferLen);
            LinkNodes(entityBuffer, bufferLen, positionBuffer, bufferLen);
            
            XIVRandom.InitState(prevSeed);
        }

        void FillPositions(Vector2[] positionBuffer, int bufferLen)
        {
            PoissonDiscSampler poissonDiscSampler = new PoissonDiscSampler();
            tryCount = 1;
            var points = poissonDiscSampler.GeneratePoints(generationSettings.targetDistanceBetweenNodes, 100, generationSettings.regionSize);
            while (points.Length < bufferLen)
            {
                tryCount++;
                var newRegionSize = Vec2.one * tryCount;
                points = poissonDiscSampler.GeneratePoints(generationSettings.targetDistanceBetweenNodes, 100, newRegionSize);
            }

            for (int i = 0; i < bufferLen; i++)
            {
                positionBuffer[i] = points[i].ToVector2();
            }
        }

        void MovePositionsToPivot(Vector2[] positionBuffer, int bufferLen, Vector2 pivot)
        {
            GetMinAndMax(positionBuffer, bufferLen, out Vector2 min, out Vector2 max);

            float px = (min.x + max.x) / 2;
            float py = (min.y + max.y) / 2;
            float dx = pivot.x - px;
            float dy = pivot.y - py;
            for (int i = 0; i < bufferLen; i++)
            {
                ref var pos = ref positionBuffer[i];
                pos.x += dx;
                pos.y += dy;
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
                    if (distance > generationSettings.linkDistance) continue;
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
            
            // Remove connections made in similar directions
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
                    if (dot > generationSettings.sameDirectionCutThreshold)
                    {
                        // remove the longer link
                        int index = distAB > distAC ? i : j;
                        RemoveConnection(index, ref connectionCount);
                        break;
                    }
                }
            }
            
            const int LINERENDERER_POSITION_COUNT = 32; // link detail
            for (int connectionIdx = 0; connectionIdx < connectionCount; connectionIdx++)
            {
                var ent1 = entityBuffer[conList1[connectionIdx]];
                var ent2 = entityBuffer[conList2[connectionIdx]];
                
                ref var connectionPair = ref connectionDB.AddConnection(ent1, ent2, out var isAdded);
                if (isAdded == false) continue;
                
                var p0 = positionBuffer[conList1[connectionIdx]];
                var p1 = positionBuffer[conList2[connectionIdx]];
                var lineRenderer = Object.Instantiate(prefabReferences.connectionLineRendererPrefab).GetComponent<LineRenderer>();
#if UNITY_EDITOR
                lineRenderer.gameObject.name = connectionDB.Count + " - " + ent1 + " <-> " + ent2;
#endif
                lineRenderer.transform.localScale = ent1.GetComponent<TransformComp>().transform.localScale;
                lineRenderer.positionCount = LINERENDERER_POSITION_COUNT;
                lineRenderer.XIVStraightLine(p0, p1);
                lineRenderer.XIVSetWidth(0.1f);
                var positions = new Vector3[LINERENDERER_POSITION_COUNT];
                lineRenderer.GetPositions(positions);

                connectionPair.entity1 = ent1;
                connectionPair.entity2 = ent2;
                connectionPair.startPosition = p0;
                connectionPair.endPosition = p1;
                connectionPair.positions = positions;
                connectionPair.lineRenderer = lineRenderer;
            }
            
            XIVPoolSystem.ReleaseItem(conList1);
            XIVPoolSystem.ReleaseItem(conList2);
        }


        static void GetMinAndMax(Vector2[] positionBuffer, int bufferLen, out Vector2 min, out Vector2 max)
        {
            min = Vector2.zero;
            max = Vector2.zero;
            for (int i = 0; i < bufferLen; i++)
            {
                ref var p = ref positionBuffer[i];
                min.x = XIVMathf.Min(min.x, p.x);
                min.y = XIVMathf.Min(min.y, p.y);
                max.x = XIVMathf.Max(max.x, p.x);
                max.y = XIVMathf.Max(max.y, p.y);
            }
        }
    }
}