using System;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Ecs;

namespace TheGame
{
    public struct ConnectionPair
    {
        public static ConnectionPair invalidConnectionPairComp = new ConnectionPair { entity1 = Entity.Invalid, entity2 = Entity.Invalid };
        public Entity entity1;
        public Entity entity2;
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Vector3[] positions;
        public LineRenderer lineRenderer;

        public bool Contains(Entity entity)
        {
            return entity1 == entity || entity2 == entity;
        }

        public Entity GetOpposite(Entity entity)
        {
            return entity2 == entity ? entity1 : entity1 == entity ? entity2 : Entity.Invalid;
        }
    }
    
    public class ConnectionDB
    {
        DynamicArray<ConnectionPair> connections = new DynamicArray<ConnectionPair>();
        public int Count => connections.Count;

        public ref ConnectionPair this[int index] => ref connections[index];

        public int GetPairs(Entity entity, ConnectionPair[] pairBuffer)
        {
            int connectionLength = connections.Count;
            var bufferLength = pairBuffer.Length;
            int count = 0;
            for (int i = 0; i < connectionLength && count < bufferLength; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(entity)) pairBuffer[count++] = pair;
            }

            return count;
        }

        public bool IsConnected(Entity ent1, Entity ent2)
        {
            return GetConnectionIndex(ent1, ent2) != -1;
        }

        public int GetConnectionIndex(Entity ent1, Entity ent2)
        {
            int len = connections.Count;
            for (int i = 0; i < len; i++)
            {
                ref var conn = ref connections[i];
                if (conn.Contains(ent1) && conn.Contains(ent2)) return i;
            }

            return -1;
        }

        public ref ConnectionPair GetPair(Entity ent1, Entity ent2)
        {
            int idx = GetConnectionIndex(ent1, ent2);
            if (idx == -1) return ref ConnectionPair.invalidConnectionPairComp;
            return ref connections[idx];
        }

        public ref ConnectionPair TryAddConnection(Entity ent1, Entity ent2, out bool isAdded)
        {
            isAdded = connections.Exists(p => p.Contains(ent1) && p.Contains(ent2)) == -1;
            if (isAdded) return ref connections.Add();
            
            return ref ConnectionPair.invalidConnectionPairComp;
        }
    }
}