using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class SelectionFsmManager
    {
        readonly ConnectionDB connectionDB;
        readonly AssetReferences assetReferences;
        public SelectionState currentState;
        public Entity first;
        public Entity second;
        public SwipeDetector swipeDetector = SwipeDetector.New();
        Dictionary<Type, SelectionState> states = new();

        public SelectionFsmManager(ConnectionDB connectionDB, AssetReferences assetReferences)
        {
            this.connectionDB = connectionDB;
            this.assetReferences = assetReferences;
            ChangeState<SelectionStateIdle>();
        }

        public bool AddState<T>(T state) where T: SelectionState => states.TryAdd(typeof(T), state);
        public T GetState<T>() where T : SelectionState
        {
            var t = typeof(T);
            if (states.TryGetValue(t, out var state) == false)
            {
                state = (T)Activator.CreateInstance(t, this);
                states.Add(t, state);
            }
            return (T)state;
        }

        public void ChangeState<T>() where T: SelectionState
        {
            if (currentState != null) currentState.BeforeStateChange();
            T newState = GetState<T>();
            currentState = newState;
            currentState.Start();
        }

        public void Run(ref InputData input)
        {
            if (currentState == null) return;
            currentState.Update(ref input, swipeDetector.DetectSwipe(ref input, XTime.unscaledDeltaTime));
        }

        public bool TryGetFirstFromInput(ref InputData input, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            if (TryGetEntityFromInput(ref input, out entity) == false) return false;
            return entity.HasComponent<OccupiedNodeComp>() && entity.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().unitType == UnitIdLookup.UnitType.Green;
        }

        public bool TryGetSecondFromInput(ref InputData input, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            // Second requires a connection to first
            if (TryGetEntityFromInput(ref input, out entity) == false) return false;
            return connectionDB.IsConnected(first, entity);
        }

        public bool TryGetEntityFromInput(ref InputData input, out Entity entity)
        {
            using var hits = ArrayUtils.GetBuffer<RaycastHit>(1);
            int hitCount = Physics.RaycastNonAlloc(input.InputRay, hits, 100f, 1 << PhysicsConstants.NodeLayer);
            if (hitCount > 0)
            {
                entity = hits[0].collider.XIVGetEntity();
                return entity.IsAlive() && entity.HasComponent<NodeComp>();
            }

            entity = Entity.Invalid;
            return false;
        }
        
        public Entity GetPossibleTarget(Vector2 swipeDirection)
        {
            if (first.IsAlive() == false) return Entity.Invalid;
            
            var dotProduct = 0f;
            var firstNodeEntityTransformPosition = first.GetComponent<TransformComp>().transform.position;
            Entity closestEntity = Entity.Invalid;
            using var indexBuffer = ArrayUtils.GetBuffer<int>(connectionDB.Count);
            int len = connectionDB.GetAllConnectionPairs(first, indexBuffer);
            for (var i = 0; i < len; i++)
            {
                ref var pair = ref connectionDB[indexBuffer[i]];
                var connectedEntity = pair.GetOpposite(first);
                var connectedEntityPos = connectedEntity.GetComponent<TransformComp>().transform.position;
                var dirToConnected = (Vector2)(connectedEntityPos - firstNodeEntityTransformPosition);
                // Use dot product to define the possible target direction
                var dot = Vector2.Dot(swipeDirection.normalized, dirToConnected.normalized);
                if (dotProduct < dot)
                {
                    dotProduct = dot;
                    closestEntity = connectedEntity;
                }
            }

            return closestEntity;
        }

        public void Highlight(Entity selectedEntity, bool v)
        {
            if (v) selectedEntity.world.NewEntity().AddComponent(new EnableHighlightEventComp { targetEntity = selectedEntity });
            else selectedEntity.world.NewEntity().AddComponent(new DisableHighlightEventComp { targetEntity = selectedEntity });
        }
        

        public void TransferOnce(bool allResource = true)
        {
            if (first.IsAlive() == false || second.IsAlive() == false || connectionDB.IsConnected(first, second) == false) return;
            ref var resourceComp = ref first.GetComponent<ResourceComp>();
            var total = resourceComp.resourceQuantity;
            var send = (int)(allResource ? total : total * 0.5f);
            if (send == 0) return;
            resourceComp.resourceQuantity -= send;
            
            first.world.NewEntity().AddComponent(new SendResourceEventComp
            {
                fromUnitEntity = first.GetComponent<OccupiedNodeComp>().unitEntity,
                fromEntity = first,
                toEntity = second,
                resourceQuantity = send,
            });
        }

        public void StartContinuousTransfer()
        {
            if (first.IsAlive() == false || second.IsAlive() == false || connectionDB.IsConnected(first, second) == false) return;
            
            first.world.NewEntity().AddComponent(new StartContinuousResourceTransferEventComp
            {
                fromUnitEntity = first.GetComponent<OccupiedNodeComp>().unitEntity,
                fromEntity = first,
                targetEntity = second,
            });
        }

        public void StopContinuousTransfer()
        {
            first.RemoveComponent<SendResourceContinuouslyComp>();
        }
    }
}