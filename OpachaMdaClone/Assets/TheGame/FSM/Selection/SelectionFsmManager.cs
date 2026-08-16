using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.DataStructures;
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

        public void Run(ref SingleInputData singleInput)
        {
            if (currentState == null) return;
            currentState.Update(ref singleInput, swipeDetector.DetectSwipe(ref singleInput, XTime.unscaledDeltaTime));
        }

        public bool TryGetFirstFromInput(ref SingleInputData singleInput, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            if (TryGetEntityFromInput(ref singleInput, out entity) == false) return false;
            return entity.HasComponent<OccupiedNodeComp>() && entity.GetComponent<OccupiedNodeComp>().unitEntity.GetComponent<UnitComp>().unitType == UnitIdLookup.UnitType.Green;
        }

        public bool TryGetSecondFromInput(ref SingleInputData singleInput, out Entity entity)
        {
            // first requires OccupiedNodeComp but second doesn't need it.
            // Second requires a connection to first
            if (TryGetEntityFromInput(ref singleInput, out entity) == false) return false;
            return connectionDB.IsConnected(first, entity);
        }

        public bool TryGetEntityFromInput(ref SingleInputData singleInput, out Entity entity)
        {
            using var hits = ArrayUtils.GetBuffer<RaycastHit>(1);
            int hitCount = Physics.RaycastNonAlloc(singleInput.GetInputRay(Camera.main), hits, 100f, 1 << PhysicsConstants.NodeLayer);
            if (hitCount > 0)
            {
                entity = hits[0].collider.XIVGetEntity();
                return entity.IsAlive() && entity.HasComponent<NodeComp>();
            }

            entity = Entity.Invalid;
            return false;
        }
        
        public Entity GetPossibleTarget(Vec2 swipeDirection)
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
                var dirToConnected = ((Vector2)(connectedEntityPos - firstNodeEntityTransformPosition)).ToVec2();
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
                fromUnitEpoch = first.GetComponent<NodeComp>().unitEpoch,
                fromEntity = first,
                toEntity = second,
                resourceQuantity = send,
            });
        }

        public void StartContinuousTransfer()
        {
            if (first.IsAlive() == false || second.IsAlive() == false || connectionDB.IsConnected(first, second) == false) return;
            
            first.RemoveComponent<SendResourceContinuouslyComp>();
            first.world.NewEntity().AddComponent(new StartContinuousResourceTransferEventComp
            {
                fromUnitEpoch = first.GetComponent<NodeComp>().unitEpoch,
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