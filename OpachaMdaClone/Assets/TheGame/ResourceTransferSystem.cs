using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheGame.Extensions;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct TransferableResourceComp : IComponent
    {
        public Entity unitEntity;
        public Entity endNodeEntity;
        public int quantity;
        public int connectionIndex;
    }

    public struct PooledComp : IComponent
    {
        public Action<Entity> releaseToPoolAction;
    }

    public class ResourceTransferSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, TransferableResourceComp> resourceFilter = null;
        readonly Filter<TransformComp, NodeComp, OccupiedNodeComp, SendResourceComp> sendResourceFilter = null;
        readonly Filter<NodeComp, OccupiedNodeComp, SendResourceContinuouslyComp> sendResourceContinuouslyFilter = null;
        readonly AssetReferences assetReferences = null;
        readonly Queue<GameObject> resourcePool = new Queue<GameObject>();
        readonly ConnectionDB connectionDB = null;
        readonly LineRendererPositionData lineRendererPositionData = null;
        
        readonly Action<Entity> releaseResourceAction;
        
        public ResourceTransferSystem() : base()
        {
            releaseResourceAction = ReleaseResource;
        }

        public override void Start()
        {
            const int PREWARM_COUNT = 250;
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>(PREWARM_COUNT);
            for (int i = 0; i < PREWARM_COUNT; i++)
            {
                entityBuffer[i] = GetResource(Vector3.zero, Quaternion.identity);
            }

            for (int i = 0; i < PREWARM_COUNT; i++)
            {
                ReleaseResource(entityBuffer[i]);
            }
        }

        public override void Update()
        {
            resourceFilter.ForEach(MoveResourceAlongLine);
            sendResourceFilter.ForEach(SendResource);
            sendResourceContinuouslyFilter.ForEach(SendResourceContinuously);
            
        }

        void MoveResourceAlongLine(Entity resourceEntity, ref PositionComp positionComp, ref TransferableResourceComp transferableResourceComp)
        {
            int idx = transferableResourceComp.connectionIndex;
            ref ConnectionPair pair = ref connectionDB[idx];
            
            GetStartAndTargetPositions(ref transferableResourceComp, ref pair, out var startTransformPosition, out var targetTransformPosition);
            var pos = Vec3.MoveTowards(positionComp.position, targetTransformPosition, 2f * XTime.deltaTime);
            var movementDirection = targetTransformPosition - startTransformPosition;
            
            lineRendererPositionData.connectionIndices.Add() = idx;
            lineRendererPositionData.movementPositions.Add() = pos;
            lineRendererPositionData.movementDirections.Add() = movementDirection;
            positionComp.position = pos;

            if (Vec3.Distance(pos, targetTransformPosition) < 0.02f == false) return;
            
            transferableResourceComp.endNodeEntity.AddComponent(new NodeResourceCollisionComp
            {
                sender = connectionDB[transferableResourceComp.connectionIndex].GetOpposite(transferableResourceComp.endNodeEntity),
                senderUnitEntity = transferableResourceComp.unitEntity,
                quantity = transferableResourceComp.quantity,
            });
            resourceEntity.AddTag<ReturnToPoolTag>();
        }

        void SendResource(Entity entity, ref TransformComp transformComp, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref SendResourceComp sendResourceComp)
        {
            var transform = transformComp.transform;
            var transformPosition = transform.position;
            var transformRotation = transform.rotation;
            var resourceEntity = GetResource(transformPosition, transformRotation);
            // if we process collision before sending the resource it may get negative
            sendResourceComp.resourceQuantity = XIVMathInt.Clamp(sendResourceComp.resourceQuantity, 0, (int)nodeComp.resourceQuantity);
            int connIdx = connectionDB.GetConnectionIndex(entity, sendResourceComp.toEntity);
            
            var resourceComp = new TransferableResourceComp
            {
                unitEntity = occupiedNodeComp.unitEntity,
                endNodeEntity = sendResourceComp.toEntity,
                quantity = sendResourceComp.resourceQuantity,
                connectionIndex = connIdx,
            };
            
            resourceEntity.AddTag<InitTag>();
            resourceEntity.AddComponent(resourceComp);
            resourceEntity.GetComponent<TextComp>().txt.text = resourceComp.quantity.ToString();
            
            var resourceEntityRenderer = resourceEntity.GetComponent<TransformComp>().transform.GetComponent<SpriteRenderer>();
            resourceEntityRenderer.color = UnitIdLookup.GetColor(occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType);
            nodeComp.resourceQuantity -= sendResourceComp.resourceQuantity;
            entity.RemoveComponent<SendResourceComp>();
        }

        void SendResourceContinuously(Entity nodeEntity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref SendResourceContinuouslyComp sendResourceContinuouslyComp)
        {
            sendResourceContinuouslyComp.currentDuration -= XTime.deltaTime;
            if (sendResourceContinuouslyComp.currentDuration > 0) return;
            // a little trick for visual improvement
            if ((int)nodeComp.resourceQuantity == 0)
            {
                sendResourceContinuouslyComp.currentDuration = sendResourceContinuouslyComp.duration;
                return;
            }

            sendResourceContinuouslyComp.currentDuration = sendResourceContinuouslyComp.duration;
            nodeEntity.AddComponent(new SendResourceComp
            {
                resourceQuantity = (int)nodeComp.resourceQuantity,
                toEntity = sendResourceContinuouslyComp.toEntity,
            });
        }

        static void GetStartAndTargetPositions(ref TransferableResourceComp transferableResourceComp, ref ConnectionPair connectionPairComp, out Vec3 startTransformPosition, out Vec3 targetTransformPosition)
        {
            if (connectionPairComp.GetOpposite(transferableResourceComp.endNodeEntity) == connectionPairComp.entity1)
            {
                startTransformPosition = connectionPairComp.startPosition;
                targetTransformPosition = connectionPairComp.endPosition;
            }
            else
            {
                startTransformPosition = connectionPairComp.endPosition;
                targetTransformPosition = connectionPairComp.startPosition;
            }
        }

        Entity GetResource(Vector3 transformPosition, Quaternion transformRotation)
        {
            Entity entity;
            if (resourcePool.Count == 0)
            {
                entity = GameObjectEntity.CreateEntity(world, assetReferences.resourceEntity, transformPosition, transformRotation);
            }
            else
            {
                var go = resourcePool.Dequeue();
                go.transform.position = transformPosition;
                go.transform.rotation = transformRotation;
                go.SetActive(true);
                entity = GameObjectEntity.BindGameObjectToEntity(world, go);
            }
            entity.AddComponent(new PooledComp
            {
                releaseToPoolAction = releaseResourceAction,
            });
            return entity;
        }

        void ReleaseResource(Entity resourceEntity)
        {
            var gameObject = resourceEntity.GetTransform().gameObject;
            resourcePool.Enqueue(gameObject);
            resourceEntity.Unbind();
            gameObject.SetActive(false);
        }
        
    }
}