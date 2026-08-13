using UnityEngine;
using UnityEngine.EventSystems;
using XIV.Core.DataStructures;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public class UnityInputHandler : IInputHandler
    {
        Vec2 prevMousePos;

        public bool InputOnUI()
        {
            if (EventSystem.current == null) return false;

            if (Application.isMobilePlatform)
            {
                return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }

            return EventSystem.current.IsPointerOverGameObject();
        }

        public bool InputOnUI(int fingerId)
        {
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject(fingerId);
        }

        public bool FingerDownThisFrame() => Input.GetMouseButtonDown(0);

        public Vec3 FingerScreenPos() => Input.mousePosition.ToVec3();

        public bool FingerDown() => Input.GetMouseButton(0);

        public bool IsFingerUpThisFrame() => Input.GetMouseButtonUp(0);

        // --- Multi-Touch Implementation ---

        public int GetTouchCount()
        {
            int touchCount = Input.touchCount;

            // Desktop Fallback: Return 1 touch if mouse button is held/clicked
#if UNITY_EDITOR || UNITY_STANDALONE
            if (touchCount == 0 && (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)))
            {
                return 1;
            }
#endif
            return touchCount;
        }

        public bool TryGetTouch(int index, out RawTouch touch)
        {
            int touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                if (index >= 0 && index < touchCount)
                {
                    Touch unityTouch = Input.GetTouch(index);
                    touch = new RawTouch
                    {
                        fingerId = unityTouch.fingerId,
                        position = unityTouch.position.ToVec2(),
                        deltaPosition = unityTouch.deltaPosition.ToVec2(),
                        phase = ConvertPhase(unityTouch.phase)
                    };
                    return true;
                }
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            else if (index == 0 && (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)))
            {
                Vec2 currentMousePos = Input.mousePosition.ToVec3();
                Vec2 mouseDelta = currentMousePos - prevMousePos;

                InputTouchPhase phase = InputTouchPhase.Moved;
                if (Input.GetMouseButtonDown(0))
                {
                    phase = InputTouchPhase.Began;
                    mouseDelta = Vec2.zero; // Reset delta on initial click frame
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    phase = InputTouchPhase.Ended;
                }
                else if (mouseDelta.sqrMagnitude < 0.01f)
                {
                    phase = InputTouchPhase.Stationary;
                }

                touch = new RawTouch
                {
                    fingerId = 0,
                    position = currentMousePos,
                    deltaPosition = mouseDelta,
                    phase = phase
                };

                prevMousePos = currentMousePos;
                return true;
            }
#endif

            touch = default;
            return false;
        }

        private static InputTouchPhase ConvertPhase(TouchPhase phase)
        {
            return phase switch
            {
                TouchPhase.Began => InputTouchPhase.Began,
                TouchPhase.Moved => InputTouchPhase.Moved,
                TouchPhase.Stationary => InputTouchPhase.Stationary,
                TouchPhase.Ended => InputTouchPhase.Ended,
                TouchPhase.Canceled => InputTouchPhase.Canceled,
                _ => InputTouchPhase.Canceled
            };
        }
    }
}