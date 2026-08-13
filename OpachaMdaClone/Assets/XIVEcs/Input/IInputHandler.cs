using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public enum InputTouchPhase
    {
        Began,
        Moved,
        Stationary,
        Ended,
        Canceled
    }

    public struct RawTouch
    {
        public int fingerId;
        public Vec2 position;
        public Vec2 deltaPosition;
        public InputTouchPhase phase;
    }
    
    public interface IInputHandler
    {
        // --- Single Pointer / Desktop Compatibility ---
        bool InputOnUI();
        bool FingerDownThisFrame();
        Vec3 FingerScreenPos();
        bool FingerDown();
        bool IsFingerUpThisFrame();

        // --- Low-Level Multi-Touch Abstractions ---
        bool InputOnUI(int fingerId);
        int GetTouchCount();
        bool TryGetTouch(int index, out RawTouch touch);
    }
    
    /*
     
      inputData.isFingerDownThisFrame = false;
        inputData.isFingerUpThisFrame = false;

        inputData.isOnUI = InputOnUI();

        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            inputData.isFingerDownThisFrame = true;
            inputData.inputScreenPosStart = UnityEngine.Input.mousePosition;
            inputData.inputScreenPos = UnityEngine.Input.mousePosition;

            if (!inputData.isOnUI)
            {
                inputData.isFingerDownNoUI = true;
            }
        }

        inputData.isFingerDown = UnityEngine.Input.GetMouseButton(0);

        
        inputData.deltaMovementInch = (UnityEngine.Input.mousePosition - inputData.inputScreenPos) / inputData.dpi;
        inputData.inputScreenPos = UnityEngine.Input.mousePosition;
            
        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            inputData.isFingerUpThisFrame = true;
            inputData.isFingerDown = false;
            inputData.isFingerDownNoUI = false;
        }
     
     */
}