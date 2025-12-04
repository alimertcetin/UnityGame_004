using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct TransferableResourceComp : IComponent
    {
        public Entity unitEntity;
        public Entity endNodeEntity;
        // TODO: Integrate with ResourceComp
        public int quantity;
        public int connectionIndex;
    }

    public struct PooledComp : IComponent
    {
        public Action<Entity> releaseToPoolAction;
    }

    public struct SendResourceContinuouslyComp : IComponent
    {
        public Entity toEntity;
        public Timer sendTimer;
    }

    public struct StartContinuousResourceTransferEventComp : IComponent
    {
        public Entity fromEntity;
        public Entity targetEntity;
        public float sendInterval; // in seconds
    }
    
    public struct SendResourceEventComp : IComponent
    {
        public Entity fromEntity;
        public Entity toEntity;
        public int resourceQuantity;
    }

    public class ResourceTransferSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, TransferableResourceComp> transferableResourceFilter = null;
        readonly Filter<SendResourceEventComp> sendResourceFilter = null;
        readonly Filter<ResourceComp, SendResourceContinuouslyComp> sendResourceContinuouslyFilter = null;
        readonly Filter<StartContinuousResourceTransferEventComp> startContinuousResourceTransferFilter = null;
        
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
                entityBuffer[i] = GetResource(Vec3.zero);
            }

            for (int i = 0; i < PREWARM_COUNT; i++)
            {
                ReleaseResource(entityBuffer[i]);
            }
        }

        public override void Update()
        {
            startContinuousResourceTransferFilter.ForEach(StartContinuousResourceTransfer);
            transferableResourceFilter.ForEach(MoveResourceAlongLine);
            sendResourceFilter.ForEach(SendResource);
            sendResourceContinuouslyFilter.ForEach(SendResourceContinuously);
        }

        void StartContinuousResourceTransfer(Entity entity, ref StartContinuousResourceTransferEventComp startContinuousResourceTransferComp)
        {
            entity.Destroy();
            var sendTimer = new Timer(startContinuousResourceTransferComp.sendInterval);
            sendTimer.Update(float.MaxValue); // send immediate
            startContinuousResourceTransferComp.fromEntity.AddComponent(new SendResourceContinuouslyComp
            {
                toEntity = startContinuousResourceTransferComp.targetEntity,
                sendTimer = sendTimer,
            });
            // world.NewEntity().AddComponent(new CreateResourceTransferIndicatorEventComp()
            // {
            //     fromEntity = startContinuousResourceTransferComp.fromEntity,
            //     targetEntity = startContinuousResourceTransferComp.targetEntity,
            // });
        }

        void MoveResourceAlongLine(Entity resourceEntity, ref PositionComp positionComp, ref TransferableResourceComp transferableResourceComp)
        {
            int idx = transferableResourceComp.connectionIndex;
            ref ConnectionPair pair = ref connectionDB[idx];
            
            GetStartAndTargetPositions(ref transferableResourceComp, ref pair, out var startTransformPosition, out var targetTransformPosition);
            var pos = Vec3.MoveTowards(positionComp.position, targetTransformPosition, GameConstants.RESOURCE_MOVEMENT_SPEED * XTime.deltaTime);
            var movementDirection = targetTransformPosition - startTransformPosition;
            
            lineRendererPositionData.connectionIndices.Add() = idx;
            lineRendererPositionData.movementPositions.Add() = pos;
            lineRendererPositionData.movementDirections.Add() = movementDirection;
            positionComp.Set(pos);

            if (Vec3.Distance(pos, targetTransformPosition) < 0.02f == false) return;
            
            transferableResourceComp.endNodeEntity.AddComponent(new NodeResourceCollisionComp
            {
                senderEntity = connectionDB[transferableResourceComp.connectionIndex].GetOpposite(transferableResourceComp.endNodeEntity),
                senderUnitEntity = transferableResourceComp.unitEntity,
                quantity = transferableResourceComp.quantity,
            });
            resourceEntity.AddTag<ReturnToPoolTag>();
        }

        void SendResource(Entity entity, ref SendResourceEventComp sendResourceEventComp)
        {
            entity.Destroy();
            ref var positionComp = ref sendResourceEventComp.fromEntity.GetComponent<PositionComp>();
            ref var resourceComp = ref sendResourceEventComp.fromEntity.GetComponent<ResourceComp>();
            ref var occupiedNodeComp = ref sendResourceEventComp.fromEntity.GetComponent<OccupiedNodeComp>();
            
            var resourceEntity = GetResource(positionComp.position);
            sendResourceEventComp.resourceQuantity = XIVMathInt.Clamp(sendResourceEventComp.resourceQuantity, 0, (int)resourceComp.resourceQuantity);
            int connIdx = connectionDB.GetConnectionIndex(sendResourceEventComp.fromEntity, sendResourceEventComp.toEntity);
            
            var transferableResourceComp = new TransferableResourceComp
            {
                unitEntity = occupiedNodeComp.unitEntity,
                endNodeEntity = sendResourceEventComp.toEntity,
                quantity = sendResourceEventComp.resourceQuantity,
                connectionIndex = connIdx,
            };
            
            resourceEntity.AddComponent(transferableResourceComp);
            resourceEntity.GetComponent<TextComp>().txt.text = transferableResourceComp.quantity.ToString();
            connectionDB[transferableResourceComp.connectionIndex].AddResourceTransfer(resourceEntity, ref transferableResourceComp);
            
            var resourceEntityRenderer = resourceEntity.GetComponent<TransformComp>().transform.GetComponent<SpriteRenderer>();
            resourceEntityRenderer.color = UnitIdLookup.GetColor(occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType);
            resourceComp.resourceQuantity -= sendResourceEventComp.resourceQuantity;

            if (sendResourceEventComp.fromEntity.HasTween() == false)
            {
                ref var scaleComp = ref sendResourceEventComp.fromEntity.GetComponent<ScaleComp>();
                var scale = scaleComp.scale.ToVector3();
                sendResourceEventComp.fromEntity.XIVTween()
                    .Scale(scale, scale * 1.1f, 0.5f, EasingFunction.EaseOutCubic, true)
                    .Start();
            }
        }

        void SendResourceContinuously(Entity nodeEntity, ref ResourceComp resourceComp, ref SendResourceContinuouslyComp sendResourceContinuouslyComp)
        {
            if (sendResourceContinuouslyComp.sendTimer.Update(XTime.deltaTime) == false) return;
            sendResourceContinuouslyComp.sendTimer.Restart();
            // we don't have resource to send
            if ((int)resourceComp.resourceQuantity == 0) return;
            
            world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromEntity = nodeEntity,
                toEntity = sendResourceContinuouslyComp.toEntity,
                resourceQuantity = (int)resourceComp.resourceQuantity,
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

        Entity GetResource(Vec3 position)
        {
            Entity entity;
            if (resourcePool.Count == 0)
            {
                entity = GameObjectEntity.CreateEntity(world, assetReferences.resourceEntity, position.ToVector3(), Quaternion.identity);
            }
            else
            {
                var go = resourcePool.Dequeue();
                go.transform.position = position;
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