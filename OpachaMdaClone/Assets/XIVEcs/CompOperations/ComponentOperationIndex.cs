using System;
using System.Runtime.CompilerServices;
using XIV.Core.Collections;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    public static class ComponentOperationIndex
    {
        public enum OpType : byte
        {
            AddComponent,
            RemoveComponent,
            EnableComponent,
            DisableComponent,
            AddTag,
            RemoveTag
        }

        public struct QueuedOp
        {
            public OpType Type;
            public int TypeId;       // componentId or tagId
            public EntityId Entity;
        }

        // Unified single-thread queue (for simple usage)
        static DynamicArray<QueuedOp> opQueue;

        // Per-thread queues (ParallelWriter)
        static DynamicArray<QueuedOp>[] threadOpBuffers;
        static int maxParallelWriters = 0;

        // Executor delegates (batched): calling these executes all pending ops for that type
        static Action<World>[] addComponentExec;
        static Action<World>[] removeComponentExec;
        static Action<World>[] enableComponentExec;
        static Action<World>[] disableComponentExec;

        static Action<World>[] addTagExec;
        static Action<World>[] removeTagExec;

        // Convenience direct-by-id delegates (non-batched helpers)
        static Action<EntityId>[] removeComponentById;
        static Action<EntityId>[] addTagById;
        static Action<EntityId>[] removeTagById;

        // Small flag arrays to ensure we execute each (TypeKind, TypeId) at most once per ExecutePending call
        static byte[] executedAddComponent;
        static byte[] executedRemoveComponent;
        static byte[] executedEnableComponent;
        static byte[] executedDisableComponent;
        static byte[] executedAddTag;
        static byte[] executedRemoveTag;

        // Lists of indices set (so we can clear flags cheaply)
        static DynamicArray<int> executedAddComponentIndices;
        static DynamicArray<int> executedRemoveComponentIndices;
        static DynamicArray<int> executedEnableComponentIndices;
        static DynamicArray<int> executedDisableComponentIndices;
        static DynamicArray<int> executedAddTagIndices;
        static DynamicArray<int> executedRemoveTagIndices;

        // --- Init ---------------------------------------------------------
        public static void Init(TypeManager typeManager, int parallelWriters = 0)
        {
            var componentTypes = typeManager.GetComponentTypes();
            var tagTypes = typeManager.GetTagTypes();
            int componentTypeCount = componentTypes.Count;
            int tagTypeCount = tagTypes.Count;

            // Queues
            opQueue = new DynamicArray<QueuedOp>(1024);

            // Executors arrays
            addComponentExec = new Action<World>[componentTypeCount];
            removeComponentExec = new Action<World>[componentTypeCount];
            enableComponentExec = new Action<World>[componentTypeCount];
            disableComponentExec = new Action<World>[componentTypeCount];

            addTagExec = new Action<World>[tagTypeCount];
            removeTagExec = new Action<World>[tagTypeCount];

            removeComponentById = new Action<EntityId>[componentTypeCount];
            addTagById = new Action<EntityId>[tagTypeCount];
            removeTagById = new Action<EntityId>[tagTypeCount];

            // Flag arrays
            executedAddComponent = new byte[componentTypeCount];
            executedRemoveComponent = new byte[componentTypeCount];
            executedEnableComponent = new byte[componentTypeCount];
            executedDisableComponent = new byte[componentTypeCount];

            executedAddTag = new byte[tagTypeCount];
            executedRemoveTag = new byte[tagTypeCount];

            executedAddComponentIndices = new DynamicArray<int>(64);
            executedRemoveComponentIndices = new DynamicArray<int>(64);
            executedEnableComponentIndices = new DynamicArray<int>(64);
            executedDisableComponentIndices = new DynamicArray<int>(64);
            executedAddTagIndices = new DynamicArray<int>(64);
            executedRemoveTagIndices = new DynamicArray<int>(64);

            // Build delegates for each component type
            for (int componentId = 0; componentId < componentTypeCount; componentId++)
            {
                var componentType = componentTypes[componentId];

                var addComponentType = typeof(AddComponentOperations<>).MakeGenericType(componentType);
                var removeComponentType = typeof(RemoveComponentOperations<>).MakeGenericType(componentType);
                var activateComponentType = typeof(ActivateComponentOperations<>).MakeGenericType(componentType);

                addComponentType.XIVGetMethodByName("Init")?.Invoke(addComponentType, Array.Empty<object>());
                removeComponentType.XIVGetMethodByName("Init")?.Invoke(removeComponentType, Array.Empty<object>());
                activateComponentType.XIVGetMethodByName("Init")?.Invoke(activateComponentType, Array.Empty<object>());

                // Each of these Execute methods should process ALL per-type queued items (single-threaded or per-thread merged).
                var addExecMethod = addComponentType.XIVGetMethodByName("Execute");
                addComponentExec[componentId] = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), addExecMethod);

                var removeExecMethod = removeComponentType.XIVGetMethodByName("Execute");
                removeComponentExec[componentId] = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), removeExecMethod);

                var enableExecMethod = activateComponentType.XIVGetMethodByName("ExecuteEnableComponent");
                enableComponentExec[componentId] = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), enableExecMethod);

                var disableExecMethod = activateComponentType.XIVGetMethodByName("ExecuteDisableComponent");
                disableComponentExec[componentId] = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), disableExecMethod);

                // compatibility direct-by-id
                var removeByIdMethod = removeComponentType.XIVGetMethodByName("RemoveComponent");
                removeComponentById[componentId] = (Action<EntityId>)Delegate.CreateDelegate(typeof(Action<EntityId>), removeByIdMethod);
            }

            // Build tag delegates
            for (int tagId = 0; tagId < tagTypeCount; tagId++)
            {
                var tagType = tagTypes[tagId];
                var addTagType = typeof(AddTagOperations<>).MakeGenericType(tagType);
                var removeTagType = typeof(RemoveTagOperations<>).MakeGenericType(tagType);

                addTagType.XIVGetMethodByName("Init")?.Invoke(addTagType, Array.Empty<object>());
                removeTagType.XIVGetMethodByName("Init")?.Invoke(removeTagType, Array.Empty<object>());

                var addExecMethod = addTagType.XIVGetMethodByName("Execute");
                addTagExec[tagId] = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), addExecMethod);

                var removeExecMethod = removeTagType.XIVGetMethodByName("Execute");
                removeTagExec[tagId] = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), removeExecMethod);

                var addByIdMethod = addTagType.XIVGetMethodByName("AddTag");
                addTagById[tagId] = (Action<EntityId>)Delegate.CreateDelegate(typeof(Action<EntityId>), addByIdMethod);

                var removeByIdMethod = removeTagType.XIVGetMethodByName("RemoveTag");
                removeTagById[tagId] = (Action<EntityId>)Delegate.CreateDelegate(typeof(Action<EntityId>), removeByIdMethod);
            }

            // Parallel writers
            if (parallelWriters > 0)
            {
                maxParallelWriters = parallelWriters;
                threadOpBuffers = new DynamicArray<QueuedOp>[parallelWriters];
                for (int i = 0; i < parallelWriters; i++) threadOpBuffers[i] = new DynamicArray<QueuedOp>(256);
            }
        }

        // -------------------------
        // Enqueue API (single-threaded)
        // -------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponent<T>(EntityId entityId, in T componentValue) where T : struct, IComponent
        {
            AddComponentOperations<T>.AddComponent(entityId, componentValue);
            var id = ComponentIdManager.GetComponentId<T>();
            ref var r = ref opQueue.Add();
            r.Type = OpType.AddComponent;
            r.TypeId = id;
            r.Entity = entityId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            RemoveComponentOperations<T>.RemoveComponent(entityId);
            var id = ComponentIdManager.GetComponentId<T>();
            ref var r = ref opQueue.Add();
            r.Type = OpType.RemoveComponent;
            r.TypeId = id;
            r.Entity = entityId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableComponent<T>(World world, EntityId entityId) where T : struct, IComponent
        {
            ActivateComponentOperations<T>.EnableComponent(entityId);
            var id = ComponentIdManager.GetComponentId<T>();
            ref var r = ref opQueue.Add();
            r.Type = OpType.EnableComponent;
            r.TypeId = id;
            r.Entity = entityId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisableComponent<T>(World world, EntityId entityId) where T : struct, IComponent
        {
            ActivateComponentOperations<T>.DisableComponent(world, entityId); // Disable enqueues internal removal too
            var id = ComponentIdManager.GetComponentId<T>();
            ref var r = ref opQueue.Add();
            r.Type = OpType.DisableComponent;
            r.TypeId = id;
            r.Entity = entityId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTag<T>(EntityId entityId) where T : struct, ITag
        {
            AddTagOperations<T>.AddTag(entityId);
            var id = TagIdManager.GetTagId<T>();
            ref var r = ref opQueue.Add();
            r.Type = OpType.AddTag;
            r.TypeId = id;
            r.Entity = entityId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveTag<T>(EntityId entityId) where T : struct, ITag
        {
            RemoveTagOperations<T>.RemoveTag(entityId);
            var id = TagIdManager.GetTagId<T>();
            ref var r = ref opQueue.Add();
            r.Type = OpType.RemoveTag;
            r.TypeId = id;
            r.Entity = entityId;
        }

        // Direct by-id convenience
        public static void RemoveComponent(EntityId entityId, int componentId) => removeComponentById[componentId](entityId);
        public static void AddTag(EntityId entityId, int tagId) => addTagById[tagId](entityId);
        public static void RemoveTag(EntityId entityId, int tagId) => removeTagById[tagId](entityId);

        // // -------------------------
        // // ParallelWriter API
        // // -------------------------
        // public struct ParallelWriter
        // {
        //     readonly DynamicArray<QueuedOp>[] buffers;
        //     readonly int threadIndex;
        //
        //     internal ParallelWriter(DynamicArray<QueuedOp>[] buffers, int threadIndex)
        //     {
        //         this.buffers = buffers;
        //         this.threadIndex = threadIndex;
        //     }
        //
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     public void AddComponent<T>(EntityId entityId, in T componentValue) where T : struct, IComponent
        //     {
        //         // Per-type class must expose QueueThread for threadIndex
        //         AddComponentOperations<T>.QueueThread(threadIndex, entityId, componentValue);
        //         var id = ComponentIdManager.GetComponentId<T>();
        //         ref var r = ref buffers[threadIndex].Add();
        //         r.Type = OpType.AddComponent;
        //         r.TypeId = id;
        //         r.Entity = entityId;
        //     }
        //
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     public void RemoveComponent<T>(EntityId entityId) where T : struct, IComponent
        //     {
        //         RemoveComponentOperations<T>.QueueThread(threadIndex, entityId);
        //         var id = ComponentIdManager.GetComponentId<T>();
        //         ref var r = ref buffers[threadIndex].Add();
        //         r.Type = OpType.RemoveComponent;
        //         r.TypeId = id;
        //         r.Entity = entityId;
        //     }
        //
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     public void EnableComponent<T>(EntityId entityId) where T : struct, IComponent
        //     {
        //         ActivateComponentOperations<T>.QueueEnableThread(threadIndex, entityId);
        //         var id = ComponentIdManager.GetComponentId<T>();
        //         ref var r = ref buffers[threadIndex].Add();
        //         r.Type = OpType.EnableComponent;
        //         r.TypeId = id;
        //         r.Entity = entityId;
        //     }
        //
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     public void DisableComponent<T>(EntityId entityId) where T : struct, IComponent
        //     {
        //         ActivateComponentOperations<T>.QueueDisableThread(threadIndex, entityId);
        //         var id = ComponentIdManager.GetComponentId<T>();
        //         ref var r = ref buffers[threadIndex].Add();
        //         r.Type = OpType.DisableComponent;
        //         r.TypeId = id;
        //         r.Entity = entityId;
        //     }
        //
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     public void AddTag<T>(EntityId entityId) where T : struct, ITag
        //     {
        //         AddTagOperations<T>.QueueThread(threadIndex, entityId);
        //         var id = TagIdManager.GetTagId<T>();
        //         ref var r = ref buffers[threadIndex].Add();
        //         r.Type = OpType.AddTag;
        //         r.TypeId = id;
        //         r.Entity = entityId;
        //     }
        //
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     public void RemoveTag<T>(EntityId entityId) where T : struct, ITag
        //     {
        //         RemoveTagOperations<T>.QueueThread(threadIndex, entityId);
        //         var id = TagIdManager.GetTagId<T>();
        //         ref var r = ref buffers[threadIndex].Add();
        //         r.Type = OpType.RemoveTag;
        //         r.TypeId = id;
        //         r.Entity = entityId;
        //     }
        // }
        //
        // public static ParallelWriter GetParallelWriter(int threadIndex)
        // {
        //     if (threadOpBuffers == null) throw new InvalidOperationException("Parallel writers not initialized. Call Init(..., parallelWriters).");
        //     if ((uint)threadIndex >= (uint)threadOpBuffers.Length) throw new ArgumentOutOfRangeException(nameof(threadIndex));
        //     return new ParallelWriter(threadOpBuffers, threadIndex);
        // }
        
        public static void CollapseAdjacent()
        {
            int dst = 0;

            var opQueueCount = opQueue.Count;
            for (int src = 0; src < opQueueCount; src++)
            {
                var cur = opQueue[src];

                if (dst > 0)
                {
                    ref var prev = ref opQueue[dst - 1];

                    bool sameType = prev.TypeId == cur.TypeId &&
                                    prev.Entity.id == cur.Entity.id &&
                                    prev.Entity.generation == cur.Entity.generation;

                    if (sameType)
                    {
                        // ADD → REMOVE  => collapse both (final = removed)
                        if (prev.Type == OpType.AddComponent && cur.Type == OpType.RemoveComponent)
                        {
                            dst--;
                            continue;
                        }

                        // REMOVE → ADD => DO NOT collapse (final = add with new value)

                        // ADD → ADD => keep only newest (replace prev)
                        if (prev.Type == OpType.AddComponent && cur.Type == OpType.AddComponent)
                        {
                            // overwrite previous Add with current Add
                            prev = cur;
                            continue;
                        }

                        // REMOVE → REMOVE => keep only one
                        if (prev.Type == OpType.RemoveComponent && cur.Type == OpType.RemoveComponent)
                        {
                            // ignore newest remove
                            continue;
                        }
                    }
                }

                opQueue[dst++] = cur;
            }

            var removeCount = opQueueCount - dst;
            for (int i = 0; i < removeCount; i++)
            {
                opQueue.RemoveLast();
            }
        }

        // -------------------------
        // ExecutePending (single-thread): iterate opQueue in order, but batch per TypeId+OpType
        // -------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecutePending(World world)
        {
            var span = opQueue.AsReadOnlySpan();
            var spanLength = span.Length;
            for (int i = 0; i < spanLength; i++)
            {
                ref readonly var op = ref span[i];
                switch (op.Type)
                {
                    case OpType.AddComponent:
                        if (executedAddComponent[op.TypeId] == 0)
                        {
                            executedAddComponent[op.TypeId] = 1;
                            executedAddComponentIndices.Add() = op.TypeId;
                            addComponentExec[op.TypeId](world);
                        }
                        break;
                    case OpType.RemoveComponent:
                        if (executedRemoveComponent[op.TypeId] == 0)
                        {
                            executedRemoveComponent[op.TypeId] = 1;
                            executedRemoveComponentIndices.Add() = op.TypeId;
                            removeComponentExec[op.TypeId](world);
                        }
                        break;
                    case OpType.EnableComponent:
                        if (executedEnableComponent[op.TypeId] == 0)
                        {
                            executedEnableComponent[op.TypeId] = 1;
                            executedEnableComponentIndices.Add() = op.TypeId;
                            enableComponentExec[op.TypeId](world);
                        }
                        break;
                    case OpType.DisableComponent:
                        if (executedDisableComponent[op.TypeId] == 0)
                        {
                            executedDisableComponent[op.TypeId] = 1;
                            executedDisableComponentIndices.Add() = op.TypeId;
                            disableComponentExec[op.TypeId](world);
                        }
                        break;
                    case OpType.AddTag:
                        if (executedAddTag[op.TypeId] == 0)
                        {
                            executedAddTag[op.TypeId] = 1;
                            executedAddTagIndices.Add() = op.TypeId;
                            addTagExec[op.TypeId](world);
                        }
                        break;
                    case OpType.RemoveTag:
                        if (executedRemoveTag[op.TypeId] == 0)
                        {
                            executedRemoveTag[op.TypeId] = 1;
                            executedRemoveTagIndices.Add() = op.TypeId;
                            removeTagExec[op.TypeId](world);
                        }
                        break;
                }
            }

            // clear op queue
            opQueue.Clear();

            // reset flags using collected indices (fast)
            for (int i = 0; i < executedAddComponentIndices.Count; i++) executedAddComponent[executedAddComponentIndices[i]] = 0;
            for (int i = 0; i < executedRemoveComponentIndices.Count; i++) executedRemoveComponent[executedRemoveComponentIndices[i]] = 0;
            for (int i = 0; i < executedEnableComponentIndices.Count; i++) executedEnableComponent[executedEnableComponentIndices[i]] = 0;
            for (int i = 0; i < executedDisableComponentIndices.Count; i++) executedDisableComponent[executedDisableComponentIndices[i]] = 0;
            for (int i = 0; i < executedAddTagIndices.Count; i++) executedAddTag[executedAddTagIndices[i]] = 0;
            for (int i = 0; i < executedRemoveTagIndices.Count; i++) executedRemoveTag[executedRemoveTagIndices[i]] = 0;

            executedAddComponentIndices.Clear();
            executedRemoveComponentIndices.Clear();
            executedEnableComponentIndices.Clear();
            executedDisableComponentIndices.Clear();
            executedAddTagIndices.Clear();
            executedRemoveTagIndices.Clear();
        }

        // -------------------------
        // ExecutePendingParallel: process thread buffers deterministically and batch per type
        // usedThreadCount = number of threads that wrote (<= maxParallelWriters)
        // -------------------------
        public static void ExecutePendingParallel(World world, int usedThreadCount)
        {
            if (threadOpBuffers == null) throw new InvalidOperationException("ParallelWriter not initialized.");

            for (int t = 0; t < usedThreadCount; t++)
            {
                var span = threadOpBuffers[t].AsReadOnlySpan();
                for (int i = 0; i < span.Length; i++)
                {
                    ref readonly var op = ref span[i];
                    switch (op.Type)
                    {
                        case OpType.AddComponent:
                            if (executedAddComponent[op.TypeId] == 0)
                            {
                                executedAddComponent[op.TypeId] = 1;
                                executedAddComponentIndices.Add() = op.TypeId;
                                addComponentExec[op.TypeId](world);
                            }
                            break;
                        case OpType.RemoveComponent:
                            if (executedRemoveComponent[op.TypeId] == 0)
                            {
                                executedRemoveComponent[op.TypeId] = 1;
                                executedRemoveComponentIndices.Add() = op.TypeId;
                                removeComponentExec[op.TypeId](world);
                            }
                            break;
                        case OpType.EnableComponent:
                            if (executedEnableComponent[op.TypeId] == 0)
                            {
                                executedEnableComponent[op.TypeId] = 1;
                                executedEnableComponentIndices.Add() = op.TypeId;
                                enableComponentExec[op.TypeId](world);
                            }
                            break;
                        case OpType.DisableComponent:
                            if (executedDisableComponent[op.TypeId] == 0)
                            {
                                executedDisableComponent[op.TypeId] = 1;
                                executedDisableComponentIndices.Add() = op.TypeId;
                                disableComponentExec[op.TypeId](world);
                            }
                            break;
                        case OpType.AddTag:
                            if (executedAddTag[op.TypeId] == 0)
                            {
                                executedAddTag[op.TypeId] = 1;
                                executedAddTagIndices.Add() = op.TypeId;
                                addTagExec[op.TypeId](world);
                            }
                            break;
                        case OpType.RemoveTag:
                            if (executedRemoveTag[op.TypeId] == 0)
                            {
                                executedRemoveTag[op.TypeId] = 1;
                                executedRemoveTagIndices.Add() = op.TypeId;
                                removeTagExec[op.TypeId](world);
                            }
                            break;
                    }
                }
                threadOpBuffers[t].Clear();
            }

            // also drain single-thread opQueue if used
            if (opQueue.Count > 0) ExecutePending(world);

            // reset flags (same as ExecutePending - executed*Indices arrays already hold entries)
            // ExecutePending cleared them already; if we only used parallel buffers then clear now:
            for (int i = 0; i < executedAddComponentIndices.Count; i++) executedAddComponent[executedAddComponentIndices[i]] = 0;
            for (int i = 0; i < executedRemoveComponentIndices.Count; i++) executedRemoveComponent[executedRemoveComponentIndices[i]] = 0;
            for (int i = 0; i < executedEnableComponentIndices.Count; i++) executedEnableComponent[executedEnableComponentIndices[i]] = 0;
            for (int i = 0; i < executedDisableComponentIndices.Count; i++) executedDisableComponent[executedDisableComponentIndices[i]] = 0;
            for (int i = 0; i < executedAddTagIndices.Count; i++) executedAddTag[executedAddTagIndices[i]] = 0;
            for (int i = 0; i < executedRemoveTagIndices.Count; i++) executedRemoveTag[executedRemoveTagIndices[i]] = 0;

            executedAddComponentIndices.Clear();
            executedRemoveComponentIndices.Clear();
            executedEnableComponentIndices.Clear();
            executedDisableComponentIndices.Clear();
            executedAddTagIndices.Clear();
            executedRemoveTagIndices.Clear();
        }

        // // -------------------------
        // // Optional: full collapse (allocating) - returns final ops keyed by (entity,type) using last-op semantics
        // // Use only if you understand this loses inter-entity ordering and focuses on final state.
        // // -------------------------
        // public static void FullCollapse()
        // {
        //     // WARNING: allocates. Use sparingly or use a pooled dictionary.
        //     var map = new Dictionary<ulong, OpType>(opQueue.Count * 2);
        //     var span = opQueue.AsReadOnlySpan();
        //     for (int i = 0; i < span.Length; i++)
        //     {
        //         ref readonly var op = ref span[i];
        //         // key: (entity.id << 32) | typeId (assuming typeId fits 32 bits)
        //         ulong key = ((ulong)op.Entity.id << 32) | (uint)op.TypeId;
        //         map[key] = op.Type; // last op wins
        //     }
        //
        //     // rebuild opQueue from dictionary entries (deterministic: iterate keys in sorted order)
        //     opQueue.Clear();
        //     var keys = new List<ulong>(map.Keys);
        //     keys.Sort();
        //     foreach (var k in keys)
        //     {
        //         var typeId = (int)(k & 0xffffffff);
        //         var entityId = new EntityId((int)(k >> 32), 0); // generation lost here — not safe unless you encode generation too!
        //         // WARNING: this naive version loses generation info; a production version must encode generation in key
        //         // I'm leaving this here as an illustration — use with caution.
        //         var t = map[k];
        //         var op = new QueuedOp { Type = t, TypeId = typeId, Entity = entityId };
        //         opQueue.Add() = op;
        //     }
        // }
    }
}
