using UnityEngine;
using UnityEngine.EventSystems;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public class InputSystem : XIV.Ecs.System
    {
        public SingleInputData singleInputData;
        public MultiInputData multiInputData;
        public readonly IInputHandler inputHandler = new UnityInputHandler();
        
        readonly Filter<InputListenerComp> inputListenerFilter = null;
        readonly Filter<JoystickComp> joystickFilter = null;
        readonly Filter<JoystickListenerComp> joystickListenerFilter = null;

        public override void Awake()
        {
            UnityEngine.Input.multiTouchEnabled = true;
            float dpi = Screen.dpi;
            if (dpi <= 0)
            {
                dpi = 128f;
            }

            singleInputData.dpi = dpi;
            multiInputData.dpi = dpi;
            multiInputData.activeTouches = new DynamicArray<TouchData>();
            multiInputData.prevActiveTouches = new DynamicArray<TouchData>();

            if (EventSystem.current == null)
            {
                GameObject go = new GameObject("EventSystem - InputSystem");
                go.AddComponent<EventSystem>();
            }

            if (joystickFilter.NumberOfEntities == 1)
            {
                Iterator.Iterate(joystickFilter, joystickListenerFilter, (Entity joystickEntity, ref JoystickComp _,
                    Entity joystickListenerEntity, ref JoystickListenerComp joystickListenerComp) =>
                {
                    if (joystickListenerComp.joystickEntities == null)
                    {
                        joystickListenerComp.joystickEntities = new [] { joystickEntity };
                        joystickListenerComp.inputs = new JoystickInputData[1];
                    }
                });
            }

#if UNITY_EDITOR
            Iterator.Iterate(joystickFilter, joystickListenerFilter, (Entity joystickEntity, ref JoystickComp _,
                Entity joystickListenerEntity, ref JoystickListenerComp joystickListenerComp) =>
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
            UpdateMultiInputData();

            joystickFilter.ForEach(UpdateJoystick);
            
            inputListenerFilter.ForEach((ref InputListenerComp inputListenerComp) =>
            {
                inputListenerComp.singleInput = singleInputData;
                inputListenerComp.multiInput = multiInputData;
            });
            
            joystickListenerFilter.ForEach((ref JoystickListenerComp joystickListenerComp) =>
            {
                for (int i = 0; i < joystickListenerComp.joystickEntities.Length; i++)
                {
                    ref var joystickComp = ref joystickListenerComp.joystickEntities[i].GetComponent<JoystickComp>();
                    joystickListenerComp.inputs[i] = joystickComp.joystickInputData;
                }
            });
        }

        void UpdateInputData()
        {
            singleInputData.isFingerDownThisFrame = false;
            singleInputData.isFingerUpThisFrame = false;

            singleInputData.isOnUI = inputHandler.InputOnUI();

            if (inputHandler.FingerDownThisFrame())
            {
                singleInputData.isFingerDownThisFrame = true;
                singleInputData.totalDeltaMovementInInc = Vec2.zero;
                singleInputData.inputDuration = 0;

                var fingerScreenPos = inputHandler.FingerScreenPos();
                singleInputData.inputScreenPosStart = fingerScreenPos;
                singleInputData.inputScreenPos = fingerScreenPos;

                if (!singleInputData.isOnUI)
                {
                    singleInputData.isFingerDownNoUI = true;
                }
            }

            singleInputData.isFingerDown = inputHandler.FingerDown();

            var inputScreenPos = inputHandler.FingerScreenPos();
            singleInputData.deltaMovementInch = (inputScreenPos - singleInputData.inputScreenPos) / singleInputData.dpi;
            singleInputData.totalDeltaMovementInInc += singleInputData.deltaMovementInch;
            singleInputData.inputScreenPos = inputScreenPos;
            if (inputHandler.FingerDown())
            {
                singleInputData.inputDuration += XTime.unscaledDeltaTime;
            }
                
            if (inputHandler.IsFingerUpThisFrame())
            {
                singleInputData.isFingerUpThisFrame = true;
                singleInputData.isFingerDown = false;
                singleInputData.isFingerDownNoUI = false;
            }
        }

        void UpdateMultiInputData()
        {
            // 1. Cycle previous active touches
            multiInputData.prevActiveTouches.Clear();
            for (int i = 0; i < multiInputData.activeTouches.Count; i++)
            {
                multiInputData.prevActiveTouches.Add() = multiInputData.activeTouches[i];
            }

            int rawTouchCount = inputHandler.GetTouchCount();
            multiInputData.touchCount = rawTouchCount;
            multiInputData.hasActiveTouch = rawTouchCount > 0;
            multiInputData.isAnyTouchDownThisFrame = false;
            multiInputData.isAnyTouchUpThisFrame = false;
            multiInputData.isAllTouchNoUI = true;

            multiInputData.activeTouches.Clear();

            // 2. Gather active touches
            for (int i = 0; i < rawTouchCount; i++)
            {
                if (!inputHandler.TryGetTouch(i, out RawTouch rawTouch)) continue;

                bool isOnUI = inputHandler.InputOnUI(rawTouch.fingerId);

                TouchData touchData = new TouchData
                {
                    fingerId = rawTouch.fingerId,
                    position = rawTouch.position,
                    startPosition = rawTouch.position - rawTouch.deltaPosition,
                    deltaPosition = rawTouch.deltaPosition,
                    deltaMovementInch = rawTouch.deltaPosition / multiInputData.dpi,
                    phase = rawTouch.phase,
                    isOnUI = isOnUI
                };

                if (touchData.phase == InputTouchPhase.Began) multiInputData.isAnyTouchDownThisFrame = true;
                if (touchData.phase == InputTouchPhase.Ended || touchData.phase == InputTouchPhase.Canceled) multiInputData.isAnyTouchUpThisFrame = true;
                if (isOnUI) multiInputData.isAllTouchNoUI = false;

                if (i < MultiInputData.MAX_TOUCHES)
                {
                    multiInputData.activeTouches.Add() = touchData;
                }
            }

            // 3. Match touches that existed in BOTH frames to compute continuous, jump-free Centroid & Spread
            Vec2 currMatchedCentroidSum = Vec2.zero;
            Vec2 prevMatchedCentroidSum = Vec2.zero;
            int matchedTouchCount = 0;

            for (int i = 0; i < multiInputData.activeTouches.Count; i++)
            {
                ref readonly var currTouch = ref multiInputData.activeTouches[i];

                // Skip newly down fingers for delta calculation this frame to avoid centroid jumps
                if (currTouch.phase == InputTouchPhase.Began) continue;

                if (multiInputData.TryGetPrevTouch(currTouch.fingerId, out TouchData prevTouch))
                {
                    currMatchedCentroidSum += currTouch.position;
                    prevMatchedCentroidSum += prevTouch.position;
                    matchedTouchCount++;
                }
            }

            if (matchedTouchCount > 0)
            {
                Vec2 currCentroid = currMatchedCentroidSum / matchedTouchCount;
                Vec2 prevCentroid = prevMatchedCentroidSum / matchedTouchCount;

                multiInputData.centroidPosition = currCentroid;
                multiInputData.prevCentroidPosition = prevCentroid;
                multiInputData.centroidDeltaPixels = currCentroid - prevCentroid;
                multiInputData.centroidDeltaInch = multiInputData.centroidDeltaPixels / multiInputData.dpi;

                // Calculate spread relative to matched touches only
                if (matchedTouchCount >= 2)
                {
                    float currSpreadSum = 0f;
                    float prevSpreadSum = 0f;

                    for (int i = 0; i < multiInputData.activeTouches.Count; i++)
                    {
                        ref readonly var currTouch = ref multiInputData.activeTouches[i];
                        if (currTouch.phase == InputTouchPhase.Began) continue;

                        if (multiInputData.TryGetPrevTouch(currTouch.fingerId, out TouchData prevTouch))
                        {
                            currSpreadSum += Vec2.Distance(currTouch.position, currCentroid);
                            prevSpreadSum += Vec2.Distance(prevTouch.position, prevCentroid);
                        }
                    }

                    multiInputData.averageSpreadPixels = currSpreadSum / matchedTouchCount;
                    multiInputData.prevAverageSpreadPixels = prevSpreadSum / matchedTouchCount;
                    multiInputData.spreadDeltaPixels = multiInputData.averageSpreadPixels - multiInputData.prevAverageSpreadPixels;
                    multiInputData.spreadDeltaInch = multiInputData.spreadDeltaPixels / multiInputData.dpi;
                }
                else
                {
                    ResetSpreadDeltas();
                }
            }
            else
            {
                ResetCentroidAndSpreadDeltas();
            }
        }

        void ResetSpreadDeltas()
        {
            multiInputData.averageSpreadPixels = 0f;
            multiInputData.prevAverageSpreadPixels = 0f;
            multiInputData.spreadDeltaPixels = 0f;
            multiInputData.spreadDeltaInch = 0f;
        }

        void ResetCentroidAndSpreadDeltas()
        {
            multiInputData.centroidPosition = Vec2.zero;
            multiInputData.prevCentroidPosition = Vec2.zero;
            multiInputData.centroidDeltaPixels = Vec2.zero;
            multiInputData.centroidDeltaInch = Vec2.zero;
            ResetSpreadDeltas();
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

            if (singleInputData.isFingerDownThisFrameNoUI)
            {
                if (joystickComp.joystickType == JoystickComp.JoystickType.Static)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(knobContainer,
                            singleInputData.inputScreenPos.ToVector3(),
                            canvasCamera))
                    {
                        joystickInputData.inputStarted = true;
                        joystickInputData.inputStartedThisFrame = true;
                    }
                }
                else if (joystickComp.joystickType != JoystickComp.JoystickType.Static && singleInputData.isOnUI == false)
                {
                    joystickInputData.inputStarted = true;
                    joystickInputData.inputStartedThisFrame = true;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(knobContainerContainer, singleInputData.inputScreenPos.ToVector3(), canvasCamera, out var localPosInKnobContainer);
                    knobContainer.localPosition = localPosInKnobContainer;
                    knobContainer.gameObject.SetActive(true);
                }
            }

            if (joystickInputData.inputStarted && !singleInputData.isFingerDown)
            {
                joystickInputData.inputStarted = false;
                joystickInputData.inputEndedThisFrame = true;
                joystickInputData.inputEndedByFingerUp = singleInputData.isFingerUpThisFrame;
        
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
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(knobContainer, singleInputData.inputScreenPos.ToVector3(),
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