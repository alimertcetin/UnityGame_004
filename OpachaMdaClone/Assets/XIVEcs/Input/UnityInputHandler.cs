using UnityEngine;
using UnityEngine.EventSystems;

namespace XIV.Ecs
{
    public class UnityInputHandler : IInputHandler
    {
        public bool InputOnUI()
        {
            if (EventSystem.current == null)
            {
                return false;
            }
            
            if (Application.isMobilePlatform)
            {
                return EventSystem.current.IsPointerOverGameObject(0);
            }
            
            // Desktop
            return EventSystem.current.IsPointerOverGameObject();
        }

        public bool FingerDownThisFrame()
        {
            return UnityEngine.Input.GetMouseButtonDown(0);
        }

        public Vector3 FingerScreenPos()
        {
            return UnityEngine.Input.mousePosition;
        }

        public bool FingerDown()
        {
            return UnityEngine.Input.GetMouseButton(0);
        }

        public bool IsFingerUpThisFrame()
        {
            return UnityEngine.Input.GetMouseButtonUp(0);
        }
    }
}