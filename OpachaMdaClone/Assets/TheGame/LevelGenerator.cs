using System.Collections.Generic;
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
        public int tryCount { get; private set; }

        readonly LevelGenerationSettings generationSettings;
        readonly World world;
        readonly AssetReferences assetReferences;
        readonly ConnectionDB connectionDB;

        public LevelGenerator(LevelGenerationSettings generationSettings, World world, AssetReferences assetReferences, ConnectionDB connectionDB)
        {
            this.generationSettings = generationSettings;
            this.world = world;
            this.assetReferences = assetReferences;
            this.connectionDB = connectionDB;
        }

        public void GenerateLevel()
        {
            var prevSeed = XIVRandom.seed;
            XIVRandom.InitState(generationSettings.seed);

            int nodeCount = generationSettings.GetNodeQuantity();
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>(nodeCount);
            using var positionBuffer = ArrayUtils.GetBuffer<Vec2>(nodeCount);
            
            var pivot = new Vec2(-5f, -5f);
            FillPositions(positionBuffer, nodeCount);
            MovePositionsToPivot(positionBuffer, nodeCount, pivot);
            CreateNodes(entityBuffer, nodeCount, positionBuffer, nodeCount);
            
            LinkNodes(entityBuffer, nodeCount, positionBuffer, nodeCount);

            XIVRandom.InitState(prevSeed);
        }

        void FillPositions(Vec2[] positionBuffer, int bufferLen)
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
                positionBuffer[i] = points[i];
            }
        }

        void MovePositionsToPivot(Vec2[] positionBuffer, int bufferLen, Vec2 pivot)
        {
            GetMinAndMax(positionBuffer, bufferLen, out Vec2 min, out Vec2 max);

            float px = (min.x + max.x) / 2f;
            float py = (min.y + max.y) / 2f;
            float dx = pivot.x - px;
            float dy = pivot.y - py;
            for (int i = 0; i < bufferLen; i++)
            {
                ref var pos = ref positionBuffer[i];
                pos.x += dx;
                pos.y += dy;
            }
        }

        void CreateNodes(Entity[] entityBuffer, int entityBufferLen, Vec2[] positionBuffer, int positionBufferLen)
        {
            for (int i = 0; i < entityBufferLen && i < positionBufferLen; i++)
            {
                var pos = new Vector3(positionBuffer[i].x, positionBuffer[i].y, 0f);
                entityBuffer[i] = GameObjectEntity.CreateEntity(world, assetReferences.nodeEntity, pos, Quaternion.identity);
            }
        }

        void LinkNodes(Entity[] entityBuffer, int nodeCount, Vec2[] positionBuffer, int posCount)
        {
            if (nodeCount <= 1) return;

            // 1. Calculate center point and max radius to evaluate centrality
            GetMinAndMax(positionBuffer, nodeCount, out Vec2 min, out Vec2 max);
            Vec2 center = (min + max) * 0.5f;
            float maxRadius = Vec2.Distance(min, max) * 0.5f;

            // 2. Define dynamic max degrees (Center nodes get up to 5-6, Edge nodes get 2-3)
            using var maxDegrees = ArrayUtils.GetBuffer<int>(nodeCount);
            using var currentDegrees = ArrayUtils.GetBuffer<int>(nodeCount);
            for (int i = 0; i < nodeCount; i++)
            {
                float distFromCenter = Vec2.Distance(positionBuffer[i], center);
                float centerNormalized = 1f - XIVMathf.Clamp01(distFromCenter / XIVMathf.Max(maxRadius, 0.001f)); // 1 at center, 0 at outer edge
                // Outer edge nodes max degree: 1 or 2 | Center nodes max degree: 4 to 6
                maxDegrees[i] = XIVMathf.RoundToInt(XIVMathf.Lerp(1.5f, 5.5f, centerNormalized));
            }

            // 3. Delaunay Triangulation (Guarantees planar candidate edges)
            var delaunayEdges = ComputeDelaunayEdges(positionBuffer, nodeCount);
            // 4. Minimum Spanning Tree (Guarantees zero disconnected/dangling nodes)
            var selectedEdges = ComputeMST(nodeCount, positionBuffer, delaunayEdges, out var remainingCandidateEdges);

            int selectedEdgesCount = selectedEdges.Count;
            for (int i = 0; i < selectedEdgesCount; i++)
            {
                var edge = selectedEdges[i];
                currentDegrees[edge.u]++;
                currentDegrees[edge.v]++;
            }

            // Adjacency graph for angle comparison
            List<int>[] adj = new List<int>[nodeCount];
            for (int i = 0; i < nodeCount; i++) adj[i] = new List<int>();
            foreach (var edge in selectedEdges)
            {
                adj[edge.u].Add(edge.v);
                adj[edge.v].Add(edge.u);
            }

            // 5. Smart Loop Edge Re-introduction with Angle and Distance Scoring
            float maxAllowedEdgeLength = generationSettings.linkDistance > 0 ? generationSettings.linkDistance : float.MaxValue;
            
            // Score and sort remaining edges
            List<(Edge edge, float score)> scoredCandidates = new List<(Edge edge, float score)>();

            foreach (var candidate in remainingCandidateEdges)
            {
                // Reject if longer than cutoff threshold
                if (candidate.weight > maxAllowedEdgeLength) continue;

                int u = candidate.u;
                int v = candidate.v;

                // Max degree check
                if (currentDegrees[u] >= maxDegrees[u] || currentDegrees[v] >= maxDegrees[v]) continue;

                // Calculate minimum dot product with existing connected neighbors at both endpoints
                float maxDot = -1f;
                Vec2 dirUV = (positionBuffer[v] - positionBuffer[u]).normalized;
                Vec2 dirVU = -dirUV;

                foreach (int n in adj[u])
                {
                    Vec2 dirUN = (positionBuffer[n] - positionBuffer[u]).normalized;
                    maxDot = XIVMathf.Max(maxDot, Vec2.Dot(dirUV, dirUN));
                }
                foreach (int n in adj[v])
                {
                    Vec2 dirVN = (positionBuffer[n] - positionBuffer[v]).normalized;
                    maxDot = XIVMathf.Max(maxDot, Vec2.Dot(dirVU, dirVN));
                }

                // Reject if angle is too similar to an existing link at either endpoint
                if (maxDot > generationSettings.sameDirectionCutThreshold) continue;

                // Center bonus: Prefer edges near the map center
                float centerFactor = 1f - (Vec2.Distance((positionBuffer[u] + positionBuffer[v]) * 0.5f, center) / XIVMathf.Max(maxRadius, 0.001f));
                
                // Final Score: Lower is better (Short distance + away from center penalty + angle penalty)
                float score = candidate.weight * (1.5f - centerFactor * 0.5f) * (1f + XIVMathf.Max(0f, maxDot));
                scoredCandidates.Add((candidate, score));
            }

            // Sort candidate edges by quality score (best first)
            scoredCandidates.Sort((a, b) => a.score.CompareTo(b.score));

            // Select extra edges based on quality and max degree budget
            foreach (var item in scoredCandidates)
            {
                var edge = item.edge;
                if (currentDegrees[edge.u] < maxDegrees[edge.u] && currentDegrees[edge.v] < maxDegrees[edge.v])
                {
                    selectedEdges.Add(edge);
                    currentDegrees[edge.u]++;
                    currentDegrees[edge.v]++;
                    adj[edge.u].Add(edge.v);
                    adj[edge.v].Add(edge.u);
                }
            }

            // 6. Build final connections
            var conList1 = XIVPoolSystem.GetItem<DynamicArray<int>>();
            var conList2 = XIVPoolSystem.GetItem<DynamicArray<int>>();

            foreach (var edge in selectedEdges)
            {
                conList1.Add() = edge.u;
                conList2.Add() = edge.v;
            }

            CreateLineRenderers(entityBuffer, positionBuffer, selectedEdges.Count, conList1, conList2);

            conList1.Clear();
            conList2.Clear();
            XIVPoolSystem.ReleaseItem(conList1);
            XIVPoolSystem.ReleaseItem(conList2);
        }

        #region Graph Algorithms (Delaunay & MST)

        struct Edge
        {
            public int u;
            public int v;
            public float weight;

            public Edge(int u, int v, float weight)
            {
                this.u = u < v ? u : v;
                this.v = u < v ? v : u;
                this.weight = weight;
            }
        }

        List<Edge> ComputeDelaunayEdges(Vec2[] positions, int count)
        {
            GetMinAndMax(positions, count, out Vec2 min, out Vec2 max);
            float dx = max.x - min.x;
            float dy = max.y - min.y;
            float deltaMax = XIVMathf.Max(dx, dy) * 10f;

            Vec2 p1 = new Vec2(min.x - deltaMax, min.y - deltaMax);
            Vec2 p2 = new Vec2(min.x + deltaMax * 2f, min.y - deltaMax);
            Vec2 p3 = new Vec2(min.x + deltaMax / 2f, min.y + deltaMax * 2f);

            var allPoints = new List<Vec2>(count + 3);
            for (int i = 0; i < count; i++) allPoints.Add(positions[i]);
            allPoints.Add(p1);
            allPoints.Add(p2);
            allPoints.Add(p3);

            var triangles = new List<Triangle>
            {
                new Triangle(count, count + 1, count + 2, allPoints)
            };

            for (int i = 0; i < count; i++)
            {
                var badTriangles = new List<Triangle>();
                for (int t = 0; t < triangles.Count; t++)
                {
                    if (triangles[t].IsPointInsideCircumcircle(positions[i]))
                    {
                        badTriangles.Add(triangles[t]);
                    }
                }

                var polygon = new List<Edge>();
                for (int t = 0; t < badTriangles.Count; t++)
                {
                    var tri = badTriangles[t];
                    AddEdgeToPolygon(polygon, tri.a, tri.b, positions);
                    AddEdgeToPolygon(polygon, tri.b, tri.c, positions);
                    AddEdgeToPolygon(polygon, tri.c, tri.a, positions);
                    triangles.Remove(tri);
                }

                for (int e = 0; e < polygon.Count; e++)
                {
                    triangles.Add(new Triangle(polygon[e].u, polygon[e].v, i, allPoints));
                }
            }

            var edgeSet = new HashSet<(int, int)>();
            var result = new List<Edge>();

            foreach (var tri in triangles)
            {
                if (tri.a >= count || tri.b >= count || tri.c >= count) continue;

                AddUniqueEdge(tri.a, tri.b, positions, edgeSet, result);
                AddUniqueEdge(tri.b, tri.c, positions, edgeSet, result);
                AddUniqueEdge(tri.c, tri.a, positions, edgeSet, result);
            }

            return result;
        }

        void AddEdgeToPolygon(List<Edge> polygon, int u, int v, Vec2[] positions)
        {
            var edge = new Edge(u, v, 0f);
            for (int i = polygon.Count - 1; i >= 0; i--)
            {
                if (polygon[i].u == edge.u && polygon[i].v == edge.v)
                {
                    polygon.RemoveAt(i);
                    return;
                }
            }
            polygon.Add(edge);
        }

        void AddUniqueEdge(int u, int v, Vec2[] positions, HashSet<(int, int)> set, List<Edge> edges)
        {
            int min = u < v ? u : v;
            int max = u < v ? v : u;
            if (set.Add((min, max)))
            {
                float weight = Vec2.Distance(positions[min], positions[max]);
                edges.Add(new Edge(min, max, weight));
            }
        }

        List<Edge> ComputeMST(int nodeCount, Vec2[] positions, List<Edge> delaunayEdges, out List<Edge> remainingEdges)
        {
            var mst = new List<Edge>();
            remainingEdges = new List<Edge>(delaunayEdges);

            if (nodeCount == 0 || delaunayEdges.Count == 0) return mst;

            var adj = new List<Edge>[nodeCount];
            for (int i = 0; i < nodeCount; i++) adj[i] = new List<Edge>();
            foreach (var e in delaunayEdges)
            {
                adj[e.u].Add(e);
                adj[e.v].Add(e);
            }

            var inMST = new bool[nodeCount];
            inMST[0] = true;

            for (int step = 0; step < nodeCount - 1; step++)
            {
                Edge minEdge = default;
                float minWeight = float.MaxValue;
                bool found = false;

                for (int i = 0; i < nodeCount; i++)
                {
                    if (!inMST[i]) continue;

                    foreach (var edge in adj[i])
                    {
                        int other = (edge.u == i) ? edge.v : edge.u;
                        if (!inMST[other] && edge.weight < minWeight)
                        {
                            minWeight = edge.weight;
                            minEdge = edge;
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    mst.Add(minEdge);
                    inMST[minEdge.u] = true;
                    inMST[minEdge.v] = true;
                    remainingEdges.RemoveAll(e => e.u == minEdge.u && e.v == minEdge.v);
                }
            }

            return mst;
        }

        struct Triangle
        {
            public int a, b, c;
            public Vec2 center;
            public float radiusSqr;

            public Triangle(int a, int b, int c, List<Vec2> points)
            {
                this.a = a;
                this.b = b;
                this.c = c;

                Vec2 pA = points[a];
                Vec2 pB = points[b];
                Vec2 pC = points[c];

                float d = 2 * (pA.x * (pB.y - pC.y) + pB.x * (pC.y - pA.y) + pC.x * (pA.y - pB.y));
                if (XIVMathf.Abs(d) < 0.0001f)
                {
                    center = pA;
                    radiusSqr = float.MaxValue;
                    return;
                }

                float ux = ((pA.x * pA.x + pA.y * pA.y) * (pB.y - pC.y) + (pB.x * pB.x + pB.y * pB.y) * (pC.y - pA.y) + (pC.x * pC.x + pC.y * pC.y) * (pA.y - pB.y)) / d;
                float uy = ((pA.x * pA.x + pA.y * pA.y) * (pC.x - pB.x) + (pB.x * pB.x + pB.y * pB.y) * (pA.x - pC.x) + (pC.x * pC.x + pC.y * pC.y) * (pB.x - pA.x)) / d;

                center = new Vec2(ux, uy);
                radiusSqr = (pA - center).sqrMagnitude;
            }

            public bool IsPointInsideCircumcircle(Vec2 point)
            {
                return (point - center).sqrMagnitude <= radiusSqr;
            }
        }

        #endregion

        void CreateLineRenderers(Entity[] entityBuffer, Vec2[] positionBuffer, int connectionCount, DynamicArray<int> conList1, DynamicArray<int> conList2)
        {
            const int LINERENDERER_POSITION_COUNT = 32;
            for (int connectionIdx = 0; connectionIdx < connectionCount; connectionIdx++)
            {
                var ent1 = entityBuffer[conList1[connectionIdx]];
                var ent2 = entityBuffer[conList2[connectionIdx]];

                if (connectionDB.IsConnected(ent1, ent2)) continue;

                var p0 = positionBuffer[conList1[connectionIdx]];
                var p1 = positionBuffer[conList2[connectionIdx]];

                var lineRendererEntity = GameObjectEntity.CreateEntity(world, assetReferences.connectionLineRendererPrefab);
                var lineRenderer = lineRendererEntity.GetComponent<TransformComp>().transform.GetComponent<LineRenderer>();

                lineRendererEntity.AddComponent(new LineRendererComp { lineRenderer = lineRenderer });
                lineRendererEntity.AddComponent(new InstancedRendererComp
                {
                    renderer = lineRenderer.GetComponent<Renderer>(),
                    materialPropertyBlock = new MaterialPropertyBlock(),
                });

#if UNITY_EDITOR
                lineRenderer.gameObject.name = connectionDB.Count + " - " + ent1 + " <-> " + ent2;
#endif
                lineRendererEntity.GetComponent<ScaleComp>().Set(ent1.GetComponent<ScaleComp>().scale);
                lineRenderer.positionCount = LINERENDERER_POSITION_COUNT;
                lineRenderer.XIVStraightLine(p0.ToVector2(), p1.ToVector2());
                lineRenderer.XIVSetWidth(0.1f);
                var positions = new Vector3[LINERENDERER_POSITION_COUNT];
                lineRenderer.GetPositions(positions);

                connectionDB.AddConnection(ent1, ent2, p0, p1, positions, lineRendererEntity);
            }
        }

        static void GetMinAndMax(Vec2[] positionBuffer, int bufferLen, out Vec2 min, out Vec2 max)
        {
            if (bufferLen == 0)
            {
                min = Vec2.zero;
                max = Vec2.zero;
                return;
            }

            min = positionBuffer[0];
            max = positionBuffer[0];
            for (int i = 1; i < bufferLen; i++)
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