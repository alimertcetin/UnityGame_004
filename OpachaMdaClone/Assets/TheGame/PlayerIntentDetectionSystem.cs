using System;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    [Flags]
    public enum PlayerIntentType
    {
        None = 0,
        SelectNode = 1,
        DeselectNode = 2,
        StartContinuousResourceTransfer = 4,
        CancelContinuousResourceTransfer = 8,
        OneTimeResourceTransfer = 16,
        ToggleHalfResourceTransfer = 32,
    }

    public struct PlayerIntentSelectNodeEventComp : IComponent
    {
        public Entity targetNodeEntity;
    }

    public struct PlayerIntentDeselectNodeEventComp : IComponent
    {
        public Entity targetNodeEntity;
    }

    public class PlayerIntentDetectionSystem : XIV.Ecs.System
    {
        readonly Filter<InputContextEventComp> playerInputContextEventFilter = null;
        readonly ConnectionDB connectionDB = null;

        public override void Update()
        {
            playerInputContextEventFilter.ForEach((Entity inputContextEventEntity, ref InputContextEventComp inputContextEventComp) =>
            {
                void Select(Entity targetEntity)
                {
                    world.NewEntity().AddComponent(new PlayerIntentSelectNodeEventComp
                    {
                        targetNodeEntity = targetEntity,
                    });
                }

                void Deselect(Entity targetEntity)
                {
                    world.NewEntity().AddComponent(new PlayerIntentDeselectNodeEventComp
                    {
                        targetNodeEntity = targetEntity,
                    });
                }
                
                void StartContinuousResourceTransfer(Entity from, Entity to)
                {
                    if (connectionDB.IsConnected(from, to) == false) return;
                    
                    from.RemoveComponent<SendResourceContinuouslyComp>();
                    from.world.NewEntity().AddComponent(new StartContinuousResourceTransferEventComp
                    {
                        fromUnitEpoch = from.GetComponent<NodeComp>().unitEpoch,
                        fromEntity = from,
                        targetEntity = to,
                    });
                }

                void StopContinuousTransfer(Entity nodeEntity)
                {
                    nodeEntity.RemoveComponent<SendResourceContinuouslyComp>();
                }

                void ToggleTransferState(Entity nodeEntity)
                {
                    ref var occupiedNodeComp = ref nodeEntity.GetComponent<OccupiedNodeComp>();
                    switch (occupiedNodeComp.resourceTransferState)
                    {
                        case ResourceTransferState.Full:
                            occupiedNodeComp.resourceTransferState = ResourceTransferState.Half;
                            break;
                        case ResourceTransferState.Half:
                            occupiedNodeComp.resourceTransferState = ResourceTransferState.Full;
                            break;
                        default:
                            break;
                    }
                }

                bool IsPlayerNode(Entity nodeEntity)
                {
                    return nodeEntity.IsAlive() && nodeEntity.HasComponent<OccupiedNodeComp>() && nodeEntity.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().unitType == UnitIdLookup.UnitType.Green;
                }
                
                inputContextEventEntity.Destroy();
                PlayerIntentType playerIntentType = GetPlayerIntent(ref inputContextEventComp);
                Debug.Log("playerIntentType = ".ToColorGreen() + playerIntentType);

                switch (playerIntentType)
                {
                    case PlayerIntentType.None:
                        break;
                    case PlayerIntentType.SelectNode:
                        
                        if (IsPlayerNode(inputContextEventComp.playerContext.inputStartEntity))
                        {
                            Select(inputContextEventComp.playerContext.inputStartEntity);
                        }
                        
                        break;
                    case PlayerIntentType.DeselectNode:
                        
                        if (inputContextEventComp.nodeSelectionContext.selectedNode.IsAlive())
                        {
                            Deselect(inputContextEventComp.nodeSelectionContext.selectedNode);
                        }
                        
                        break;
                    case PlayerIntentType.StartContinuousResourceTransfer:
                        
                        if (IsPlayerNode(inputContextEventComp.playerContext.inputStartEntity))
                        {
                            var targetEntity = GetPossibleTarget(inputContextEventComp.playerContext.inputStartEntity, inputContextEventComp.playerContext.swipeResult.directionVector);
                            if (targetEntity.IsAlive() && targetEntity.HasComponent<NodeComp>() && connectionDB.IsConnected(inputContextEventComp.playerContext.inputStartEntity, targetEntity))
                            {
                                StartContinuousResourceTransfer(inputContextEventComp.playerContext.inputStartEntity, targetEntity);
                            }
                        }
                        
                        break;
                    case PlayerIntentType.CancelContinuousResourceTransfer:
                        
                        if (IsPlayerNode(inputContextEventComp.playerContext.inputStartEntity))
                        {
                            StopContinuousTransfer(inputContextEventComp.playerContext.inputStartEntity);
                        }
                        
                        break;
                    case PlayerIntentType.OneTimeResourceTransfer:

                        if (IsPlayerNode(inputContextEventComp.nodeSelectionContext.selectedNode))
                        {
                            TransferOnce(inputContextEventComp.nodeSelectionContext.selectedNode, inputContextEventComp.playerContext.inputStartEntity);
                        }
                        
                        break;
                    case PlayerIntentType.ToggleHalfResourceTransfer:
                        
                        if (IsPlayerNode(inputContextEventComp.playerContext.inputStartEntity))
                        {
                            ToggleTransferState(inputContextEventComp.playerContext.inputStartEntity);
                        }
                        
                        break;
                    default:
                        UnityEngine.Debug.LogError(playerIntentType + " is not a valid PlayerIntentType");
                        break;
                }
            });
        }
        
        PlayerIntentType GetPlayerIntent(ref InputContextEventComp inputContextEventComp)
        {
            PlayerIntentType playerIntentType = PlayerIntentType.None;
            bool inputStartOnANode = inputContextEventComp.playerContext.inputStartEntity.IsAlive() && inputContextEventComp.playerContext.inputStartEntity.HasComponent<NodeComp>();
            bool inputStartOnOccupiedNode = inputStartOnANode && inputContextEventComp.playerContext.inputStartEntity.HasComponent<OccupiedNodeComp>();
            bool isThereAnActiveSelection = inputContextEventComp.nodeSelectionContext.selectedNode.IsAlive();

            switch (inputContextEventComp.playerContext.playerInputType)
            {
                case PlayerInputType.None:
                    break;
                case PlayerInputType.PointerDown:
                    break;
                case PlayerInputType.Press:
                        
                    if (inputStartOnANode == false)
                    {
                        playerIntentType = isThereAnActiveSelection ? PlayerIntentType.DeselectNode : PlayerIntentType.None;
                        break;
                    }

                    if (isThereAnActiveSelection == false)
                    {
                        playerIntentType = PlayerIntentType.SelectNode;
                        break;
                    }
                        
                    if (inputContextEventComp.nodeSelectionContext.selectedNode == inputContextEventComp.playerContext.inputStartEntity)
                    {
                        playerIntentType = PlayerIntentType.CancelContinuousResourceTransfer;
                        break;
                    }

                    playerIntentType = connectionDB.IsConnected(inputContextEventComp.nodeSelectionContext.selectedNode, inputContextEventComp.playerContext.inputStartEntity)
                        ? PlayerIntentType.OneTimeResourceTransfer
                        : PlayerIntentType.SelectNode;
                        
                        
                    break;
                case PlayerInputType.LongPress:
                    if (isThereAnActiveSelection == false || inputStartOnANode == false)
                    {
                        playerIntentType = PlayerIntentType.None;
                        break;
                    }
                    if (inputContextEventComp.nodeSelectionContext.selectedNode == inputContextEventComp.playerContext.inputStartEntity)
                    {
                        playerIntentType = PlayerIntentType.ToggleHalfResourceTransfer;
                        break;
                    }
                    if (connectionDB.IsConnected(inputContextEventComp.nodeSelectionContext.selectedNode, inputContextEventComp.playerContext.inputStartEntity) == false)
                    {
                        playerIntentType = PlayerIntentType.SelectNode;
                        break;
                    }
                    break;
                case PlayerInputType.Swipe:
                    if (inputStartOnOccupiedNode)
                    {
                        playerIntentType = PlayerIntentType.StartContinuousResourceTransfer;
                        break;
                    }
                    break;
                default:
                    break;
            }
            return playerIntentType;
        }

        public Entity GetPossibleTarget(Entity nodeEntity, Vec2 swipeDirection)
        {
            if (nodeEntity.IsAlive() == false) return Entity.Invalid;

            var dotProduct = 0f;
            var firstNodeEntityTransformPosition = nodeEntity.GetComponent<TransformComp>().transform.position.ToVec3();
            Entity closestEntity = Entity.Invalid;
            using var indexBuffer = ArrayUtils.GetBuffer<int>(connectionDB.Count);
            int len = connectionDB.GetAllConnectionPairs(nodeEntity, indexBuffer);
            for (var i = 0; i < len; i++)
            {
                ref var pair = ref connectionDB[indexBuffer[i]];
                var connectedEntity = pair.GetOpposite(nodeEntity);
                var connectedEntityPos = connectedEntity.GetComponent<TransformComp>().transform.position.ToVec3();
                var dirToConnected = ((Vec2)(connectedEntityPos - firstNodeEntityTransformPosition));
                // Use dot product to define the possible target direction
                var dot = Vec2.Dot(swipeDirection.normalized, dirToConnected.normalized);
                if (dotProduct < dot)
                {
                    dotProduct = dot;
                    closestEntity = connectedEntity;
                }
            }

            return closestEntity;
        }

        void TransferOnce(Entity from, Entity to)
        {
            if (from.IsAlive() == false || to.IsAlive() == false || connectionDB.IsConnected(from, to) == false) return;
            ref var resourceComp = ref from.GetComponent<ResourceComp>();
            var total = resourceComp.resourceQuantity;
            var allResource = from.GetComponent<OccupiedNodeComp>().resourceTransferState == ResourceTransferState.Full;
            var send = (int)(allResource ? total : total * 0.5f);
            if (send == 0) return;
            resourceComp.resourceQuantity -= send;
            
            from.world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromUnitEpoch = from.GetComponent<NodeComp>().unitEpoch,
                fromEntity = from,
                toEntity = to,
                resourceQuantity = send,
            });
        }
    }

}