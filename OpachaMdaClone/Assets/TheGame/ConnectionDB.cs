using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct ConnectionPair : IEquatable<ConnectionPair>
    {
        public static readonly ConnectionPair invalidConnectionPair = new ConnectionPair { entity1 = Entity.Invalid, entity2 = Entity.Invalid };
        public Entity entity1;
        public Entity entity2;
        public Vec3 startPosition;
        public Vec3 endPosition;
        public Vector3[] positions; // lineRenderer positions
        public Entity lineRendererEntity;
        public DynamicArray<Entity> resourceEntitiesOnConnection;
        byte incrementalId;

        public bool Contains(Entity entity) => entity1 == entity || entity2 == entity;

        public Entity GetOpposite(Entity entity) => entity == entity1 ? entity2 : entity == entity2 ? entity1 : Entity.Invalid;
        public Vec3 GetOppositePosition(Entity entity) => entity == entity1 ? endPosition : entity == entity2 ? startPosition : Vec3.zero;
        public Vec3 GetPosition(Entity entity) => entity == entity1 ? startPosition : entity == entity2 ? endPosition : Vec3.zero;

        public void AddResourceTransfer(Entity resourceEntity, ref TransferableResourceComp transferableResourceComp)
        {
            resourceEntitiesOnConnection.Add() = resourceEntity;
            transferableResourceComp.localId = (byte)(++incrementalId & 31);
        }

        public void RemoveResourceTransfer(Entity resourceEntity, ref TransferableResourceComp transferableResourceComp)
        {
            resourceEntitiesOnConnection.Remove(ref resourceEntity);
        }

        public static bool operator ==(ConnectionPair a, ConnectionPair b) => a.entity1 == b.entity1 && a.entity2 == b.entity2;

        public static bool operator !=(ConnectionPair a, ConnectionPair b) => !(a == b);

        public bool Equals(ConnectionPair other)
        {
            return this.entity1 == other.entity1 && this.entity2 == other.entity2;
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(entity1);
            hashCode.Add(entity2);
            hashCode.Add(startPosition);
            hashCode.Add(endPosition);
            hashCode.Add(positions);
            hashCode.Add(lineRendererEntity);
            hashCode.Add(resourceEntitiesOnConnection);
            return hashCode.ToHashCode();
        }
    }
    
    public class ConnectionDB
    {
        DynamicArray<ConnectionPair> connections = new DynamicArray<ConnectionPair>();
        Dictionary<Entity, DynamicArray<int>> entityConnectionLookup = new Dictionary<Entity, DynamicArray<int>>();
        public int Count => connections.Count;

        public ref ConnectionPair this[int index] => ref connections[index];

        public void AddConnection(Entity ent1, Entity ent2, Vec3 connectionStartPosition, Vec3 connectionEndPosition, Vector3[] positions, Entity lineRendererEntity)
        {
            if (GetConnectionIndex(ent1, ent2) != -1) throw new InvalidOperationException($"{ent1} and {ent2} are already connected");
            int idx = connections.Count;
            DynamicArray<int> list1;
            DynamicArray<int> list2;
            list1 = entityConnectionLookup.TryGetValue(ent1, out list1) ? list1 : new DynamicArray<int>();
            list2 = entityConnectionLookup.TryGetValue(ent2, out list2) ? list2 : new DynamicArray<int>();
            list1.Add() = idx;
            list2.Add() = idx;
            entityConnectionLookup.TryAdd(ent1, list1);
            entityConnectionLookup.TryAdd(ent2, list2);
            ref var connectionPair = ref connections.Add();
            connectionPair.entity1 = ent1;
            connectionPair.entity2 = ent2;
            connectionPair.startPosition = connectionStartPosition;
            connectionPair.endPosition = connectionEndPosition;
            connectionPair.positions = positions;
            connectionPair.lineRendererEntity = lineRendererEntity;
            connectionPair.resourceEntitiesOnConnection = new DynamicArray<Entity>();

        }

        public bool IsConnected(Entity ent1, Entity ent2) => GetConnectionIndex(ent1, ent2) != -1;

        public int GetConnectionIndex(Entity ent1, Entity ent2)
        {
            if (entityConnectionLookup.TryGetValue(ent1, out var list1) == false) return -1;
            if (entityConnectionLookup.TryGetValue(ent2, out var list2) == false) return -1;

            int len1 = list1.Count;
            int len2 = list2.Count;
            for (int i = 0; i < len1; i++)
            {
                ref var connectionIndex1 = ref list1[i];
                for (int j = 0; j < len2; j++)
                {
                    ref var connectionIndex2 = ref list2[j];
                    if (connectionIndex1 == connectionIndex2)
                    {
                        return connectionIndex1;
                    }
                }
            }

            return -1;
        }

        public float GetAllNeighborResourceQuantity(Entity entity, Entity unitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            var pairLen = GetAllConnectionPairs(entity, indexBuffer);
            float resourceQuantity = 0f;
            for (int i = 0; i < pairLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                ref var oppositeResourceCompComp = ref opposite.GetComponent<ResourceComp>();
                resourceQuantity += oppositeResourceCompComp.resourceQuantity;
            }

            return resourceQuantity;
        }

        public float GetHostileNeighborResourceQuantity(Entity entity, Entity unitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            var pairLen = GetAllConnectionPairs(entity, indexBuffer);
            float resourceQuantity = 0f;
            for (int i = 0; i < pairLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (IsTargetHostile(unitEntity, opposite) == false) continue;
                ref var oppositeResourceCompComp = ref opposite.GetComponent<ResourceComp>();
                resourceQuantity += oppositeResourceCompComp.resourceQuantity;
            }

            return resourceQuantity;
        }

        public float GetAllyNeighborResourceQuantity(Entity entity, Entity unitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            var pairLen = GetAllConnectionPairs(entity, indexBuffer);
            float resourceQuantity = 0f;
            for (int i = 0; i < pairLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (IsTargetAlly(unitEntity, opposite) == false) continue;
                ref var oppositeResourceCompComp = ref opposite.GetComponent<ResourceComp>();
                resourceQuantity += oppositeResourceCompComp.resourceQuantity;
            }

            return resourceQuantity;
        }

        public float GetNeighborResourceQuantity(Entity entity, Func<Entity, bool> predicate)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            var pairLen = GetAllConnectionPairs(entity, indexBuffer);
            float resourceQuantity = 0f;
            for (int i = 0; i < pairLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (predicate.Invoke(opposite) == false) continue;
                ref var oppositeResourceCompComp = ref opposite.GetComponent<ResourceComp>();
                resourceQuantity += oppositeResourceCompComp.resourceQuantity;
            }

            return resourceQuantity;
        }

        public int GetAllNeighbors(Entity entity, Entity[] entityBuffer)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            int pairLen = GetAllConnectionPairs(entity, indexBuffer);
            int entityBufferLen = entityBuffer.Length;
            int count = 0;
            for (int i = 0; i < pairLen && count < entityBufferLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                entityBuffer[count++] = opposite;
            }

            return count;
        }

        public int GetHostileNeighbors(Entity entity, Entity unitEntity, Entity[] entityBuffer)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            int pairLen = GetAllConnectionPairs(entity, indexBuffer);
            int entityBufferLen = entityBuffer.Length;
            int count = 0;
            for (int i = 0; i < pairLen && count < entityBufferLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (IsTargetHostile(unitEntity, opposite)) entityBuffer[count++] = opposite;
            }

            return count;
        }

        public int GetAllyNeighbors(Entity entity, Entity unitEntity, Entity[] entityBuffer)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            int pairLen = GetAllConnectionPairs(entity, indexBuffer);
            int entityBufferLen = entityBuffer.Length;
            int count = 0;
            for (int i = 0; i < pairLen && count < entityBufferLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (IsTargetAlly(unitEntity, opposite)) entityBuffer[count++] = opposite;
            }

            return count;
        }

        public int GetHostileAndNeutralNeighbors(Entity entity, Entity unitEntity, Entity[] entityBuffer)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            int pairLen = GetAllConnectionPairs(entity, indexBuffer);
            int entityBufferLen = entityBuffer.Length;
            int count = 0;
            for (int i = 0; i < pairLen && count < entityBufferLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (IsTargetHostile(unitEntity, opposite) || IsNeutralNode(opposite)) entityBuffer[count++] = opposite;
            }

            return count;
        }

        public int GetNeighbors(Entity entity, Entity[] entityBuffer, Func<Entity, bool> predicate)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>(16);
            int pairLen = GetAllConnectionPairs(entity, indexBuffer);
            int entityBufferLen = entityBuffer.Length;
            int count = 0;
            for (int i = 0; i < pairLen && count < entityBufferLen; i++)
            {
                ref var pair = ref this[indexBuffer[i]];
                var opposite = pair.GetOpposite(entity);
                if (predicate.Invoke(opposite) == false) continue;
                entityBuffer[count++] = opposite;
            }

            return count;
        }

        public int GetAllConnectionPairs(Entity entity, int[] indexBuffer)
        {
            int connectionLength = connections.Count;
            var bufferLength = indexBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity))
                {
                    indexBuffer[count++] = i;
                }
            }

            return count;
        }

        public int GetAlliedConnectionPairs(Entity entity, Entity unitEntity, int[] indexBuffer)
        {
            int connectionLength = connections.Count;
            var bufferLength = indexBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity))
                {
                    var opposite = pair.GetOpposite(entity);
                    if (IsTargetAlly(unitEntity, opposite)) indexBuffer[count++] = i;
                }
            }

            return count;
        }

        public int GetHostileConnectionPairs(Entity entity, Entity unitEntity, int[] indexBuffer)
        {
            int connectionLength = connections.Count;
            var bufferLength = indexBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity))
                {
                    var opposite = pair.GetOpposite(entity);
                    if (IsTargetHostile(unitEntity, opposite)) indexBuffer[count++] = i;
                }
            }

            return count;
        }

        public int GetHostileAndNeutralPairs(Entity entity, Entity unitEntity, int[] indexBuffer)
        {
            int connectionLength = connections.Count;
            var bufferLength = indexBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity))
                {
                    var opposite = pair.GetOpposite(entity);
                    if (IsNeutralNode(opposite) || IsTargetHostile(unitEntity, opposite)) indexBuffer[count++] = i;
                }
            }

            return count;
        }

        public int GetNeutralConnectionPairs(Entity entity, int[] indexBuffer)
        {
            int connectionLength = connections.Count;
            var bufferLength = indexBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity))
                {
                    var opposite = pair.GetOpposite(entity);
                    if (IsNeutralNode(opposite)) indexBuffer[count++] = i;
                }
            }

            return count;
        }

        public int GetPairs(Entity entity, int[] indexBuffer, Func<Entity, bool> predicate)
        {
            int connectionLength = connections.Count;
            var bufferLength = indexBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity))
                {
                    var opposite = pair.GetOpposite(entity);
                    if (predicate.Invoke(opposite)) indexBuffer[count++] = i;
                }
            }

            return count;
        }

        public bool IsTargetHostile(Entity attackerUnitEntity, Entity targetNode)
        {
            if (IsNeutralNode(targetNode)) return false;
            ref var oppositeOccupiedNodeComp = ref targetNode.GetComponent<OccupiedNodeComp>();
            return attackerUnitEntity != oppositeOccupiedNodeComp.unitEntity;
        }

        // TODO: Rename
        public bool IsTargetAlly(Entity attackerUnitEntity, Entity targetNode)
        {
            if (IsNeutralNode(targetNode)) return false;
            ref var oppositeOccupiedNodeComp = ref targetNode.GetComponent<OccupiedNodeComp>();
            return attackerUnitEntity == oppositeOccupiedNodeComp.unitEntity;
        }

        public bool IsNeutralNode(Entity target)
        {
            return target.HasComponent<OccupiedNodeComp>() == false;
        }

        public float GetAllyResourceTransfer(Entity entity, Entity attackerUnitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            float allyResourceTransfer = 0f;
            int connectionCount = GetAllConnectionPairs(entity, indexBuffer);
            for (int i = 0; i < connectionCount; i++)
            {
                ref var pair = ref connections[indexBuffer[i]];
                int resourceEntityCount = pair.resourceEntitiesOnConnection.Count;
                for (int j = 0; j < resourceEntityCount; j++)
                {
                    var resourceEntity = pair.resourceEntitiesOnConnection[j];
                    ref var transferableResourceComp = ref resourceEntity.GetComponent<TransferableResourceComp>();
                    if (transferableResourceComp.unitEntity == attackerUnitEntity)
                    {
                        allyResourceTransfer += transferableResourceComp.quantity;
                    }
                }
            }

            return allyResourceTransfer;
        }

        public float GetHostileResourceTransfer(Entity entity, Entity attackerUnitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            float hostileResourceTransfer = 0f;
            int connectionCount = GetAllConnectionPairs(entity, indexBuffer);
            for (int i = 0; i < connectionCount; i++)
            {
                ref var pair = ref connections[indexBuffer[i]];
                int resourceEntityCount = pair.resourceEntitiesOnConnection.Count;
                for (int j = 0; j < resourceEntityCount; j++)
                {
                    var resourceEntity = pair.resourceEntitiesOnConnection[j];
                    ref var transferableResourceComp = ref resourceEntity.GetComponent<TransferableResourceComp>();
                    if (transferableResourceComp.unitEntity != attackerUnitEntity)
                    {
                        hostileResourceTransfer += transferableResourceComp.quantity;
                    }
                }
            }

            return hostileResourceTransfer;
        }

        public int GetHostileTransferableResources(Entity entity, Entity attackerUnitEntity, Entity[] entityBuffer)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            int connectionCount = GetAllConnectionPairs(entity, indexBuffer);
            int count = 0;
            var entityBufferLength = entityBuffer.Length;
            for (int i = 0; i < connectionCount && count < entityBufferLength; i++)
            {
                ref var pair = ref connections[indexBuffer[i]];
                int resourceEntityCount = pair.resourceEntitiesOnConnection.Count;
                for (int j = 0; j < resourceEntityCount; j++)
                {
                    var resourceEntity = pair.resourceEntitiesOnConnection[j];
                    ref var transferableResourceComp = ref resourceEntity.GetComponent<TransferableResourceComp>();
                    if (transferableResourceComp.unitEntity != attackerUnitEntity)
                    {
                        entityBuffer[count++] = resourceEntity;
                    }
                }
            }

            return count;
        }

        public bool HasHostileNeighbor(Entity entity, Entity unitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            return GetHostileConnectionPairs(entity, unitEntity, indexBuffer) != 0;
        }

        public bool HasHostileOrNeutralNeighbor(Entity entity, Entity unitEntity)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            return GetHostileAndNeutralPairs(entity, unitEntity, indexBuffer) != 0;
        }
    }
}