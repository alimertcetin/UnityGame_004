using UnityEngine;
using XIV.Core.DataStructures;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public static class MultiInputDataExtensions
    {
        public static Ray GetTouchRay(this in MultiInputData multiInputData, int touchIndex, Camera cam)
        {
            if (multiInputData.TryGetTouch(touchIndex, out var touch))
            {
                return cam.ScreenPointToRay(touch.position.ToVector2());
            }
            return default;
        }

        public static Vec3 TouchRayAtPlane(this in MultiInputData multiInputData, int touchIndex, Plane plane, Camera cam)
        {
            var ray = GetTouchRay(in multiInputData, touchIndex, cam);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter).ToVec3();
        }

        public static Vec3 CentroidRayAtPlane(this in MultiInputData multiInputData, Plane plane, Camera cam)
        {
            var ray = cam.ScreenPointToRay(multiInputData.centroidPosition.ToVector2());
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter).ToVec3();
        }

        // ==========================================
        // DUAL-TOUCH SPECIFIC EXTENSIONS
        // ==========================================

        /// <summary>
        /// Calculates exact midpoint between touch 0 and touch 1.
        /// </summary>
        public static bool TryGetDualMidpoint(this in MultiInputData multiInputData, out Vec2 midpoint, out Vec2 midpointDeltaPixels)
        {
            if (multiInputData.touchCount >= 2)
            {
                ref readonly var t0 = ref multiInputData.activeTouches[0];
                ref readonly var t1 = ref multiInputData.activeTouches[1];

                Vec2 currentMidpoint = (t0.position + t1.position) * 0.5f;
                Vec2 prevT0 = t0.position - t0.deltaPosition;
                Vec2 prevT1 = t1.position - t1.deltaPosition;
                Vec2 prevMidpoint = (prevT0 + prevT1) * 0.5f;

                midpoint = currentMidpoint;
                midpointDeltaPixels = currentMidpoint - prevMidpoint;
                return true;
            }

            midpoint = Vec2.zero;
            midpointDeltaPixels = Vec2.zero;
            return false;
        }

        /// <summary>
        /// Calculates exact distance shift between two fingers.
        /// </summary>
        public static bool TryGetDualPinchDelta(this in MultiInputData multiInputData, out float pinchDeltaPixels, out float pinchDeltaInch)
        {
            if (multiInputData.touchCount >= 2)
            {
                ref readonly var t0 = ref multiInputData.activeTouches[0];
                ref readonly var t1 = ref multiInputData.activeTouches[1];

                float currentDist = Vec2.Distance(t0.position, t1.position);
                Vec2 prevT0 = t0.position - t0.deltaPosition;
                Vec2 prevT1 = t1.position - t1.deltaPosition;
                float prevDist = Vec2.Distance(prevT0, prevT1);

                pinchDeltaPixels = currentDist - prevDist;
                pinchDeltaInch = pinchDeltaPixels / multiInputData.dpi;
                return true;
            }

            pinchDeltaPixels = 0f;
            pinchDeltaInch = 0f;
            return false;
        }
    }
}