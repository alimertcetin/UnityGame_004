using System;
using System.Collections.Generic;
using XIV.Core.Collections;
using XIV.Core.Utils;

namespace XIV.Ecs
{
    public class World
    {
        public EntityDataList entityDataList;
        public ArchetypeMap archetypeMap;
        public List<Query> queries;
        public List<Filter> filters;

        public DynamicArray<EntityId> delayedComponentOperationEntityIds;
        public DynamicArray<Action<EntityId>> delayedComponentOperations;
        public DynamicArray<TagOperation> delayedTagOperations;
        public DynamicArray<DestroyOperation> destroyedEntities;

        public World(int entityCapacity = 64)
        {
            TypeManager typeManager = new TypeManager("XIV", "Assembly-CSharp");
            ComponentIdManager.Init(typeManager);
            TagIdManager.Init(typeManager);
            Filter.Init();
            
            entityDataList = new EntityDataList(entityCapacity, entityCapacity / 4);
            archetypeMap = new ArchetypeMap();
            queries = new List<Query>();
            filters = new List<Filter>();

            var capacity = entityCapacity / 2;
            delayedComponentOperations = new DynamicArray<Action<EntityId>>(capacity);
            delayedComponentOperationEntityIds = new DynamicArray<EntityId>(capacity);
            delayedTagOperations = new DynamicArray<TagOperation>(capacity);
            destroyedEntities = new DynamicArray<DestroyOperation>(capacity);
        }

        public Entity NewEntity()
        {
            ref var entityData = ref entityDataList.Add(out var entityIdx);
            return new Entity(this, entityIdx, entityData.generationId);
        }

        public void UnlockComponentOperation()
        {
            HandleDestroyOperations();
            HandleTagOperations();

            // Update Data
            for (int i = 0; i < delayedComponentOperations.Count; i++)
            {
                ref var entityId = ref delayedComponentOperationEntityIds[i];
                ref var operation = ref delayedComponentOperations[i];
                operation.Invoke(entityId);
            }

            delayedTagOperations.Clear();
            delayedComponentOperations.Clear();
            delayedComponentOperationEntityIds.Clear();
        }

        void HandleDestroyOperations()
        {
            for (int i = 0; i < destroyedEntities.Count; i++)
            {
                ref var destroyOperation = ref destroyedEntities[i];
                ref var entityData = ref entityDataList[destroyOperation.entityId.id];
                entityData.archetype.RemoveEntityAndComponents(entityData.indexInArchetype, entityDataList);

                entityDataList.Free(destroyOperation.entityId.id);
            }
            destroyedEntities.Clear();
        }

        void HandleTagOperations()
        {
            using var dispose = ArrayUtils.GetBuffer(out EntityId[] buffer, delayedComponentOperations.Count);
            int bufferTrack = 0;
            for (int i = 0; i < delayedTagOperations.Count; i++)
            {
                ref var operation = ref delayedTagOperations[i];
                ref var entityData = ref entityDataList[operation.entityId.id];

                if (operation.entityId.generation != entityData.generationId)
                {
                    // Operation on destroyed entity
                    continue;
                }

                if (operation.tagId >= 0)
                {
                    // Add
                    entityData.tagBitSet.SetBit1(operation.tagId);
                }
                else
                {
                    // Remove
                    int tagId = -(operation.tagId + 1);
                    entityData.tagBitSet.SetBit0(tagId);
                }

                buffer[bufferTrack++] = operation.entityId;
            }
            
            // Update archetypes
            for (int i = 0; i < bufferTrack; i++)
            {
                var entityId = buffer[i];
                ref var entityData = ref entityDataList[entityId.id];
                var newArchetype = archetypeMap.GetArchetype(entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

                if (newArchetype != entityData.archetype)
                {
                    archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);
                }

                if (newArchetypeGenerated)
                {
                    UpdateQueries(newArchetype);
                }
            }
        }

        public void DestroyEntity(EntityId entityId)
        {
            // Debug.Assert(IsEntityAlive(entityId));
            ref var entityData = ref entityDataList[entityId.id];
            var archetype = entityData.archetype;
            if (archetype == null)
            {
                entityDataList.Free(entityId.id);
                return;
            }

            if (archetype.lockCounter > 0)
            {
                destroyedEntities.Add() = new DestroyOperation()
                {
                    entityId = entityId
                };
                return;
            }

            archetype.RemoveEntityAndComponents(entityData.indexInArchetype, entityDataList);

            entityDataList.Free(entityId.id);

        }

