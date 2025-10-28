using System;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Ecs;

namespace TheGame
{
    public struct ConnectionPair
    {
        public static ConnectionPair invalidConnectionPair = new ConnectionPair { entity1 = Entity.Invalid, entity2 = Entity.Invalid };
        public Entity entity1;
        public Entity entity2;
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Vector3[] positions;
        public LineRenderer lineRenderer;

        public bool Contains(Entity entity) => entity1 == entity || entity2 == entity;

        public Entity GetOpposite(Entity entity) => entity == entity1 ? entity2 : entity == entity2 ? entity1 : Entity.Invalid;
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

        public bool IsConnected(Entity ent1, Entity ent2) => GetConnectionIndex(ent1, ent2) != -1;

        public int GetConnectionIndex(Entity ent1, Entity ent2)
        {
            // TODO : ConnectionDb -> Faster connection index lookup
            int len = connections.Count;
            for (int i = 0; i < len; i++)
            {
                ref var conn = ref connections[i];
                if (conn.Contains(ent1) && conn.Contains(ent2)) return i;
            }

            return -1;
        }

        public ref ConnectionPair AddConnection(Entity ent1, Entity ent2, out bool isAdded)
        {
            isAdded = GetConnectionIndex(ent1, ent2) == -1;
            if (isAdded) return ref connections.Add();
            
            return ref ConnectionPair.invalidConnectionPair;
        }
    }
}