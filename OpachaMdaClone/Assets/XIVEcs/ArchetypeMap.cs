using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace XIV.Ecs
{
    public class ArchetypeMap
    {
        public class Node
        {
            public Archetype archetype;
            public Node next;
        }

        public readonly List<Archetype> archetypes = new List<Archetype>();
        public Node[] buckets = new Node[128];

        public Dictionary<int, Delegate> customResetMap;
        public Dictionary<int, Delegate> customAssignMap;

        Archetype cachedArchetype = null;
        // lazy boxed delegates for dynamic paths
        // Set existing component value (pool.Set(idx, boxedValue))
        public Dictionary<int, Action<ComponentPoolBase, int, object>> boxedSetMap;
        // Set new component into freshly allocated slot (pool.SetNewComponent(idx, boxedValue))
        public Dictionary<int, Action<ComponentPoolBase, int, object>> boxedSetNewMap;

        public ArchetypeMap()
        {
            customResetMap = new Dictionary<int, Delegate>(ComponentIdManager.ComponentTypeCount);
            customAssignMap = new Dictionary<int, Delegate>(ComponentIdManager.ComponentTypeCount);
            
            boxedSetMap = new Dictionary<int, Action<ComponentPoolBase, int, object>>(ComponentIdManager.ComponentTypeCount);
            boxedSetNewMap = new Dictionary<int, Action<ComponentPoolBase, int, object>>(ComponentIdManager.ComponentTypeCount);
        }

        // ensure boxed Set delegate exists for componentId
        Action<ComponentPoolBase, int, object> EnsureBoxedSet(int componentId)
        {
            if (boxedSetMap.TryGetValue(componentId, out var action)) return action;

            var poolType = ComponentIdManager.GetComponentPoolType(componentId); // e.g. typeof(ComponentPool<TheType>)
            var method = poolType.GetMethod("Set", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int), typeof(object) }, null);

            // Prefer calling typed Set(int, in T) if present to avoid runtime cast inside pool method
            if (method == null)
            {
                // fallback to any Set method (object overload exists on your class)
                method = poolType.GetMethod("Set", BindingFlags.Instance | BindingFlags.Public);
            }

            // Build Expression: (ComponentPoolBase poolBase, int idx, object val) => ((PoolType)poolBase).Set(idx, val);
            var poolParam = Expression.Parameter(typeof(ComponentPoolBase), "poolBase");
            var idxParam = Expression.Parameter(typeof(int), "idx");
            var valParam = Expression.Parameter(typeof(object), "val");

            var convertedPool = Expression.Convert(poolParam, poolType);

            // If method expects (int, object) we can pass valParam directly.
            // If method expects (int, T) we must convert valParam to T.
            ParameterInfo[] parameters = method.GetParameters();
            Expression call;
            if (parameters.Length == 2 && parameters[1].ParameterType == typeof(object))
            {
                call = Expression.Call(convertedPool, method, idxParam, valParam);
            }
            else if (parameters.Length == 2)
            {
                // assume second parameter is the concrete T (e.g. Set(int, T) or Set(int, in T))
                var targetType = parameters[1].ParameterType;
                var convertedVal = Expression.Convert(valParam, targetType);
                call = Expression.Call(convertedPool, method, idxParam, convertedVal);
            }
            else
            {
                // unexpected signature; fallback to reflection invoke in a closure
                Action<ComponentPoolBase, int, object> fallback = (pool, idx, value) => { method.Invoke(pool, new object[] { idx, value }); };
                boxedSetMap[componentId] = fallback;
                return fallback;
            }

            var lambda = Expression.Lambda<Action<ComponentPoolBase, int, object>>(call, poolParam, idxParam, valParam);
            var compiled = lambda.Compile();
            boxedSetMap[componentId] = compiled;
            return compiled;
        }

        Action<ComponentPoolBase, int, object> EnsureBoxedSetNew(int componentId)
        {
            if (boxedSetNewMap.TryGetValue(componentId, out var action)) return action;

            var poolType = ComponentIdManager.GetComponentPoolType(componentId);
            var method = poolType.GetMethod("SetNewComponent", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int), typeof(object) }, null);

            if (method == null)
            {
                method = poolType.GetMethod("SetNewComponent", BindingFlags.Instance | BindingFlags.Public);
            }

            var poolParam = Expression.Parameter(typeof(ComponentPoolBase), "poolBase");
            var idxParam = Expression.Parameter(typeof(int), "idx");
            var valParam = Expression.Parameter(typeof(object), "val");

            var convertedPool = Expression.Convert(poolParam, poolType);

            ParameterInfo[] parameters = method.GetParameters();
            Expression call;
            if (parameters.Length == 2 && parameters[1].ParameterType == typeof(object))
            {
                call = Expression.Call(convertedPool, method, idxParam, valParam);
            }
            else if (parameters.Length == 2)
            {
                var targetType = parameters[1].ParameterType;
                var convertedVal = Expression.Convert(valParam, targetType);
                call = Expression.Call(convertedPool, method, idxParam, convertedVal);
            }
            else
            {
                Action<ComponentPoolBase, int, object> fallback = (pool, idx, value) => { method.Invoke(pool, new object[] { idx, value }); };
                boxedSetNewMap[componentId] = fallback;
                return fallback;
            }

            var lambda = Expression.Lambda<Action<ComponentPoolBase, int, object>>(call, poolParam, idxParam, valParam);
            var compiled = lambda.Compile();
            boxedSetNewMap[componentId] = compiled;
            return compiled;
        }

