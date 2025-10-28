using System.Threading;
using UnityEngine;
using XIV.Core.XIVMath;

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
                    for (int i = count - 1; i >= 0; i--)
                    {
                        HandleLineRendererVisual(ref connectionDB[connectionIndices[i]], movementDirections[i], movementPositions[i]);
                        connectionIndices.RemoveLast();
                        movementDirections.RemoveLast();
                        movementPositions.RemoveLast();
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

        void HandleLineRendererVisual(ref ConnectionPair connectionPair, Vector3 movementDirection, Vector3 position)
        {
            int pointCount = connectionPair.positions.Length;
            const float stepOffset = 0.3f;
            const float scale = 0.075f;
            const float frequency = 0.75f;
            const float falloff = 2f;

            Vector3 lineStart = connectionPair.startPosition;
            Vector3 lineEnd = connectionPair.endPosition;
            Vector3 direction = (lineEnd - lineStart).normalized;

            float totalDistance = Vector3.Distance(lineStart, lineEnd);
            if (totalDistance < Mathf.Epsilon) return;

            // Project position onto the line segment (direction-agnostic)
            Vector3 lineVector = lineEnd - lineStart;
            Vector3 toPosition = position - lineStart;
            float projectedLength = Vector3.Dot(toPosition, lineVector.normalized);
            float projectedT = projectedLength / lineVector.magnitude;
            projectedT = Mathf.Clamp01(projectedT); // Clamp to valid range

            // int affectedIndex = Mathf.Clamp((int)(projectedT * (pointCount - 1)), 1, pointCount - 2);

            Vector3 normal = Vector3.Cross(Vector3.forward, direction); // Perpendicular in XY

            for (int i = 1; i < pointCount - 1; i++)
            {
                float t = (float)(i - 1) / pointCount;
                Vector3 basePos = Vector3.LerpUnclamped(lineStart, lineEnd, t);
                var d = Vector3.Dot(movementDirection, position - basePos);
                if (d < 0) continue;
                
                // Distortion falloff based on distance from projection
                float dist = projectedT - t;
                float weight = XIVMathf.Clamp01(1f - XIVMathf.Abs(dist) * falloff); // Falloff multiplier controls width

                if (weight <= 0f) continue;

                float sin = XIVMathf.Sin(dist + stepOffset + (frequency * i)) * scale;
                basePos += normal * (sin * weight);

                connectionPair.positions[i] = basePos;
            }
        }
        
        void FixLineRendererPositions()
        {
            var dt = 0.005f;
            int count = connectionDB.Count;
            for (int i = 0; i < count; i++)
            {
                ref ConnectionPair connectionPair = ref connectionDB[i];
                var startPos = connectionPair.startPosition;
                var endPos = connectionPair.endPosition;
                var positions = connectionPair.positions;
                var positionCount = positions.Length;

                for (int j = 0; j < positionCount; j++)
                {
                    var t = (float)j / positionCount;
                    var targetPos = Vector3.Lerp(startPos, endPos, t);
                    var currentPos = positions[j];
                    var newPos = Vector3.MoveTowards(currentPos, targetPos, dt);
                    positions[j] = newPos;
                }
            }
        }
    }
}