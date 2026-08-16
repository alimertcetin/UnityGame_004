using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct CameraPanningTag : ITag { }
    public struct CameraZoomTag : ITag { }

    public class CameraSystem : XIV.Ecs.System
    {
        readonly Filter<InputListenerComp, PositionComp, CameraComp> cameraFilter = new Filter<InputListenerComp, PositionComp, CameraComp>().ExcludeTag<CameraPanningTag>().ExcludeTag<CameraZoomTag>();
        readonly Filter<InputListenerComp, PositionComp, CameraComp> cameraPanningFilter = new Filter<InputListenerComp, PositionComp, CameraComp>().Tag<CameraPanningTag>();
        readonly Filter<InputListenerComp, PositionComp, CameraComp> cameraZoomFilter = new Filter<InputListenerComp, PositionComp, CameraComp>().Tag<CameraPanningTag>();

        public override void Update()
        {
            cameraFilter.ForEach((Entity entity, ref InputListenerComp inputListenerComp, ref PositionComp positionComp, ref CameraComp cameraComp) =>
            {
                ref readonly MultiInputData multiInput = ref inputListenerComp.multiInput;

                // Multi-Touch Zoom (Pinch Spread)
                if (multiInput.touchCount >= 2)
                {
                    entity.AddTag<CameraZoomTag>();
                }
#if UNITY_EDITOR || UNITY_STANDALONE
                else
                {
                    // Desktop Scroll Zoom Fallback
                    float scrollY = Input.mouseScrollDelta.y;
                    if (XIVMathf.Abs(scrollY) > 0.01f)
                    {
                        float targetZoomDelta = -scrollY * 2f;
                        float newSize = cameraComp.camera.orthographicSize + targetZoomDelta;
                        cameraComp.camera.orthographicSize = XIVMathf.Clamp(newSize, 1f, 30f);
                    }
                }
#endif

                // Raycast / Drag Validation
                // Only evaluate raycasts on initial touch down
                if (multiInput.isAnyTouchDownThisFrame == false || multiInput.isAllTouchNoUI == false) return;
                if (multiInput.TryGetTouch(0, out TouchData firstTouch) == false) return;
                
                Ray ray = cameraComp.camera.ScreenPointToRay(firstTouch.position.ToVector2());
                using var arr = ArrayUtils.GetBuffer<RaycastHit>(1);
                // TODO: Use SphereCast?
                int hitCount = Physics.RaycastNonAlloc(ray, arr, 100f, 1 << PhysicsConstants.NodeLayer);

                if (hitCount == 0)
                {
                    // touched area is safe
                    entity.AddTag<CameraPanningTag>();
                    return;
                }
                
                var hitEntity = arr[0].transform.XIVGetEntity();
                if (!hitEntity.HasComponent<OccupiedNodeComp>())
                {
                    // node is not occupied
                    entity.AddTag<CameraPanningTag>();
                    return;
                }
                
                ref var occupiedNodeComp = ref hitEntity.GetComponent<OccupiedNodeComp>();
                // Allow camera drag only if we didn't touch a player unit
                if (occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType != UnitIdLookup.UnitType.Green)
                {
                    entity.AddTag<CameraPanningTag>();
                }
            });
            
            cameraZoomFilter.ForEach((Entity entity, ref InputListenerComp inputListenerComp, ref PositionComp positionComp, ref CameraComp cameraComp) =>
            {
                ref readonly MultiInputData multiInput = ref inputListenerComp.multiInput;
                if (multiInput.touchCount < 2 || multiInput.isAllTouchNoUI == false)
                {
                    entity.RemoveTag<CameraZoomTag>();
                    return;
                }

                if (XIVMathf.Abs(multiInput.spreadDeltaInch) > 0.0001f)
                {
                    float targetZoomDelta = -multiInput.spreadDeltaInch * 12f;
                    float targetSize = XIVMathf.Clamp(cameraComp.camera.orthographicSize + targetZoomDelta, 1f, 30f);

                    // Smoothly decay towards target zoom size
                    // const float zoomSmoothSpeed = 20f;
                    // float t = 1f - Mathf.Exp(-zoomSmoothSpeed * XTime.unscaledDeltaTime);

                    // cameraComp.camera.orthographicSize = XIVMathf.Lerp(cameraComp.camera.orthographicSize, targetSize, t);
                    cameraComp.camera.orthographicSize = targetSize;
                }
            });

            cameraPanningFilter.ForEach((Entity entity, ref InputListenerComp inputListenerComp, ref PositionComp positionComp, ref CameraComp cameraComp) =>
            {
                ref MultiInputData multiInput = ref inputListenerComp.multiInput;

                // Continues seamlessly as long as at least 1 finger remains on screen (CameraPanningTag persists)
                if (multiInput.hasActiveTouch == false || multiInput.isAllTouchNoUI == false)
                {
                    entity.RemoveTag<CameraPanningTag>();
                    return;
                }

                if (XIVMathf.Abs(multiInput.centroidDeltaPixels.sqrMagnitude) < 0.0001f) return;
                
                float unitsPerPixel = (cameraComp.camera.orthographicSize * 2f) / Screen.height;
                Vec3 deltaWorld = new Vec3(
                    -multiInput.centroidDeltaPixels.x * unitsPerPixel,
                    -multiInput.centroidDeltaPixels.y * unitsPerPixel,
                    0f
                );

                Vec3 currentPos = positionComp.position;
                Vec3 targetPos = currentPos + deltaWorld;

                // Higher value = tighter/faster response, Lower value = smoother/looser damping
                // const float smoothSpeed = 25f;
                //
                // // Frame-rate independent exponential decay factor
                // float t = 1f - Mathf.Exp(-smoothSpeed * XTime.unscaledDeltaTime);
                //
                // Vec3 smoothedPos = Vec3.Lerp(currentPos, targetPos, t);
                // positionComp.Set(smoothedPos);
                positionComp.Set(targetPos);
            });
        }
    }
}