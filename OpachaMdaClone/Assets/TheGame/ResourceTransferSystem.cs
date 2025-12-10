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
        public Entity startNodeEntity;
        public Entity endNodeEntity;
        public Vec3 startPos;
        public Vec3 endPos;
        public Vec3 movementDirection; // Not normalized, endPos - startPos
        // TODO: Integrate with ResourceComp
        public int quantity;
        public int connectionIndex;
        
        public byte localId; // id on the connection, not the index
        public int collisionMask;
    }

    public struct PooledComp : IComponent
    {
        public Action<Entity> releaseToPoolAction;
    }

    public struct SendResourceContinuouslyComp : IComponent
    {
        public Entity toEntity;
    }

    public struct StartContinuousResourceTransferEventComp : IComponent
    {
        public Entity fromUnitEntity;
        public Entity fromEntity;
        public Entity targetEntity;
    }
    
    public struct SendResourceEventComp : IComponent
    {
        public Entity fromUnitEntity;
        public Entity fromEntity;
        public Entity toEntity;
        public int resourceQuantity;
    }

    public class ResourceTransferSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, TransferableResourceComp> transferableResourceFilter = new Filter<PositionComp, TransferableResourceComp>().ExcludeTag<ReturnToPoolTag>();
        readonly Filter<SendResourceEventComp> sendResourceFilter = null;
        readonly Filter<ResourceComp, OccupiedNodeComp, SendResourceContinuouslyComp> sendResourceContinuouslyFilter = null;
        readonly Filter<StartContinuousResourceTransferEventComp> startContinuousResourceTransferFilter = null;
        readonly Filter<TransferableResourceComp, PooledComp> pooledResourceFilter = new  Filter<TransferableResourceComp, PooledComp>().Tag<ReturnToPoolTag>();
        
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
            pooledResourceFilter.ForEach(ReturnToPool);
            startContinuousResourceTransferFilter.ForEach(StartContinuousResourceTransfer);
            transferableResourceFilter.ForEach(MoveResourceAlongLine);
            sendResourceFilter.ForEach(SendResource);
            sendResourceContinuouslyFilter.ForEach(SendResourceContinuously);
        }

        void ReturnToPool(Entity entity, ref TransferableResourceComp transferableResourceComp, ref PooledComp pooledComp)
        {
            connectionDB[transferableResourceComp.connectionIndex].RemoveResourceTransfer(entity, ref transferableResourceComp);
            pooledComp.releaseToPoolAction(entity);
        }

        void StartContinuousResourceTransfer(Entity entity, ref StartContinuousResourceTransferEventComp startContinuousResourceTransferComp)
        {
            entity.Destroy();
            if (startContinuousResourceTransferComp.fromEntity.HasComponent<SendResourceContinuouslyComp>()) return;
            // this node is occupied after event fired
            if (startContinuousResourceTransferComp.fromEntity.GetComponent<OccupiedNodeComp>().unitEntity != startContinuousResourceTransferComp.fromUnitEntity) return;
            
            ref var resourceComp = ref startContinuousResourceTransferComp.fromEntity.GetComponent<ResourceComp>();
            var quantityToSend = (int)resourceComp.resourceQuantity;
            if (quantityToSend == 0) return;
            
            resourceComp.resourceQuantity -= quantityToSend;
            // send immediately
            world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromUnitEntity = startContinuousResourceTransferComp.fromUnitEntity,
                fromEntity = startContinuousResourceTransferComp.fromEntity,
                toEntity = startContinuousResourceTransferComp.targetEntity,
                resourceQuantity = quantityToSend,
            });
            startContinuousResourceTransferComp.fromEntity.AddComponent(new SendResourceContinuouslyComp
            {
                toEntity = startContinuousResourceTransferComp.targetEntity,
            });
        }

        void MoveResourceAlongLine(Entity resourceEntity, ref PositionComp positionComp, ref TransferableResourceComp transferableResourceComp)
        {
            int connectionIndex = transferableResourceComp.connectionIndex;
            var nextPos = Vec3.MoveTowards(positionComp.position, transferableResourceComp.endPos, GameConstants.RESOURCE_MOVEMENT_SPEED * XTime.deltaTime);
            lineRendererPositionData.connectionIndices.Add() = connectionIndex;
            lineRendererPositionData.movementPositions.Add() = nextPos;
            lineRendererPositionData.movementDirections.Add() = transferableResourceComp.movementDirection;

            // Check collision
            ref var connectionPair = ref connectionDB[connectionIndex];
            const float step = GameConstants.RESOURCE_RADIUS * 0.5f;
            var resourceEntitiesOnConnection = connectionPair.resourceEntitiesOnConnection;
            int count = resourceEntitiesOnConnection.Count;
            bool isCollided = false;
            for (int i = 0; i < count && isCollided == false; i++)
            {
                ref var otherResourceEntity = ref resourceEntitiesOnConnection[i];
                if (otherResourceEntity == resourceEntity) continue;
                ref var otherTransferableResourceComp = ref otherResourceEntity.GetComponent<TransferableResourceComp>();
                bool isCollidedBefore = (transferableResourceComp.collisionMask & (1 << otherTransferableResourceComp.localId)) != 0;
                if (otherTransferableResourceComp.unitEntity == transferableResourceComp.unitEntity || isCollidedBefore) continue;
                
                Vec3 p1 = positionComp.position;
                Vec3 p3 = otherResourceEntity.GetComponent<PositionComp>().position;

                do
                {
                    if (Vec3.Distance(p1, p3) > GameConstants.RESOURCE_RADIUS)
                    {
                        // No collision
                        p1 = Vec3.MoveTowards(p1, nextPos, step);
                    }
                    else
                    {
                        isCollided = true;
                        transferableResourceComp.collisionMask |= 1 << otherTransferableResourceComp.localId;
                        otherTransferableResourceComp.collisionMask |= 1 << transferableResourceComp.localId;
                        nextPos = p1;
                        world.NewEntity().AddComponent(new ResourceTransferCollisionEventComp
                        {
                            resourceEntity1 = resourceEntity,
                            resourceEntity2 = otherResourceEntity,
                        });
                    }
                } while (Vec3.Distance(p1, nextPos) > step);
            }
            
            positionComp.Set(nextPos);
            
            if (isCollided || Vec3.Distance(nextPos, transferableResourceComp.endPos) < 0.02f == false) return;
            
            // Reached the target position
            world.NewEntity().AddComponent(new NodeResourceCollisionEventComp
            {
                senderEntity = connectionPair.GetOpposite(transferableResourceComp.endNodeEntity),
                receiver = transferableResourceComp.endNodeEntity,
                senderUnitEntity = transferableResourceComp.unitEntity,
                quantity = transferableResourceComp.quantity,
            });
            resourceEntity.AddTag<ReturnToPoolTag>();
        }

        void SendResource(Entity entity, ref SendResourceEventComp sendResourceEventComp)
        {
            entity.Destroy();
            // unit has been changed
            ref var occupiedNodeComp = ref sendResourceEventComp.fromEntity.GetComponent<OccupiedNodeComp>();
            if (occupiedNodeComp.unitEntity != sendResourceEventComp.fromUnitEntity) return;
            ref var positionComp = ref sendResourceEventComp.fromEntity.GetComponent<PositionComp>();
            
            var resourceEntity = GetResource(positionComp.position);
            int connIdx = connectionDB.GetConnectionIndex(sendResourceEventComp.fromEntity, sendResourceEventComp.toEntity);
            ref var connectionPair = ref connectionDB[connIdx];
            Vec3 startPos = sendResourceEventComp.fromEntity == connectionPair.entity1 ? connectionPair.startPosition : connectionPair.endPosition;
            Vec3 endPos = sendResourceEventComp.toEntity == connectionPair.entity2 ? connectionPair.endPosition : connectionPair.startPosition;
            
            var transferableResourceComp = new TransferableResourceComp
            {
                unitEntity = occupiedNodeComp.unitEntity,
                startNodeEntity = sendResourceEventComp.fromEntity,
                endNodeEntity = sendResourceEventComp.toEntity,
                startPos = startPos,
                endPos = endPos,
                movementDirection = endPos - startPos,
                quantity = sendResourceEventComp.resourceQuantity,
                connectionIndex = connIdx,
            };
            
            connectionPair.AddResourceTransfer(resourceEntity, ref transferableResourceComp);
            resourceEntity.AddComponent(transferableResourceComp);
            resourceEntity.GetComponent<TextComp>().txt.WriteScoreText(transferableResourceComp.quantity);
            
            ref var instancedRendererComp = ref resourceEntity.GetComponent<InstancedRendererComp>();
            instancedRendererComp.renderer.GetPropertyBlock(instancedRendererComp.materialPropertyBlock);
            instancedRendererComp.materialPropertyBlock.SetColor(ShaderConstants.Custom_SpriteWithShadow_Instanced.Color_ColorID, UnitIdLookup.GetColor(occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType));
            instancedRendererComp.renderer.SetPropertyBlock(instancedRendererComp.materialPropertyBlock);

            ref var scaleComp = ref sendResourceEventComp.fromEntity.GetComponent<ScaleComp>();
            var scale = scaleComp.scale.ToVector3();
            sendResourceEventComp.fromEntity.CancelTween();
            sendResourceEventComp.fromEntity.XIVTween()
                .Scale(scale, scale * 1.1f, 0.5f, EasingFunction.EaseOutCubic, true)
                .UseCustomDeltaTime(() => XTime.deltaTime)
                .Start();
        }

        void SendResourceContinuously(Entity nodeEntity, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp, ref SendResourceContinuouslyComp sendResourceContinuouslyComp)
        {
            ref var unitComp = ref occupiedNodeComp.unitEntity.GetComponent<UnitComp>();
            if (unitComp.resourceTransferTimer.IsDone == false) return;
            // we don't have resource to send
            var quantityToSend = (int)resourceComp.resourceQuantity;
            if (quantityToSend == 0) return;
            resourceComp.resourceQuantity -= quantityToSend;
            
            world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromUnitEntity = occupiedNodeComp.unitEntity,
                fromEntity = nodeEntity,
                toEntity = sendResourceContinuouslyComp.toEntity,
                resourceQuantity = quantityToSend,
            });
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