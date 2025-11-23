using System.Threading;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.XIVMath;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class ConnectionLineRenderSystem : XIV.Ecs.System
    {
        readonly LineRendererPositionData lineRendererPositionData = null;
        readonly ConnectionDB connectionDB = null;
        
        public override void Start()
        {
            Thread resourceThread = new Thread(p =>
            {
#if UNITY_EDITOR
                bool continueThread = true;
                while (continueThread)
#else
                while (true)
#endif
                {
                    var connectionIndices = lineRendererPositionData.connectionIndices;
                    var movementDirections = lineRendererPositionData.movementDirections;
                    var movementPositions = lineRendererPositionData.movementPositions;
                    // I dont know how it happened but somehow movementPositions were less than others while connectionIndices and movementDirections were exactly has the same amount of items in it.
                    int count = XIVMathInt.Min(XIVMathInt.Min(connectionIndices.Count, movementDirections.Count), movementPositions.Count);
                    for (int i = 0; i < count; i++)
                    {
                        HandleLineRendererVisual(ref connectionDB[connectionIndices[i]], movementDirections[i], movementPositions[i]);
                    }
                    FixLineRendererPositions();
                    connectionIndices.Clear();
                    movementDirections.Clear();
                    movementPositions.Clear();
                    Thread.Sleep(16);
                }
            });
            resourceThread.Start();
        }

        public override void Update()
        {
            AssignLineRendererPositions();
        }

        void AssignLineRendererPositions()
        {
            int count = connectionDB.Count;
            for (int i = 0; i < count; i++)
            {
                // Perf : Skip unmodified lineRenderers
                ref ConnectionPair connectionPair = ref connectionDB[i];
                connectionPair.lineRenderer.SetPositions(connectionPair.positions);
            }
        }

        void HandleLineRendererVisual(ref ConnectionPair connectionPair, Vec3 movementDirection, Vec3 position)
        {
            int pointCount = connectionPair.positions.Length;
            const float stepOffset = 0.3f;
            const float scale = 0.09f;
            const float frequency = 0.9f;
            const float falloff = 2.5f;

            Vec3 lineStart = connectionPair.startPosition;
            Vec3 lineEnd = connectionPair.endPosition;
            Vec3 direction = (lineEnd - lineStart).normalized;

            float totalDistance = Vec3.Distance(lineStart, lineEnd);
            if (totalDistance < Mathf.Epsilon) return;

            // Project position onto the line segment (direction-agnostic)
            Vec3 lineVector = lineEnd - lineStart;
            Vec3 toPosition = position - lineStart;
            float projectedLength = Vec3.Dot(toPosition, lineVector.normalized);
            float projectedT = projectedLength / lineVector.magnitude;
            projectedT = XIVMathf.Clamp01(projectedT); // Clamp to valid range

            // int affectedIndex = Mathf.Clamp((int)(projectedT * (pointCount - 1)), 1, pointCount - 2);

            Vec3 normal = Vec3.Cross(Vec3.forward, direction); // Perpendicular in XY

            for (int i = 1; i < pointCount - 1; i++)
            {
                float t = (float)(i - 1) / pointCount;
                Vec3 basePos = Vec3.LerpUnclamped(lineStart, lineEnd, t);
                var d = Vec3.Dot(movementDirection, position - basePos);
                if (d < 0) continue;
                
                // Distortion falloff based on distance from projection
                float dist = projectedT - t;
                float weight = XIVMathf.Clamp01(1f - XIVMathf.Abs(dist) * falloff); // Falloff multiplier controls width

                if (weight <= 0f) continue;

                float sin = XIVMathf.Sin(dist + stepOffset + (frequency * i)) * scale;
                basePos += normal * (sin * weight);

                connectionPair.positions[i] = basePos.ToVector3();
            }
        }
        
        void FixLineRendererPositions()
        {
            var dt = 0.016f;
            int count = connectionDB.Count;
            for (int i = 0; i < count; i++)
            {
                ref ConnectionPair connectionPair = ref connectionDB[i];
                if (connectionPair.resourceEntitiesOnConnection.Count == 0) continue;
                
                var startPos = connectionPair.startPosition;
                var endPos = connectionPair.endPosition;
                var positions = connectionPair.positions;
                var positionCount = positions.Length;

                for (int j = 0; j < positionCount; j++)
                {
                    var t = (float)j / positionCount;
                    var targetPos = Vec3.Lerp(startPos, endPos, t);
                    var currentPos = positions[j];
                    var newPos = Vec3.MoveTowards(currentPos.ToVec3(), targetPos, dt);
                    positions[j] = newPos.ToVector3();
                }
            }
        }
    }
}