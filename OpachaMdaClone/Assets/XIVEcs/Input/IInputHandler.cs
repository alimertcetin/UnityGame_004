using UnityEngine;

namespace XIV.Ecs
{
    public interface IInputHandler
    {
        public bool InputOnUI();
        public bool FingerDownThisFrame();
        public Vector3 FingerScreenPos();
        public bool FingerDown();
        public bool IsFingerUpThisFrame();
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