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
        readonly PrefabReferences prefabReferences;
        public SelectionState currentState;
        public Entity first;
        public Entity second;
        public SwipeDetector swipeDetector = SwipeDetector.New();
        Dictionary<Type, SelectionState> states = new();

        public SelectionFsmManager(ConnectionDB connectionDB, PrefabReferences prefabReferences)
        {
            this.connectionDB = connectionDB;
            this.prefabReferences = prefabReferences;
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
            currentState.Update(ref input, swipeDetector.DetectSwipe(ref input, XTime.deltaTime));
        }

        public bool TryGetFirstFromInput(ref InputData input, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            if (TryGetEntityFromInput(ref input, out entity) == false) return false;
            return entity.HasComponent<OccupiedNodeComp>() && entity.GetComponent<NodeComp>().unitType == UnitIdLookup.UnitType.Green;
        }

        public bool TryGetSecondFromInput(ref InputData input, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            // Second requires a connection to first
            if (TryGetEntityFromInput(ref input, out entity) == false) return false;
            return IsConnected(first, entity);
        }
        
        bool IsConnected(Entity ent1, Entity ent2)
        {
            return connectionDB.IsConnected(ent1, ent2);
        }

        public bool TryGetEntityFromInput(ref InputData input, out Entity entity)
        {
            using var disposable = ArrayUtils.GetBuffer(out RaycastHit[] hits, 4);
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
            using var dispose = ArrayUtils.GetBuffer(out ConnectionPair[] pairBuffer, connectionDB.Count);
            int len = connectionDB.GetPairs(first, pairBuffer);
            for (var i = 0; i < len; i++)
            {
                ref var pair = ref pairBuffer[i];
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
            if (v) selectedEntity.AddTag<EnableHighlightTag>();
            else selectedEntity.AddTag<DisableHighlightTag>();
        }
        

        public void TransferOnce(bool allResource = true)
        {
            if (first.IsAlive() == false || second.IsAlive() == false || IsConnected(first, second) == false) return;
            var total = (int)first.GetComponent<NodeComp>().resourceQuantity;
            var send = allResource ? total : total * 0.5f;
            
            first.AddComponent(new SendResourceComp
            {
                toEntity = second,
                resourceQuantity = (int)send,
            });
        }

        public void StartContinuousTransfer()
        {
            if (first.IsAlive() == false || second.IsAlive() == false || IsConnected(first, second) == false) return;
            
            first.AddComponent(new SendResourceContinuouslyComp
            {
                toEntity = second,
                duration = prefabReferences.generationConfigs[0].duration,
                currentDuration = 0f
            });
        }

        public void StopContinuousTransfer()
        {
            first.RemoveComponent<SendResourceContinuouslyComp>();
        }
    }
}