using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct DraggableTag : ITag
    {
    }

    public class CameraSystem : XIV.Ecs.System
    {
        readonly Filter<InputListenerComp, PositionComp, CameraComp> cameraFilter = null;
        readonly Filter<InputListenerComp, PositionComp, CameraComp> draggableCameraFilter = new Filter<InputListenerComp, PositionComp, CameraComp>().Tag<DraggableTag>();

        public override void Update()
        {
            cameraFilter.ForEach((Entity entity, ref InputListenerComp inputListenerComp, ref PositionComp positionComp, ref CameraComp cameraComp) =>
            {
                MultiInputData multiInput = inputListenerComp.multiInput;

                // 1. Raycast / Drag Validation (Triggers on any finger down frame when no UI is hit)
                if (multiInput.isAnyTouchDownThisFrame && multiInput.isAllTouchNoUI)
                {
                    if (multiInput.TryGetTouch(0, out TouchData firstTouch))
                    {
                        Ray ray = cameraComp.camera.ScreenPointToRay(firstTouch.position.ToVector2());
                        using var arr = ArrayUtils.GetBuffer<RaycastHit>(1);
                        int hitCount = Physics.RaycastNonAlloc(ray, arr, 100f, 1 << PhysicsConstants.NodeLayer);

                        if (hitCount == 0)
                        {
                            entity.AddTag<DraggableTag>();
                        }
                        else
                        {
                            var hitEntity = arr[0].transform.XIVGetEntity();
                            if (hitEntity.HasComponent<OccupiedNodeComp>())
                            {
                                ref var occupiedNodeComp = ref hitEntity.GetComponent<OccupiedNodeComp>();
                                // Allow camera drag only if we didn't touch a player unit
                                if (occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType != UnitIdLookup.UnitType.Green)
                                {
                                    entity.AddTag<DraggableTag>();
                                }
                            }
                        }
                    }
                }

                // 2. Cleanup drag tag on all fingers up
                if (!multiInput.hasActiveTouch || multiInput.isAnyTouchUpThisFrame)
                {
                    entity.RemoveTag<DraggableTag>();
                }

                // 3. Multi-Touch Zoom (Pinch Spread) & Secondary Pan Logic
                if (multiInput.touchCount >= 2)
                {
                    if (!multiInput.isAllTouchNoUI) return;

                    // Pinch Zoom
                    if (XIVMathf.Abs(multiInput.spreadDeltaInch) > 0.0001f)
                    {
                        float targetZoomDelta = -multiInput.spreadDeltaInch * 12f;
                        float newSize = cameraComp.camera.orthographicSize + targetZoomDelta;
                        cameraComp.camera.orthographicSize = XIVMathf.Clamp(newSize, 1f, 30f);
                    }
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
            });

            // 4. Simultaneous Pan Phase (Works seamlessly for 1 finger, 2 fingers, or N fingers)
            draggableCameraFilter.ForEach((Entity cameraEntity, ref InputListenerComp inputListenerComp, ref PositionComp positionComp, ref CameraComp cameraComp) =>
            {
                MultiInputData multiInput = inputListenerComp.multiInput;

                if (!multiInput.hasActiveTouch || !multiInput.isAllTouchNoUI) return;

                if (multiInput.centroidDeltaPixels.sqrMagnitude > 0.0001f)
                {
                    float unitsPerPixel = (cameraComp.camera.orthographicSize * 2f) / Screen.height;
                    Vec3 moveVector = new Vec3(
                        -multiInput.centroidDeltaPixels.x * unitsPerPixel,
                        -multiInput.centroidDeltaPixels.y * unitsPerPixel,
                        0f
                    );
                    positionComp.Set(positionComp.position + moveVector);
                }
            });
        }
    }
}