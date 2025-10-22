using UnityEngine;

namespace TheGame.Extensions
{
    public static class LineRendererExtensions
    {
        public static LineRenderer XIVSetWidth(this LineRenderer lineRenderer, float value)
        {
            lineRenderer.startWidth = value;
            lineRenderer.endWidth = value;
            return lineRenderer;
        }

        public static LineRenderer XIVSetColor(this LineRenderer lineRenderer, Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            return lineRenderer;
        }

        public static LineRenderer XIVSetStartPosition(this LineRenderer lineRenderer, Vector3 position)
        {
            if (lineRenderer.positionCount < 2) lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, position);
            return lineRenderer;
        }

        public static LineRenderer XIVSetEndPosition(this LineRenderer lineRenderer, Vector3 position)
        {
            if (lineRenderer.positionCount < 2) lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
            return lineRenderer;
        }

        public static LineRenderer XIVSetStartEndPosition(this LineRenderer lineRenderer, Vector3 start, Vector3 end)
        {
            return lineRenderer.XIVSetStartPosition(start).XIVSetEndPosition(end);
        }

        public static LineRenderer XIVStraightLine(this LineRenderer lineRenderer, Vector3 start, Vector3 end)
        {
            var lineRendererPositionCount = lineRenderer.positionCount;
            lineRenderer.SetPosition(0, start);

            for (var i = 1; i < lineRendererPositionCount - 1; i++)
            {
                var t = (float)i / lineRendererPositionCount;
                lineRenderer.SetPosition(i, Vector3.Lerp(start, end, t));
            }
            
            lineRenderer.SetPosition(lineRendererPositionCount - 1, end);
            return lineRenderer;
        }
    }
}