// Public invokers used by World for dynamic paths:
        public void InvokeBoxedSet(Archetype archetype, int componentId, int idx, object value)
        {
            var pool = archetype.GetComponentPool(componentId);
            var action = EnsureBoxedSet(componentId);
            action(pool, idx, value);
        }

        public void InvokeBoxedSetNew(Archetype archetype, int componentId, int idx, object value)
        {
            var pool = archetype.GetComponentPool(componentId);
            var action = EnsureBoxedSetNew(componentId);
            action(pool, idx, value);
        }


        public Archetype GetArchetype(Bitset componentBitset, Bitset tagBitset, out bool newArchetypeGenerated)
        {
            newArchetypeGenerated = false;
            if (cachedArchetype != null && IsArchetypeCompatible(cachedArchetype))
            {
                return cachedArchetype;
            }

            var hash = GetHash();
            int bucketIdx = hash % buckets.Length;
            var node = buckets[bucketIdx];

            Archetype archetype = null;

            if (node == null)
            {
                // Create New Archetype
                newArchetypeGenerated = true;
                archetype = CreateNewArchetype(componentBitset, tagBitset, out var newNode);
                buckets[bucketIdx] = newNode;
            }
            else
            {
                while (true)
                {
                    if (IsArchetypeCompatible(node.archetype))
                    {
                        archetype = node.archetype;
                        break;
                    }

                    if (node.next == null)
                    {
                        newArchetypeGenerated = true;
                        archetype = CreateNewArchetype(componentBitset, tagBitset, out var newNode);
                        node.next = newNode;
                        break;
                    }

                    node = node.next;
                }
            }

            if (newArchetypeGenerated)
            {
                archetypes.Add(archetype);
                archetype.SetCustomReset(customResetMap);
                archetype.SetCustomAssign(customAssignMap);
            }

            cachedArchetype = archetype;
            return cachedArchetype;

            bool IsArchetypeCompatible(Archetype archetype)
            {
                return archetype.GetComponentBitSet().Equals(ref componentBitset) && archetype.GetTagBitset().Equals(ref tagBitset);
            }

            int GetHash()
            {
                int v = HashCode.Combine(componentBitset.GetHashCode(), tagBitset.GetHashCode());
                if (v < 0) v = -v;
                return v;
            }
        }

        static Archetype CreateNewArchetype(Bitset componentBitset, Bitset tagBitset, out Node newNode)
        {
            var archetype = new Archetype(Bitset.Copy(ref componentBitset), Bitset.Copy(ref tagBitset));
            newNode = new Node()
            {
                archetype = archetype,
                next = null
            };
            return archetype;
        }

        public void ChangeArchetype(World world, EntityId entityId, EntityDataList entityDataList, Archetype newArchetype)
        {
            ref var entityData = ref entityDataList[entityId.id];
            var oldArchetype = entityData.archetype;
            int oldEntityArchetypeIndex = entityData.indexInArchetype;

            // Early exit: same archetype -> nothing to do
            if (oldArchetype == newArchetype) return;

            // 1) Add & reserve slots in new archetype
            newArchetype.AddEntity(world, ref entityId, ref entityData);

            // 2) If there was no old archetype, we're done
            if (oldArchetype == null) return;

            // 3) Merge/copy/remove components
            oldArchetype.MoveTo(newArchetype, oldEntityArchetypeIndex, entityData.indexInArchetype, entityDataList);
        }

        public void SetNewComponent<T>(EntityData entityData, in T component) where T : struct, IComponent
        {
            entityData.archetype.GetComponentPool<T>().SetNewComponent(entityData.indexInArchetype, in component);
        }
    }
}