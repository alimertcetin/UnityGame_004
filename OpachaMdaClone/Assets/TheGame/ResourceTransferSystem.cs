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
    public struct ResourceComp : IComponent
    {
        public Entity unitEntity;
        public Entity startNodeEntity;
        public Entity endNodeEntity;
        public Vector3 resourcePosition;
        public int quantity;
    }

    public struct PooledComp : IComponent
    {
        public Func<Vector3, Quaternion, Entity> getFromPoolAction;
        public Action<Entity> releaseToPoolAction;
    }

    public class ResourceTransferSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, ResourceComp> resourceFilter = null;
        readonly Filter<TransformComp, NodeComp, NodeResourceCollisionComp> nodeResourceCollisionFilter = null;
        readonly Filter<TransformComp, NodeComp, OccupiedNodeComp, SendResourceComp> sendResourceFilter = null;
        readonly Filter<NodeComp, OccupiedNodeComp, SendResourceContinuouslyComp> sendResourceContinuouslyFilter = null;
        readonly PrefabReferences prefabReferences = null;
        readonly Queue<GameObject> resourcePool = new Queue<GameObject>();
        readonly ConnectionDB connectionDB = null;
        readonly LineRendererPositionData lineRendererPositionData = null;
        
        readonly Action<Entity> releaseResourceAction;
        readonly Func<Vector3, Quaternion, Entity> getResourceAction;
        
        public ResourceTransferSystem() : base()
        {
            getResourceAction = GetResource;
            releaseResourceAction = ReleaseResource;
        }

        public override void Start()
        {
            const int PREWARM_COUNT = 250;
            using var dispose = ArrayUtils.GetBuffer(out Entity[] entityBuffer, PREWARM_COUNT);
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
            nodeResourceCollisionFilter.ForEach(HandleResourceCollision);
            resourceFilter.ForEach(MoveResourceAlongLine);
            sendResourceFilter.ForEach(SendResource);
            sendResourceContinuouslyFilter.ForEach(SendResourceContinuously);
        }

        void HandleResourceCollision(Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp, ref NodeResourceCollisionComp nodeResourceCollisionComp)
        {
            var unitEntity = nodeResourceCollisionComp.unitEntity;
            ref var unitComp = ref unitEntity.GetComponent<UnitComp>();
            if (nodeComp.unitType == unitComp.unitType)
            {
                nodeComp.resourceQuantity += nodeResourceCollisionComp.quantity;
            }
            else
            {
                var remaining = nodeComp.shieldPoints - nodeResourceCollisionComp.quantity;
                nodeComp.shieldPoints = XIVMathf.Max(nodeComp.shieldPoints - nodeResourceCollisionComp.quantity, 0);
                if (remaining >= 0) return;
                remaining = -remaining;
                    
                nodeComp.resourceQuantity -= remaining;
                if (nodeComp.resourceQuantity <= 0)
                {
                    nodeComp.unitType = unitComp.unitType;
                    nodeComp.resourceQuantity = -nodeComp.resourceQuantity;
                    nodeEntity.AddComponent(new NodeOccupyComp
                    {
                        unitEntity = unitEntity,
                    });
                }
                nodeComp.txt_quantity.WriteScoreText((int)nodeComp.resourceQuantity);
            }

            transformComp.transform.CancelTween();
            transformComp.transform.XIVTween()
                .ScaleBounceOnce()
                .Start();
            nodeEntity.RemoveComponent<NodeResourceCollisionComp>();
        }

        void MoveResourceAlongLine(Entity resourceEntity, ref TransformComp transformComp, ref ResourceComp resourceComp)
        {
            int idx = connectionDB.GetConnectionIndex(resourceComp.startNodeEntity, resourceComp.endNodeEntity);
            ref ConnectionPair connectionPairComp = ref connectionDB[idx];
            
            GetStartAndTargetPositions(ref resourceComp, ref connectionPairComp, out var startTransformPosition, out var targetTransformPosition);
            resourceComp.resourcePosition = Vector3.MoveTowards(resourceComp.resourcePosition, targetTransformPosition, 2f * XTime.deltaTime);
            var movementDirection = targetTransformPosition - startTransformPosition;
            
            lineRendererPositionData.connectionIndices.Add() = idx;
            lineRendererPositionData.movementPositions.Add() = resourceComp.resourcePosition;
            lineRendererPositionData.movementDirections.Add() = movementDirection;
            transformComp.transform.position = resourceComp.resourcePosition;

            if (Vector3.Distance(resourceComp.resourcePosition, targetTransformPosition) < 0.02f == false) return;
            
            // lineRenderer.XIVStraightLine(startTransformPosition, targetTransformPosition);
                
            connectionPairComp.GetOpposite(resourceComp.startNodeEntity).AddComponent(new NodeResourceCollisionComp
            {
                unitEntity = resourceComp.unitEntity,
                quantity = resourceComp.quantity,
            });
                
            resourceEntity.RemoveComponent<ResourceComp>();
            resourceEntity.AddComponent(new CallLaterComp
            {
                action = releaseResourceAction,
                timer = 0.1f,
            });
        }

        void SendResource(Entity entity, ref TransformComp transformComp, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref SendResourceComp sendResourceComp)
        {
            var transform = transformComp.transform;
            var transformPosition = transform.position;
            var transformRotation = transform.rotation;
            var resourceEntity = GetResource(transformPosition, transformRotation);
            // if we process collision before sending the resource it may get negative
            sendResourceComp.resourceQuantity = XIVMathInt.Clamp(sendResourceComp.resourceQuantity, 0, (int)nodeComp.resourceQuantity);
            
            var resourceComp = new ResourceComp
            {
                unitEntity = occupiedNodeComp.unitEntity,
                startNodeEntity = entity,
                endNodeEntity = sendResourceComp.toEntity,
                quantity = sendResourceComp.resourceQuantity,
                resourcePosition = transformPosition,
            };
            
            resourceEntity.AddComponent(resourceComp);
            resourceEntity.GetComponent<TextComp>().txt.text = resourceComp.quantity.ToString();
            
            var resourceEntityRenderer = resourceEntity.GetComponent<TransformComp>().transform.GetComponent<SpriteRenderer>();
            resourceEntityRenderer.color = UnitIdLookup.GetColor(nodeComp.unitType);
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

        static void GetStartAndTargetPositions(ref ResourceComp resourceComp, ref ConnectionPair connectionPairComp, out Vector3 startTransformPosition, out Vector3 targetTransformPosition)
        {
            if (resourceComp.startNodeEntity == connectionPairComp.entity1)
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
                entity = GameObjectEntity.CreateEntity(world, prefabReferences.resourceEntity, transformPosition, transformRotation);
            }
            else
            {
                var go = resourcePool.Dequeue();
                go.transform.position = transformPosition;
                go.transform.rotation = transformRotation;
                go.SetActive(true);
                entity = GameObjectEntity.BindGameObjectToEntityWithDependencies(world, go.GetComponent<GameObjectEntity>());
            }
            entity.AddComponent(new PooledComp
            {
                getFromPoolAction = getResourceAction,
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