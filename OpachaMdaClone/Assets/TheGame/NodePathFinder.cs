using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public static class NodePathFinder
    {
        class Node<T>
        {
            protected bool Equals(Node<T> other)
            {
                return EqualityComparer<T>.Default.Equals(value, other.value) && cost.Equals(other.cost) && Equals(parent, other.parent) && Equals(child, other.child);
            }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Node<T>)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(value, cost, parent, child);
            }

            public T value;
            public float cost;
            public Node<T> parent;
            public Node<T> child;

            public void Set(T value, float cost, Node<T> parent, Node<T> child)
            {
                this.value = value;
                this.cost = cost;
                this.parent = parent;
                this.child = child;
            }

            public static bool operator ==(Node<T> a, Node<T> b)
            {
                if (a is null && b is null) return true;
                return a is not null && a.Equals((object)b);
            }

            public static bool operator !=(Node<T> a, Node<T> b)
            {
                return !(a == b);
            }
        }
        
        const int BUFFER_MIN_LEN = 16;
        
        static DynamicArray<Node<Entity>> openList;
        static HashSet<Entity> closedList;
        static DynamicArray<Node<Entity>> nodeBuffer;
        static DynamicArray<Entity> path;

        public static void Init()
        {
            openList = new DynamicArray<Node<Entity>>();
            closedList = new HashSet<Entity>();
            nodeBuffer = new DynamicArray<Node<Entity>>();
            path = new DynamicArray<Entity>();
        }
        
        // Path to the nearest neutral or hostile
        public static void GetPathToFirstTarget(Entity entity, ConnectionDB connectionDB, float maxResourceQuantity, ref DynamicArray<Entity> buffer)
        {
            buffer ??= new DynamicArray<Entity>();
            buffer.Clear();
            if (entity.HasComponent<OccupiedNodeComp>() == false) return;
            
            ref var occupiedNodeComp = ref entity.GetComponent<OccupiedNodeComp>();
            var attackerUnitEntity = occupiedNodeComp.unitEntity;
            ref var unitComp = ref attackerUnitEntity.GetComponent<UnitComp>();

            using var indexBuffer = ArrayUtils.GetBuffer<int>(BUFFER_MIN_LEN);
            using var tempIndexBuffer = ArrayUtils.GetBuffer<int>(BUFFER_MIN_LEN);
            openList.Clear();
            closedList.Clear();

            var startNode = GetNode(entity, 0, null);
            openList.Add() = startNode;

            while (openList.Count > 0)
            {
                var currentNode = GetCurrent();

                if (closedList.Add(currentNode.value) == false) continue;

                if (connectionDB.IsTargetAlly(attackerUnitEntity, currentNode.value) == false)
                {
                    var path = ReconstructPath(currentNode, connectionDB);
                    buffer.AddRange(path);
                    ReturnWithParents(currentNode);
                }

                int len = connectionDB.GetAllConnectionPairs(currentNode.value, indexBuffer);
                if (len == 0) continue;
                
                var currentPos = connectionDB[indexBuffer[0]].GetPosition(currentNode.value);

                for(int i = 0; i < len; i++)
                {
                    ref var connectionPair = ref connectionDB[indexBuffer[i]];
                    var neighbor = connectionPair.GetOpposite(currentNode.value);

                    if (Contains(openList, neighbor)) continue;
                    int hostilePairs = connectionDB.GetHostileConnectionPairs(neighbor, attackerUnitEntity, tempIndexBuffer);
                    var hostileRatio = (float)hostilePairs / len; // e.g. 0.75f
                    var dangerScore = connectionDB.GetHostileNeighborResourceQuantity(neighbor, attackerUnitEntity) / maxResourceQuantity;
                    var neighborPosition = connectionPair.GetPosition(neighbor);
                    var distance = Vec3.Distance(currentPos, neighborPosition);
                    openList.Add() = GetNode(neighbor, cost: currentNode.cost + (hostileRatio * unitComp.smartness01) + (dangerScore * unitComp.smartness01) + distance, parent: currentNode);
                }

            }

            foreach (var node in openList)
            {
                ReturnWithParents(node);
            }
        }
        
        // Path to target
        public static void GetPathToTarget(Entity entity, Entity targetEntity, ConnectionDB connectionDB, float maxResourceQuantity, DynamicArray<Entity> buffer)
        {
            buffer.Clear();
            ref var occupiedNodeComp = ref entity.GetComponent<OccupiedNodeComp>();
            var attackerUnitEntity = occupiedNodeComp.unitEntity;
            ref var unitComp = ref attackerUnitEntity.GetComponent<UnitComp>();

            using var indexBuffer = ArrayUtils.GetBuffer<int>(BUFFER_MIN_LEN);
            using var tempIndexBuffer = ArrayUtils.GetBuffer<int>(BUFFER_MIN_LEN);
            openList.Clear();
            closedList.Clear();

            var startNode = GetNode(entity, 0, null);
            openList.Add() = startNode;

            while (openList.Count > 0)
            {
                var currentNode = GetCurrent();

                if (closedList.Add(currentNode.value) == false) continue;

                if (currentNode.value == targetEntity)
                {
                    var path = ReconstructPath(currentNode, connectionDB);
                    buffer.AddRange(path);
                    ReturnWithParents(currentNode);
                }

                int len = connectionDB.GetAllConnectionPairs(currentNode.value, indexBuffer);
                if (len == 0) continue;
                var currentPos = connectionDB[indexBuffer[0]].GetPosition(currentNode.value);

                for(int i = 0; i < len; i++)
                {
                    ref var connectionPair = ref connectionDB[indexBuffer[i]];
                    var neighbor = connectionPair.GetOpposite(currentNode.value);

                    if (Contains(openList, neighbor)) continue;
                    int hostilePairs = connectionDB.GetHostileConnectionPairs(neighbor, attackerUnitEntity, tempIndexBuffer);
                    var hostileRatio = (float)hostilePairs / len; // e.g. 0.75f
                    var dangerScore = connectionDB.GetHostileNeighborResourceQuantity(neighbor, attackerUnitEntity) / maxResourceQuantity;
                    var neigborPosition = connectionPair.GetPosition(neighbor);
                    var distance = Vec3.Distance(currentPos, neigborPosition);
                    openList.Add() = GetNode(neighbor, cost: currentNode.cost + (hostileRatio * unitComp.smartness01) + (dangerScore * unitComp.smartness01) + distance, parent: currentNode);
                }

            }

            foreach (var node in openList)
            {
                ReturnWithParents(node);
            }
        }

        static Node<Entity> GetCurrent()
        {
            int idx = -1;
            Node<Entity> currentNode = default;
            int openListLength = openList.Count;
            float best = float.MaxValue;
            for (int i = 0; i < openListLength; i++)
            {
                var node = openList[i];
                if (node.cost < best)
                {
                    idx = i;
                    currentNode = node;
                    best = node.cost;
                }
            }
            var last = openList.RemoveLast();
            if (idx < openList.Count) openList[idx] = last;

            return currentNode;
        }

        static Node<Entity> GetNode(Entity value, float cost, Node<Entity> parent)
        {
            var node = nodeBuffer.Count > 0 ? nodeBuffer.RemoveLast() : new Node<Entity>();
            node.Set(value, cost, parent, null);
            if (parent != null) parent.child = node;
            return node;
        }

        static void ReturnNode(Node<Entity> node)
        {
            if (nodeBuffer.Contains(ref node) == false) nodeBuffer.Add() = node;
            node.value = Entity.Invalid;
            node.cost = 0f;
            node.parent = null;
            node.child = null;
        }

        static void ReturnWithParents(Node<Entity> node)
        {
            while (node != null)
            {
                ReturnNode(node);
                node = node.parent;
            }
        }

        static bool Contains(IEnumerable<Node<Entity>> collection, Entity e)
        {
            foreach (var node in collection)
            {
                if (node.value == e) return true;
            }

            return false;
        }

        static XIVMemory<Entity> ReconstructPath(Node<Entity> node, ConnectionDB connectionDB)
        {
            path.Clear();
            while (node != null)
            {
                path.Add() = node.value;
                node = node.parent;
            }

            // TODO: Smooth the path
            // int len = path.Count;
            // // ignore first and last
            // for (int i = 1; i < len - 1 && i > 0; i++)
            // {
            //     var ent1 = path[i];
            //     var ent1Position = ent1.GetComponent<PositionComp>().position;
            //     using var entityBuffer = ArrayUtils.GetBuffer<Entity>();
            //     var neighborCount = connectionDB.GetAllNeighbors(ent1, entityBuffer);
            //     var filtered = entityBuffer.AsXIVMemory().FilterBy(neighborCount, path.Contains);
            //     var filteredLen = filtered.Length;
            //     var closest = filtered.GetClosest(filteredLen, ent1Position, (entity) => entity.GetComponent<PositionComp>().position);
            //     for (int j = 0; j < filteredLen; j++)
            //     {
            //         var filteredEntity = filtered[j];
            //         if (filteredEntity == closest || filteredEntity == path[0] || filteredEntity == path[^1]) continue;
            //         path.Remove(filteredEntity);
            //         i--;
            //         len--;
            //     }
            // }
            return path.AsXIVMemory().reversed;
        }
    }
}