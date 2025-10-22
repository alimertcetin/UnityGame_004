using System.Buffers;
using System.Collections.Generic;
using TheGame.Extensions;
using UnityEngine;
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
        public Entity targetNodeEntity;
        public int quantity;
    }
    public class ResourceTransferSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, ResourceComp> resourceFilter = null;
        readonly Filter<TransformComp, NodeComp, NodeResourceCollisionComp> nodeResourceCollisionFilter = null;
        readonly Filter<TransformComp, NodeComp, OccupiedNodeComp, SendResourceComp> sendResourceFilter = null;
        readonly Filter<NodeComp, OccupiedNodeComp, SendResourceContinuouslyComp> sendResourceContinuouslyFilter = null;
        readonly Filter<LineRendererComp> lineRendererFilter = null;
        readonly PrefabReferences prefabReferences = null;
        readonly ConnectionDB connectionDB = null;
        readonly Queue<GameObject> resourcePool = new Queue<GameObject>();

        public override void Update()
        {
            nodeResourceCollisionFilter.ForEach(HandleResourceCollision);
            resourceFilter.ForEach(HandleResourceMovement);
            sendResourceFilter.ForEach(SendResource);
            sendResourceContinuouslyFilter.ForEach(SendResourceContinuously);
            lineRendererFilter.ForEach(FixLineRenderers);
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
                nodeComp.txt_quantity.text = ((int)nodeComp.resourceQuantity).ToString();
            }

            transformComp.transform.CancelTween();
            transformComp.transform.XIVTween()
                .ScaleBounceOnce()
                .Start();
            nodeEntity.RemoveComponent<NodeResourceCollisionComp>();
        }

        void HandleResourceMovement(Entity resourceEntity, ref TransformComp transformComp, ref ResourceComp resourceComp)
        {
            var pos = transformComp.transform.position;
            var targetEntity = resourceComp.targetNodeEntity;
            var startEntity = resourceComp.startNodeEntity;
            var startTransform = startEntity.GetTransform();
            var startTransformPosition = startTransform.position;
            var targetTransform = targetEntity.GetComponent<TransformComp>().transform;
            var targetTransformPosition = targetTransform.position;
            pos = Vector3.MoveTowards(pos, targetTransformPosition, 2f * XTime.deltaTime);
            transformComp.transform.position = pos;

            ConnectionDB.Pair pair = connectionDB.GetPair(startEntity, targetEntity);
            HandleLineRendererVisual(pair.lineRendererEntity, targetTransformPosition - startTransformPosition, pos);

            if (Vector3.Distance(pos, targetTransformPosition) < 0.02f == false) return;
                
            // lineRenderer.XIVStraightLine(startTransformPosition, targetTransformPosition);
                
            targetEntity.AddComponent(new NodeResourceCollisionComp
            {
                unitEntity = resourceComp.unitEntity,
                quantity = resourceComp.quantity,
            });
                
            resourceEntity.RemoveComponent<ResourceComp>();
            resourceEntity.AddComponent(new CallLaterComp
            {
                action = ReleaseResource,
                timer = 0.15f,
            });
        }

        void SendResource(Entity entity, ref TransformComp transformComp, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref SendResourceComp sendResourceComp)
        {
            var transform = transformComp.transform;
            var transformPosition = transform.position;
            var transformRotation = transform.rotation;
            var resourceEntity = GetResource(transformPosition, transformRotation);
#if UNITY_EDITOR
            var pair = connectionDB.GetPair(entity, sendResourceComp.toNode);
            if (pair.entity1.IsAlive() == false || pair.entity2.IsAlive() == false)
            {
                Camera.main.transform.position = transformPosition.SetZ(-10);
                var t1 = entity.GetTransform();
                var t2 = sendResourceComp.toNode.GetTransform();
                t1.position = t1.position.SetZ(-10);
                t2.position = t2.position.SetZ(-10);
                Debug.Break();
            }
#endif
            
            var resourceComp = new ResourceComp
            {
                unitEntity = occupiedNodeComp.unitEntity,
                startNodeEntity = entity,
                targetNodeEntity = sendResourceComp.toNode,
                quantity = sendResourceComp.resourceQuantity,
            };
            resourceEntity.AddComponent(resourceComp);
            resourceEntity.GetComponent<TextComp>().txt.text = resourceComp.quantity.ToString();
            nodeComp.resourceQuantity -= sendResourceComp.resourceQuantity;
            entity.RemoveComponent<SendResourceComp>();
        }

        void FixLineRenderers(Entity nodeEntity, ref LineRendererComp lineRendererComp)
        {
            // slowly fix line renderers
            using var dispose = ArrayUtils.GetBuffer(out ConnectionDB.Pair[] pairBuffer, connectionDB.Count);
            
            var lineRenderer = lineRendererComp.lineRenderer;
            var positionCount = lineRendererComp.positions.Length;
            var startPos = lineRenderer.GetPosition(0);
            var endPos = lineRenderer.GetPosition(positionCount - 1);
            
            for (int i = 0; i < positionCount; i++)
            {
                var t = (float)i / positionCount;
                var targetPos = Vector3.Lerp(startPos, endPos, t);
                var currentPos = lineRendererComp.positions[i];
                var newPos = Vector3.MoveTowards(currentPos, targetPos, 0.01f * XTime.deltaTime);
                lineRendererComp.positions[i] = newPos;
            }
            lineRenderer.SetPositions(lineRendererComp.positions);
        }

        void SendResourceContinuously(Entity nodeEntity, ref NodeComp nodeComp, ref OccupiedNodeComp comp1, ref SendResourceContinuouslyComp sendResourceContinuouslyComp)
        {
            sendResourceContinuouslyComp.currentDuration -= XTime.deltaTime;
            if (sendResourceContinuouslyComp.currentDuration > 0) return;
            // a little trick for visual improvement
            if ((int)nodeComp.resourceQuantity == 0)
            {
                sendResourceContinuouslyComp.currentDuration = sendResourceContinuouslyComp.duration;
                return;
            }
            
            var pair = connectionDB.GetPair(nodeEntity, sendResourceContinuouslyComp.toNode);
            if (pair.entity1.IsAlive() == false || pair.entity2.IsAlive() == false)
            {
                Debug.Log("SendResourceContinuously");
                var t1 = pair.entity1.GetTransform();
                var t2 = pair.entity2.GetTransform();
                t1.position = t1.position.SetZ(-10);
                t2.position = t2.position.SetZ(-10);
                Debug.Break();
            }

            sendResourceContinuouslyComp.currentDuration = sendResourceContinuouslyComp.duration;
            nodeEntity.AddComponent(new SendResourceComp
            {
                resourceQuantity = (int)nodeComp.resourceQuantity,
                toNode = sendResourceContinuouslyComp.toNode,
            });
        }

        void HandleLineRendererVisual(Entity lineRendererEntity, Vector3 movementDirection, Vector3 position)
        {
            ref var lineRendererComp = ref lineRendererEntity.GetComponent<LineRendererComp>();
            
            int pointCount = lineRendererComp.positions.Length;
            const float stepOffset = 0.3f;
            const float scale = 0.075f;
            const float frequency = 0.75f;

            Vector3 lineStart = lineRendererComp.lineRenderer.GetPosition(0);
            Vector3 lineEnd = lineRendererComp.lineRenderer.GetPosition(pointCount - 1);
            Vector3 direction = (lineEnd - lineStart).normalized;

            float totalDistance = Vector3.Distance(lineStart, lineEnd);
            if (totalDistance < Mathf.Epsilon) return;

            // Project position onto the line segment (direction-agnostic)
            Vector3 lineVector = lineEnd - lineStart;
            Vector3 toPosition = position - lineStart;
            float projectedLength = Vector3.Dot(toPosition, lineVector.normalized);
            float projectedT = projectedLength / lineVector.magnitude;
            projectedT = Mathf.Clamp01(projectedT); // Clamp to valid range

            // int affectedIndex = Mathf.Clamp((int)(projectedT * (pointCount - 1)), 1, pointCount - 2);

            Vector3 normal = Vector3.Cross(Vector3.forward, direction); // Perpendicular in XY

            for (int i = 1; i < pointCount - 1; i++)
            {
                float tt = (float)(i - 1) / pointCount;
                Vector3 basePos = Vector3.LerpUnclamped(lineStart, lineEnd, tt);
                var d = Vector3.Dot(movementDirection, position - basePos);
                if (d < 0.5f) continue;
                
                // Distortion falloff based on distance from projection
                float dist = projectedT - tt;
                float weight = XIVMathf.Clamp01(1f - XIVMathf.Abs(dist) * 4f); // Falloff multiplier controls width

                if (weight <= 0f) continue;

                float sin = XIVMathf.Sin(dist + stepOffset + (frequency * i)) * scale;
                basePos += normal * (sin * weight);

                lineRendererComp.positions[i] = basePos;
            }
        }

        Entity GetResource(Vector3 transformPosition, Quaternion transformRotation)
        {
            if (resourcePool.Count == 0) return GameObjectEntity.CreateEntity(world, prefabReferences.resourceEntity, transformPosition, transformRotation);
            var go = resourcePool.Dequeue();
            go.transform.position = transformPosition;
            go.transform.rotation = transformRotation;
            go.SetActive(true);
            return GameObjectEntity.BindGameObjectToEntityWithDependencies(world, go.GetComponent<GameObjectEntity>());
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