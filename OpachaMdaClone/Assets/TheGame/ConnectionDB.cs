using System;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Ecs;

namespace TheGame
{
    public class ConnectionDB
    {
        public struct Pair
        {
            public Entity entity1;
            public Entity entity2;
            public Entity lineRendererEntity;

            public Pair(Entity entity1, Entity entity2, Entity lineRendererEntity)
            {
                this.entity1 = entity1;
                this.entity2 = entity2;
                this.lineRendererEntity = lineRendererEntity;
            }

            public bool Contains(Entity entity)
            {
                return entity1 == entity || entity2 == entity;
            }

            public Entity GetOpposite(Entity entity)
            {
                return entity2 == entity ? entity1 : entity1 == entity ? entity2 : Entity.Invalid;
            }
        }
        
        DynamicArray<Pair> connections = new DynamicArray<Pair>();
        public int Count => connections.Count;
        public ReadOnlySpan<Pair> Connections => connections.AsReadOnlySpan();

        public bool TryAddConnection(Entity entity1, Entity entity2)
        {
            int idx = connections.Exists(p => p.Contains(entity1) && p.Contains(entity2));
            if (idx != -1) return false;
            connections.Add() = new Pair
            {
                entity1 = entity1,
                entity2 = entity2
            };
            return true;
        }

        public void AssignLineRenderer(Entity entity1, Entity entity2, Entity lineRendererEntity)
        {
            int idx = connections.Exists(p => p.Contains(entity1) && p.Contains(entity2));
            if (idx == -1) throw new InvalidOperationException("This connection doesn't exists");
            ref var pair = ref connections[idx];
            pair.lineRendererEntity = lineRendererEntity;
        }

        public void RemoveConnection(Entity entity1, Entity entity2)
        {
            connections.RemoveAll(p => p.Contains(entity1) && p.Contains(entity2));
        }

        public int GetPairs(Entity entity, Pair[] pairBuffer)
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

        int GetConnectionIndex(Entity ent1, Entity ent2)
        {
            int count = connections.Count;
            for (int i = 0; i < count; i++)
            {
                ref var pair = ref connections[i];
                if (pair.Contains(ent1) && pair.Contains(ent2)) return i;
            }
            return -1;
        }

        public Pair GetPair(Entity ent1, Entity ent2)
        {
            int idx = GetConnectionIndex(ent1, ent2);
            if (idx == -1) return new Pair(Entity.Invalid, Entity.Invalid, Entity.Invalid);
            return connections[idx];
        }
    }
}