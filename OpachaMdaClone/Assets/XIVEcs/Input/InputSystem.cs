using UnityEngine;
using UnityEngine.EventSystems;

namespace XIV.Ecs
{
    public class InputSystem : XIV.Ecs.System
    {
        public InputData inputData;
        public readonly IInputHandler inputHandler = new UnityInputHandler();
        
        readonly Filter<InputListenerComp> inputListenerFilter = null;
        readonly Filter<JoystickComp> joystickFilter = null;
        readonly Filter<JoystickListenerComp> joystickListenerFilter = null;

        public override void Awake()
        {
            UnityEngine.Input.multiTouchEnabled = false;
            float dpi = Screen.dpi;
            if (dpi <= 0)
            {
                dpi = 128f;
            }

            inputData.dpi = dpi;

            if (EventSystem.current == null)
            {
                GameObject go = new GameObject("EventSystem - InputSystem");
                go.AddComponent<EventSystem>();
            }

            if (joystickFilter.NumberOfEntities == 1)
            {
                Iterator.Iterate(joystickFilter,joystickListenerFilter, (Entity joystickEntity, ref JoystickComp _,
                    Entity joystickListenerEntity,ref JoystickListenerComp joystickListenerComp) =>
                {
                    if (joystickListenerComp.joystickEntities == null)
                    {
                        joystickListenerComp.joystickEntities = new []{joystickEntity};
                        joystickListenerComp.inputs = new JoystickInputData[1];
                    }
                });
            }

#if UNITY_EDITOR
            Iterator.Iterate(joystickFilter,joystickListenerFilter, (Entity joystickEntity, ref JoystickComp _,
                Entity joystickListenerEntity,ref JoystickListenerComp joystickListenerComp) =>
            {
                if (joystickListenerComp.joystickEntities == null)
                {
                    Debug.LogError("Please assign joystick to joystick listener, auto assignment does not work if there are multiple joysticks in scene");
                }
            });
#endif
        }

        public override void PreUpdate()
        {
            UpdateInputData();
            joystickFilter.ForEach(UpdateJoystick);
            
            inputListenerFilter.ForEach(((ref InputListenerComp inputListenerComp) =>
            {
                inputListenerComp.input = inputData;
            } ));
            
            joystickListenerFilter.ForEach((ref JoystickListenerComp joystickListenerComp) =>
            {
                for (int i = 0; i < joystickListenerComp.joystickEntities.Length; i++)
                {
                    ref var joystickComp = ref joystickListenerComp.joystickEntities[i].GetComponent<JoystickComp>();
                    joystickListenerComp.inputs[i] = joystickComp.joystickInputData;
                }
            } );
        }

        void UpdateInputData()
        {
            inputData.isFingerDownThisFrame = false;
            inputData.isFingerUpThisFrame = false;

            inputData.isOnUI = inputHandler.InputOnUI();

            if (inputHandler.FingerDownThisFrame())
            {
                inputData.isFingerDownThisFrame = true;
                inputData.totalDeltaMovementInInc = Vector2.zero;
                inputData.inputDuration = 0;

                var fingerScreenPos = inputHandler.FingerScreenPos();
                inputData.inputScreenPosStart = fingerScreenPos;
                inputData.inputScreenPos = fingerScreenPos;

                if (!inputData.isOnUI)
                {
                    inputData.isFingerDownNoUI = true;
                }
            }

            inputData.isFingerDown = inputHandler.FingerDown();

            var inputScreenPos = inputHandler.FingerScreenPos();
            inputData.deltaMovementInch = (inputScreenPos - inputData.inputScreenPos) / inputData.dpi;
            inputData.totalDeltaMovementInInc += inputData.deltaMovementInch;
            inputData.inputScreenPos = inputScreenPos;
            if (inputHandler.FingerDown())
            {
                inputData.inputDuration += XTime.deltaTime;
            }
                
            if (inputHandler.IsFingerUpThisFrame())
            {
                inputData.isFingerUpThisFrame = true;
                inputData.isFingerDown = false;
                inputData.isFingerDownNoUI = false;
            }
        }
        

        void UpdateJoystick(ref JoystickComp joystickComp)
        {
            var knobContainerContainer = joystickComp.knobContainerContainer;
            var knobContainer = joystickComp.knobContainer;
            var knob = joystickComp.knob;
            ref var joystickInputData = ref joystickComp.joystickInputData;
                
            joystickInputData.inputStartedThisFrame = false;
            joystickInputData.inputEndedThisFrame = false;

            Camera canvasCamera = joystickComp.screenSpaceOverlay ? null : Camera.main;

            if (inputData.isFingerDownThisFrameNoUI)
            {
                if (joystickComp.joystickType == JoystickComp.JoystickType.Static)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(knobContainer,
                            inputData.inputScreenPos,
                            canvasCamera))
                    {
                        joystickInputData.inputStarted = true;
                        joystickInputData.inputStartedThisFrame = true;
                    }
                }
                else if (joystickComp.joystickType != JoystickComp.JoystickType.Static && inputData.isOnUI == false)
                {
                    joystickInputData.inputStarted = true;
                    joystickInputData.inputStartedThisFrame = true;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(knobContainerContainer, inputData.inputScreenPos, canvasCamera, out var localPosInKnobContainer);
                    knobContainer.localPosition = localPosInKnobContainer;
                    knobContainer.gameObject.SetActive(true);
                }
            }

            if (joystickInputData.inputStarted && !inputData.isFingerDown)
            {
                joystickInputData.inputStarted = false;
                joystickInputData.inputEndedThisFrame = true;
                joystickInputData.inputEndedByFingerUp = inputData.isFingerUpThisFrame;
        
                if (joystickComp.joystickType != JoystickComp.JoystickType.Static)
                {
                    knobContainer.gameObject.SetActive(false);
                }
        
                knob.anchoredPosition = Vector2.zero;
                joystickInputData.inputAngle = 0;
                joystickInputData.inputDirection = Vector2.zero;
            }
            
        
            if (joystickInputData.inputStarted)
            {
                float knobContainerRadius = knobContainer.sizeDelta.x / 2;
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(knobContainer, inputData.inputScreenPos,
                    canvasCamera, out var localPosInKnobContainer);
                
                if (joystickComp.joystickType == JoystickComp.JoystickType.DynamicAndFloating
                && localPosInKnobContainer.magnitude > knobContainerRadius)
                {
                    Vector3 movement = localPosInKnobContainer.normalized *
                                       (localPosInKnobContainer.magnitude - knobContainerRadius);
                    knobContainer.localPosition += movement;
                }
                
                localPosInKnobContainer = localPosInKnobContainer.normalized * Mathf.Min(knobContainerRadius, localPosInKnobContainer.magnitude);
                
                var input = localPosInKnobContainer / knobContainerRadius;
                var inputAngle = -Vector2.SignedAngle(Vector2.up, input);
                
                knob.anchoredPosition = localPosInKnobContainer;
                joystickInputData.inputAngle = inputAngle;
                joystickInputData.inputDirection = input;
            }
        }
        
       
       
    }
}
