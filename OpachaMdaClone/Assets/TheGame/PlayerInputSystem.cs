using UnityEngine;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;

namespace TheGame
{
    public enum PlayerInputType
    {
        None,
        PointerDown, // occurs immediately on pointer down
        Press, // occurs on pointer up
        LongPress,
        Swipe,
    }

    public struct PlayerInputContext
    {
        public PlayerInputType playerInputType;
        public SwipeResult swipeResult;
        public Entity inputStartEntity;
        public Entity inputEndEntity; // entity that's on the inputEndPosition
    }

    public struct NodeSelectionContext
    {
        public Entity previousSelectedNode;
        public Entity selectedNode;
    }

    public struct InputContextEventComp : IComponent
    {
        public PlayerInputContext playerContext;
        public NodeSelectionContext nodeSelectionContext;
    }
    
    public class PlayerInputSystem : XIV.Ecs.System
    {
        readonly Filter<SwipeDetectorComp, InputListenerComp> swipeListenerFilter = null;
        readonly Filter<CameraComp, NodeSelectionListenerComp, SwipeDetectorComp, InputListenerComp> inputListenerFilter = null;
        const float LONG_PRESS_TIME = 0.75f;

        public override void Update()
        {
            swipeListenerFilter.ForEach(UpdateSwipeInput);
            
            inputListenerFilter.ForEach((Entity cameraEntity, ref CameraComp cameraComp, ref NodeSelectionListenerComp nodeSelectionListenerComp, ref SwipeDetectorComp swipeDetectorComp, ref InputListenerComp inputListenerComp) =>
            {
                PlayerInputType playerInputType = PlayerInputType.None;
                SwipeResult swipeResult = swipeDetectorComp.swipeResult;
                bool isPressedOnANode = TryGetEntityFromInput(inputListenerComp.singleInput.GetInputStartRay(cameraComp.camera), out var inputStartEntity);
                TryGetEntityFromInput(inputListenerComp.singleInput.GetInputRay(cameraComp.camera), out var inputEndEntity);
                
                if (swipeDetectorComp.swipeResult.direction != SwipeResult.Direction.None && isPressedOnANode) // swipe is only detected on finger up
                {
                    playerInputType = PlayerInputType.Swipe;
                    goto CREATE_CONTEXT;
                }

                if (inputListenerComp.singleInput.isFingerDownThisFrameNoUI)
                {
                    playerInputType = PlayerInputType.PointerDown;
                }

                if (inputListenerComp.singleInput.isFingerDownNoUI && inputListenerComp.singleInput.inputDuration >= LONG_PRESS_TIME && inputListenerComp.singleInput.inputDuration - XTime.unscaledDeltaTime < LONG_PRESS_TIME && inputListenerComp.singleInput.totalDeltaMovementInInc.sqrMagnitude < 0.1f)
                {
                    playerInputType = PlayerInputType.LongPress;
                }

                // Debug.Log("inputListenerComp.singleInput.totalDeltaMovementInInc.sqrMagnitude = " + inputListenerComp.singleInput.totalDeltaMovementInInc.sqrMagnitude);
                if (inputListenerComp.singleInput.isFingerUpThisFrame && inputListenerComp.singleInput.isOnUI == false && inputListenerComp.singleInput.inputDuration < LONG_PRESS_TIME && inputListenerComp.singleInput.totalDeltaMovementInInc.sqrMagnitude < 0.1f)
                {
                    playerInputType = PlayerInputType.Press;
                }

                CREATE_CONTEXT:
                if (playerInputType != PlayerInputType.None)
                {
                    Debug.Log("playerInputType = ".ToColorYellow() + playerInputType);
                    world.NewEntity().AddComponent(new InputContextEventComp
                    {
                        playerContext = new PlayerInputContext
                        {
                            playerInputType = playerInputType,
                            swipeResult = swipeResult,
                            inputStartEntity = inputStartEntity,
                            inputEndEntity = inputEndEntity,
                        },
                        nodeSelectionContext = new NodeSelectionContext
                        {
                            previousSelectedNode = nodeSelectionListenerComp.previousSelectedNode,
                            selectedNode = nodeSelectionListenerComp.selectedNode,
                        },
                    });
                }
                
            });
        }
        void UpdateSwipeInput(ref SwipeDetectorComp swipeDetectorComp, ref InputListenerComp inputListenerComp)
        {
            swipeDetectorComp.swipeResult = swipeDetectorComp.swipeDetector.DetectSwipe(ref inputListenerComp.singleInput, XTime.unscaledDeltaTime);
        }

        static bool TryGetEntityFromInput(Ray inputRay, out Entity entity)
        {
            using var hits = ArrayUtils.GetBuffer<RaycastHit>(1);
            // 0.03125f = 1 / 32, a little outline for input to detect edge presses? Might work, might work unexpectedly...
            int hitCount = Physics.SphereCastNonAlloc(inputRay, LevelGenerationSettings.NODE_RADIUS * 0.03125f, hits, 100f, 1 << PhysicsConstants.NodeLayer);
            if (hitCount > 0)
            {
                entity = hits[0].collider.XIVGetEntity();
                return entity.IsAlive() && entity.HasComponent<NodeComp>();
            }

            entity = Entity.Invalid;
            return false;
        }
    }
}