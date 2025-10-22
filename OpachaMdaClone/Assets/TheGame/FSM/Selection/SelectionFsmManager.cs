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
        public SelectionState currentState;
        public Entity first;
        public Entity second;
        public SwipeDetector swipeDetector = SwipeDetector.New();
        Dictionary<Type, SelectionState> states = new();

        public SelectionFsmManager(ConnectionDB connectionDB)
        {
            this.connectionDB = connectionDB;
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
            return entity.HasComponent<OccupiedNodeComp>();
        }

        public bool TryGetSecondFromInput(ref InputData input, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            // Besides second requires a connection to first
            if (TryGetEntityFromInput(ref input, out entity) == false) return false;
            return connectionDB.IsConnected(first, entity);
        }

        public bool TryGetEntityFromInput(ref InputData input, out Entity entity)
        {
            using var disposable = ArrayUtils.GetBuffer(out RaycastHit[] hits, 4);
            int hitCount = Physics.RaycastNonAlloc(input.InputRay, hits, 100f, 1 << PhysicsConstants.NodeLayer);
            if (hitCount > 0)
            {
                entity = hits[0].collider.XIVGetEntity();
                return entity.IsAlive();
            }

            entity = Entity.Invalid;
            return false;
        }
        
        public Entity GetPossibleTarget(Vector2 swipeDirection)
        {
            if (first.IsAlive() == false) return Entity.Invalid;
            
            var dotProduct = 0f;
            using var entityBufferDispose = ArrayUtils.GetBuffer(out ConnectionDB.Pair[] pairBuffer, connectionDB.Count);
            int connectionCount = connectionDB.GetPairs(first, pairBuffer);
            var selectedNodeEntityPosition = first.GetTransform().position;
            Entity closestEntity = Entity.Invalid;
            for (int i = 0; i < connectionCount; i++)
            {
                var connectedEntity = pairBuffer[i].GetOpposite(first);
                var connectedEntityPos = connectedEntity.GetTransform().position;
                var dirToConnected = (Vector2)(connectedEntityPos - selectedNodeEntityPosition);
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
            if (first.IsAlive() == false || second.IsAlive() == false || connectionDB.IsConnected(first, second) == false) return;
            var total = (int)first.GetComponent<NodeComp>().resourceQuantity;
            var send = allResource ? total : total / 2f;

            var pair = connectionDB.GetPair(first, second);
            if (pair.entity1.IsAlive() == false || pair.entity2.IsAlive() == false)
            {
                Debug.Log("TransferOnce");
                var t1 = pair.entity1.GetTransform();
                var t2 = pair.entity2.GetTransform();
                t1.position = t1.position.SetZ(-10);
                t2.position = t2.position.SetZ(-10);
                Debug.Break();
            }
            
            first.AddComponent(new SendResourceComp
            {
                toNode = second,
                resourceQuantity = (int)send,
            });
        }

        public void StartContinuousTransfer()
        {
            if (first.IsAlive() == false || second.IsAlive() == false || connectionDB.IsConnected(first, second) == false) return;
            first.AddComponent(new SendResourceContinuouslyComp
            {
                toNode = second,
                duration = first.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().configs[0].duration,
                currentDuration = 0f
            });
        }

        public void StopContinuousTransfer()
        {
            first.RemoveComponent<SendResourceContinuouslyComp>();
        }
    }
}