        public void AddComponent<T>(EntityId entityId, T componentValue) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entityId));
            // Debug.Assert(!HasDisabledComponent<T>(entityId));

            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                delayedComponentOperationEntityIds.Add() = entityId;
                delayedComponentOperations.Add() = (EntityId entityId) =>
                {
                    AddCompFunc<T>(entityId, componentValue, entityDataList);
                };
                return;
            }

            AddCompFunc<T>(entityId, componentValue, entityDataList);

            void AddCompFunc<T1>(EntityId entityId1, T component, EntityDataList entityDataList) where T1 : struct, IComponent
            {
                ref var entityData = ref entityDataList[entityId1.id];
                var componentId = ComponentIdManager.GetComponentId<T>();
                if (entityData.componentBitSet.IsBit1(componentId))
                {
                    // Already has the component
                    entityData.archetype.GetComponentPool<T>().Set(entityData.indexInArchetype, in component);
                    return;
                }
                entityData.componentBitSet.SetBit1(componentId);
                var newArchetype = archetypeMap.GetArchetype(entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);
                archetypeMap.ChangeArchetype(this, entityId1, entityDataList, newArchetype);
                archetypeMap.SetNewComponent<T>(entityData, in component);
                if (newArchetypeGenerated)
                {
                    UpdateQueries(newArchetype);
                }
            }
        }

        public void AddTag<T>(EntityId entityId) where T : struct, ITag
        {
            // Debug.Assert(IsEntityAlive(entityId));

            int newTagId = TagIdManager.GetTagId<T>();
            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                delayedTagOperations.Add() = new TagOperation()
                {
                    tagId = newTagId,
                    entityId = entityId
                };
                return;
            }

            if (entityData.tagBitSet.IsBit1(newTagId))
            {
                // Already has the tag
                return;
            }

            entityData.tagBitSet.SetBit1(newTagId);

            var newArchetype = archetypeMap.GetArchetype(
                entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

            archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
            }
        }
        
        public void RemoveTag<T>(EntityId entityId) where T : struct, ITag
        {
            RemoveTag(entityId, TagIdManager.GetTagId<T>());
        }
        
        public void RemoveTag(EntityId entityId, int removedTagId)
        {
            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                delayedTagOperations.Add() = new TagOperation()
                {
                    tagId = -(removedTagId + 1),
                    entityId = entityId
                };
                return;
            }


            if (entityData.tagBitSet.IsBit1(removedTagId) == false)
            {
                // Already removed
                return;
            }

            entityData.tagBitSet.SetBit0(removedTagId);

            var newArchetype = archetypeMap.GetArchetype(
                entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

            archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
            }
        }
        
        public void RemoveComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            RemoveComponent(entityId, ComponentIdManager.GetComponentId<T>());
        }
        
        public void RemoveComponent(EntityId entityId, int componentId)
        {
            // Debug.Assert(IsEntityAlive(entityId));

            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                delayedComponentOperationEntityIds.Add() = entityId;
                delayedComponentOperations.Add() = RemoveCompFunc;
                return;
            }

            RemoveCompFunc(entityId);

            void RemoveCompFunc(EntityId entityId)
            {
                ref var entityData = ref entityDataList[entityId.id];
                int removedComponentId = componentId;

                if (!entityData.componentBitSet.IsBit1(removedComponentId))
                {
                    // Already removed
                    return;
                }

                entityData.componentBitSet.SetBit0(removedComponentId);

                var newArchetype = archetypeMap.GetArchetype(
                    entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

                archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);

                if (newArchetypeGenerated)
                {
                    UpdateQueries(newArchetype);
                }
            }
        }

        public bool HasComponent<T>(EntityId entity) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entity));
            ref var entityData = ref entityDataList[entity.id];
            return entityData.componentBitSet.IsBit1(ComponentIdManager.GetComponentId<T>());
        }


        public bool HasDisabledComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            ref var entityData = ref entityDataList[entityId.id];

            for (int i = 0; i < entityData.disabledComponents.Count; i++)
            {
                if (entityData.disabledComponents[i].componentId == componentId)
                {
                    return true;
                }
            }

            return false;
        }

        public void DisableComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entityId));
            // Debug.Assert(HasComponent<T>(entityId));
            // Debug.Assert(!HasDisabledComponent<T>(entityId));

            ref var entityData = ref entityDataList[entityId.id];
            var componentData = GetComponent<T>(entityId);
            int componentId = ComponentIdManager.GetComponentId<T>();
            entityData.disabledComponents.Add() = new DisabledComponent
            {
                componentData = componentData,
                componentId = componentId
            };

            RemoveComponent<T>(entityId);
        }

        public void EnableComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entityId));
            // Debug.Assert(!HasComponent<T>(entityId));

            int componentId = ComponentIdManager.GetComponentId<T>();
            ref var entityData = ref entityDataList[entityId.id];

            object componentData = null;

            for (int i = 0; i < entityData.disabledComponents.Count; i++)
            {
                if (entityData.disabledComponents[i].componentId == componentId)
                {
                    componentData = entityData.disabledComponents[i].componentData;
                    entityData.disabledComponents[i] = entityData.disabledComponents.RemoveLast();
                    break;
                }
            }

            // Debug.Assert(componentData != null);
            AddComponent(entityId, (T)componentData);
        }



        public bool HasTag<T>(EntityId entity) where T : struct, ITag
        {
            // Debug.Assert(IsEntityAlive(entity));
            ref var entityData = ref entityDataList[entity.id];
            return entityData.tagBitSet.IsBit1(TagIdManager.GetTagId<T>());
        }

        public ref T GetComponent<T>(EntityId entity) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entity));
            // Debug.Assert(HasComponent<T>(entity));

            ref var entityData = ref entityDataList[entity.id];
            int componentId = ComponentIdManager.GetComponentId<T>();
            return ref ((ComponentPool<T>)entityData.archetype.GetComponentPool(componentId)).components[entityData.indexInArchetype];
        }

        public Archetype GetArchetype(EntityId entity)
        {
            // Debug.Assert(IsEntityAlive(entity));
            ref var entityData = ref entityDataList[entity.id];
            return entityData.archetype;
        }

        public bool IsEntityAlive(EntityId entity)
        {
            return entityDataList[entity.id].generationId == entity.generation;
        }

        public void UpdateQueries(Archetype newArchetype)
        {
            foreach (var query in queries)
            {
                if (query.componentMask.IsCompatible(newArchetype.GetComponentBitSet(), newArchetype.GetTagBitset()))
                {
                    query.archetypes.Add(newArchetype);
                }
            }
        }

        public void SetCustomReset<T>(CustomReset<T> customReset) where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            // Debug.Assert(archetypeMap.customResetMap.Get(componentId) == null);
            archetypeMap.customResetMap[componentId] = customReset;
        }

        public void SetCustomAssign<T>(CustomAssign<T> customAssign) where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            // Debug.Assert(archetypeMap.customAssignMap.Get(componentId) == null);
            archetypeMap.customAssignMap[componentId] = customAssign;
        }

        public void CheckLeakedEntities()
        {
            for (int i = 0; i < entityDataList.items.Count; i++)
            {
                ref var entityData = ref entityDataList[i];
                if (entityData.generationId < 0)
                {
                    continue;
                }

                // archetype == null means new entity created but no component added afterwards
                // && archetype without any component or tag
                if (entityData.archetype == null ||
                    (entityData.archetype.componentIds.Length == 0 && entityData.tagBitSet.GetSetBitCount() == 0))
                {
                    // Debug.LogError("Leaked Entity");
                }
            }
        }

        public int GetNumberOfEntities()
        {
            int nEntity = 0;
            for (int i = 0; i < entityDataList.items.Count; i++)
            {
                ref var entityData = ref entityDataList[i];
                if (entityData.generationId >= 0)
                {
                    nEntity++;
                }
            }

            return nEntity;
        }
    }
    //
    // public class ComponentOperationManager
    // {
    //     public List<Action<Entity>> componentOperations
    // }
    //
    // public struct OperationWrapper<T> where T : struct, IComponent
    // {
    //     public T componentValue;
    //
    //     public void AddComponent(Entity entity)
    //     {
    //         if (entity.IsAlive() == false) return;
    //
    //         var entityDataList = entity.world.entityDataList;
    //         var archetypeMap = entity.world.archetypeMap;
    //         var entityId = entity.entityId;
    //         
    //         ref var entityData = ref entityDataList[entityId.id];
    //         int newComponentId = ComponentIdManager.GetComponentId<T>();
    //         if (entityData.componentBitSet.IsBit1(newComponentId))
    //         {
    //             // Already has the component
    //             entityData.archetype.GetComponentPool<T>().Set(entityData.indexInArchetype, in componentValue);
    //             return;
    //         }
    //
    //
    //         entityData.componentBitSet.SetBit1(newComponentId);
    //
    //         var newArchetype = archetypeMap.GetArchetype(
    //             entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);
    //
    //         archetypeMap.ChangeArchetype(entity.world, entityId, entityDataList, newArchetype);
    //
    //         // entityData.archetype.GetComponentPool(newComponentId).SetNewComponent(entityData.indexInArchetype, componentValue);
    //         entityData.archetype.GetComponentPool<T>().SetNewComponent(entityData.indexInArchetype, in componentValue);
    //
    //         if (newArchetypeGenerated)
    //         {
    //             entity.world.UpdateQueries(newArchetype);
    //         }
    //     }
    // }
}