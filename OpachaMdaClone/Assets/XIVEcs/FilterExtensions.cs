using System.Collections.Generic;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    public static class FilterExtensions
    {
        public static Entity GetClosest(this Filter filter, Vector3 pos)
        {
            float closestDistance = float.MaxValue;
            Entity closestEntity = Entity.Invalid;
            
            var nArchetypes = filter.query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = filter.query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<TransformComp>();
                for (var i = 0; i < archetype.entities.Count; i++)
                {
                    var entityPos = componentPool0.components[i].transform.position;
                    float distance = (pos - entityPos).sqrMagnitude;
                    if (distance < closestDistance)
                    {
                        closestEntity = archetype.entities[i];
                        closestDistance = distance;
                    }
                }
            }
            return closestEntity;
        }
        
        public static Entity GetClosest(this Filter filter, Vector2 pos)
        {
            float closestDistance = float.MaxValue;
            Entity closestEntity = Entity.Invalid;
            
            var nArchetypes = filter.query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = filter.query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<TransformComp>();
                for (var i = 0; i < archetype.entities.Count; i++)
                {
                    var entityPos = (Vector2)componentPool0.components[i].transform.position;
                    float distance = (pos - entityPos).sqrMagnitude;
                    if (distance < closestDistance)
                    {
                        closestEntity = archetype.entities[i];
                        closestDistance = distance;
                    }
                }
            }
            return closestEntity;
        }

        public delegate float DistanceFunction(Entity entity);
        
        public static Entity GetClosest(this Filter filter, DistanceFunction distanceFunction)
        {
            float closestDistance = float.MaxValue;
            Entity closestEntity = Entity.Invalid;
            
            var nArchetypes = filter.query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = filter.query.archetypes[a];
                for (var i = 0; i < archetype.entities.Count; i++)
                {
                    float distance = distanceFunction(archetype.entities[i]);
                    if (distance < closestDistance)
                    {
                        closestEntity = archetype.entities[i];
                        closestDistance = distance;
                    }
                }
            }
            return closestEntity;
        }
        

        public delegate bool Condition(Entity entity);

        /// Don't mutate entities inside condition
        static readonly List<Entity> buffer = new List<Entity>();
        public static Entity[] Match(this Filter filter, Condition condition)
        {
            buffer.Clear();
            filter.ForEachWithoutLock((entity =>
            {
                if (condition(entity))
                {
                    buffer.Add(entity);
                }
            } ));
            return buffer.ToArray();
        }
        
        /// Don't mutate entities inside condition
        public static XIVMemory<Entity> Entities(this Filter filter)
        {
            buffer.Clear();
            filter.ForEachWithoutLock((entity =>
            {
                buffer.Add(entity);
            } ));
            return buffer.AsXIVMemory();
        }
        
        public static int EntitiesNonAlloc(this Filter filter, Entity[] results)
        {
            int i = 0;
            filter.ForEachWithoutLock(entity =>
            {
                if (i >= results.Length)
                {
                    return;
                }
                results[i++] = entity;
            });
            return i;
        }
        
        public static int MatchNonAlloc(this Filter filter, Condition condition, Entity[] results)
        {
            int rIdx = 0;
            var nArchetypes = filter.query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = filter.query.archetypes[a];
                for (var i = 0; i < archetype.entities.Count; i++)
                {
                    var entity = archetype.entities[i];
                    if (condition(entity))
                    {
                        results[rIdx++] = entity;
                        if (rIdx == results.Length)
                        {
                            goto Return;
                        }
                    }
                }
            }

            Return:
            return rIdx;
        }
    }
}