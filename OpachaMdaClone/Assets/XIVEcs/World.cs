using System;
using System.Collections.Generic;
using XIV.Core.Collections;

namespace XIV.Ecs
{
    public class World
    {
        public EntityDataList entityDataList;
        public ArchetypeMap archetypeMap;
        public List<Query> queries;
        public List<Filter> filters;

        public DynamicArray<ComponentOperation> delayedComponentOperations;
        public DynamicArray<TagOperation> delayedTagOperations;
        public DynamicArray<DestroyOperation> destroyedEntities;
        public DynamicArray<EntityId> changedEntities;

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
            delayedComponentOperations = new DynamicArray<ComponentOperation>(capacity);
            delayedTagOperations = new DynamicArray<TagOperation>(capacity);
            destroyedEntities = new DynamicArray<DestroyOperation>(capacity);
            changedEntities = new DynamicArray<EntityId>(capacity);
        }

        public Entity NewEntity()
        {
            ref var entityData = ref entityDataList.Add(out var entityIdx);
            return new Entity(this, entityIdx, entityData.generationId);
        }

        /// Only use for entity creation 
        public void AddComponents(EntityId id, IComponent[] componentValues)
        {
            ref var entityData = ref entityDataList[id.id];
            // Debug.Assert(entityData.archetype == null);

            foreach (var componentValue in componentValues)
            {
                int componentId = ComponentIdManager.GetComponentId(componentValue.GetType());
                entityData.componentBitSet.SetBit1(componentId);
            }

            var newArchetype = archetypeMap.GetArchetype(entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);
            archetypeMap.ChangeArchetype(this, id, entityDataList, newArchetype);

            // foreach (var componentValue in componentValues)
            // {
            //     int componentId = ComponentIdManager.GetComponentId(componentValue.GetType());
            //     entityData.archetype.GetComponentPool(componentId).SetNewComponent(entityData.indexInArchetype, componentValue);
            // }
            foreach (var componentValue in componentValues)
            {
                int componentId = ComponentIdManager.GetComponentId(componentValue.GetType());
                archetypeMap.InvokeBoxedSetNew(entityData.archetype, componentId, entityData.indexInArchetype, componentValue);
            }

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
            }
        }

        /// Only use for entity creation 
        public void AddComponents(EntityId id, object[] componentValues, int[] componentIds, Archetype archetype)
        {
            ref var entityData = ref entityDataList[id.id];
            // Debug.Assert(entityData.archetype == null);

            for (int i = 0; i < componentIds.Length; i++)
            {
                int componentId = componentIds[i];
                entityData.componentBitSet.SetBit1(componentId);
            }

            archetypeMap.ChangeArchetype(this, id, entityDataList, archetype);

            // for (int i = 0; i < componentIds.Length; i++)
            // {
            //     int componentId = componentIds[i];
            //     entityData.archetype.GetComponentPool(componentId).SetNewComponent(entityData.indexInArchetype, componentValues[i]);
            // }
            for (int i = 0; i < componentIds.Length; i++)
            {
                int componentId = componentIds[i];
                archetypeMap.InvokeBoxedSetNew(entityData.archetype, componentId, entityData.indexInArchetype, componentValues[i]);
            }

        }

        /// Only use for entity creation 
        public void AddComponentsAndTags(EntityId id,
            object[] componentValues, int[] componentIds,
            int[] tagIds)
        {
            ref var entityData = ref entityDataList[id.id];
            // Debug.Assert(entityData.archetype == null);

            for (int i = 0; i < componentIds.Length; i++)
            {
                try
                {
                    entityData.componentBitSet.SetBit1(componentIds[i]);
                }
                catch (Exception e)
                {
                    var str = "";
                    foreach (var componentValue in componentValues) str += componentValue.ToString() + ", ";
                    UnityEngine.Debug.LogError($"EntityId: {id}, ComponentValues: {str}, Exception: {e}");
                }
            }

            for (int i = 0; i < tagIds.Length; i++)
            {
                entityData.tagBitSet.SetBit1(tagIds[i]);
            }

            var newArchetype = archetypeMap.GetArchetype(entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);
            archetypeMap.ChangeArchetype(this, id, entityDataList, newArchetype);

            for (int i = 0; i < componentValues.Length; i++)
            {
                // entityData.archetype.GetComponentPool(componentIds[i])
                //     .SetNewComponent(entityData.indexInArchetype, componentValues[i]);
                archetypeMap.InvokeBoxedSetNew(entityData.archetype, componentIds[i], entityData.indexInArchetype, componentValues[i]);
            }

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
            }
        }


        public void RemoveComponents(EntityId entityId, Type[] componentTypes)
        {
            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype.lockCounter > 0)
            {
                foreach (var componentType in componentTypes)
                {
                    delayedComponentOperations.Add() = new ComponentOperation
                    {
                        componentId = ComponentIdManager.GetComponentId(componentType),
                        componentValue = null,
                        entityId = entityId
                    };
                }

                return;
            }


            foreach (var componentType in componentTypes)
            {
                int removedComponentId = ComponentIdManager.GetComponentId(componentType);
                entityData.componentBitSet.SetBit0(removedComponentId);
            }

            var newArchetype = archetypeMap.GetArchetype(
                entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

            archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
            }
        }

        public void UnlockComponentOperation()
        {
            for (int i = 0; i < destroyedEntities.Count; i++)
            {
                ref var destroyOperation = ref destroyedEntities[i];
                ref var entityData = ref entityDataList[destroyOperation.entityId.id];

                var archetype = entityData.archetype;
                int archetypeIdx = entityData.indexInArchetype;

                // Swap and Remove
                // TODO change generation entityId immediately after entity.Destroy() call so we can detect this error immediately
                // Debug.Assert(archetype != null, "You might be destroy an entity twice");
                archetype.RemoveEntity(archetypeIdx);

                for (int c = 0; c < archetype.componentPools.Length; c++)
                {
                    archetype.GetPoolByIndex(c).SwapRemoveComponentAtIndex(archetypeIdx);
                }

                if (archetype.entities.Count != archetypeIdx)
                {
                    var effectedEntity = archetype.entities[archetypeIdx];
                    entityDataList[effectedEntity.entityId.id].indexInArchetype = archetypeIdx;
                }

                entityDataList.Free(destroyOperation.entityId.id);
            }

            changedEntities.Clear();
            for (int i = 0; i < delayedComponentOperations.Count; i++)
            {
                ref var operation = ref delayedComponentOperations[i];
                ref var entityData = ref entityDataList[operation.entityId.id];

                if (operation.entityId.generation != entityData.generationId)
                {
                    // Operation on destroyed entity
                    continue;
                }

                int componentId = operation.componentId;

                if (operation.componentValue != null)
                {
                    // Add
                    entityData.componentBitSet.SetBit1(componentId);
                }
                else if (entityData.componentBitSet.IsBit1(componentId))
                {
                    // Remove
                    entityData.componentBitSet.SetBit0(componentId);
                }

                changedEntities.Add() = operation.entityId;
            }

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

                changedEntities.Add() = operation.entityId;
            }



            // Update archetypes
            for (int i = 0; i < changedEntities.Count; i++)
            {
                var entityId = changedEntities[i];
                ref var entityData = ref entityDataList[entityId.id];

                var newArchetype = archetypeMap.GetArchetype(
                    entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

                if (newArchetype != entityData.archetype)
                {
                    archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);
                }

                if (newArchetypeGenerated)
                {
                    UpdateQueries(newArchetype);
                }
            }

            // Update Data
            for (int i = 0; i < delayedComponentOperations.Count; i++)
            {
                ref var operation = ref delayedComponentOperations[i];
                ref var entityData = ref entityDataList[operation.entityId.id];
                int componentId = operation.componentId;

                // if (operation.componentValue != null)
                // {
                //     // Add
                //     if (entityData.componentBitSet.IsBit1(componentId))
                //     {
                //         entityData.archetype.GetComponentPool(componentId).SetNewComponent(entityData.indexInArchetype, operation.componentValue);
                //     }
                // }
                if (operation.componentValue != null)
                {
                    // Add
                    if (entityData.componentBitSet.IsBit1(componentId))
                    {
                        archetypeMap.InvokeBoxedSetNew(entityData.archetype, componentId, entityData.indexInArchetype, operation.componentValue);
                    }
                }

            }

            delayedTagOperations.Clear();
            delayedComponentOperations.Clear();
            destroyedEntities.Clear();
        }

        public void DestroyEntity(EntityId entityId)
        {
            // Debug.Assert(IsEntityAlive(entityId));

            // Free entity data for IsEntityAlive to work
            // so we can detect wrong component operations
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


            // Swap and Remove
            int archetypeIdx = entityData.indexInArchetype;
            archetype.RemoveEntity(archetypeIdx);

            for (int c = 0; c < archetype.componentPools.Length; c++)
            {
                archetype.GetPoolByIndex(c).SwapRemoveComponentAtIndex(archetypeIdx);
            }

            if (archetype.entities.Count != archetypeIdx)
            {
                var effectedEntity = archetype.entities[archetypeIdx];
                entityDataList[effectedEntity.entityId.id].indexInArchetype = archetypeIdx;
            }

            entityDataList.Free(entityId.id);

        }

        public void AddComponent<T>(EntityId entityId, T componentValue) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entityId));
            // Debug.Assert(!HasDisabledComponent<T>(entityId));

            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                delayedComponentOperations.Add() = new ComponentOperation
                {
                    componentId = ComponentIdManager.GetComponentId<T>(),
                    componentValue = componentValue,
                    entityId = new EntityId(entityId.id, entityId.generation)
                };
                return;
            }

            int newComponentId = ComponentIdManager.GetComponentId<T>();

            // if (entityData.componentBitSet.IsBit1(newComponentId))
            // {
            //     // Already has the component
            //     entityData.archetype.GetComponentPool(newComponentId).Set(entityData.indexInArchetype, componentValue);
            //     return;
            // }
            if (entityData.componentBitSet.IsBit1(newComponentId))
            {
                // Already has the component
                entityData.archetype.GetComponentPool<T>().Set(entityData.indexInArchetype, in componentValue);
                return;
            }


            entityData.componentBitSet.SetBit1(newComponentId);

            var newArchetype = archetypeMap.GetArchetype(
                entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

            archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);

            // entityData.archetype.GetComponentPool(newComponentId).SetNewComponent(entityData.indexInArchetype, componentValue);
            entityData.archetype.GetComponentPool<T>().SetNewComponent(entityData.indexInArchetype, in componentValue);

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
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
            // Debug.Assert(IsEntityAlive(entityId));

            int removedTagId = TagIdManager.GetTagId<T>();
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

        public void RemoveTags(EntityId entityId, Type[] tagTypes)
        {
            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                foreach (var tagType in tagTypes)
                {
                    delayedTagOperations.Add() = new TagOperation()
                    {
                        tagId = -(TagIdManager.GetTagId(tagType) + 1),
                        entityId = entityId
                    };
                }

                return;
            }


            foreach (var tagType in tagTypes)
            {
                int removedComponentId = TagIdManager.GetTagId(tagType);
                entityData.tagBitSet.SetBit0(removedComponentId);
            }

            var newArchetype = archetypeMap.GetArchetype(
                entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

            archetypeMap.ChangeArchetype(this, entityId, entityDataList, newArchetype);

            if (newArchetypeGenerated)
            {
                UpdateQueries(newArchetype);
            }
        }

        public void AddTags(EntityId entityId, Type[] tagTypes)
        {
            ref var entityData = ref entityDataList[entityId.id];
            // Debug.Assert(entityData.archetype == null); // TODO only for creation?

            // if (entityData.archetype.lockCounter > 0)
            // {
            //     foreach (var tagType in tagTypes)
            //     {
            //         delayedTagOperations.Add() = new TagOperation()
            //         {
            //             tagId =  TagIdManager.GetTagId(tagType),
            //             entityId = entityId
            //         };
            //     }
            //     return;
            // }

            foreach (var tagType in tagTypes)
            {
                int componentId = TagIdManager.GetTagId(tagType);
                entityData.tagBitSet.SetBit1(componentId);
            }

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
            // Debug.Assert(IsEntityAlive(entityId));
            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                delayedComponentOperations.Add() = new ComponentOperation()
                {
                    componentId = ComponentIdManager.GetComponentId<T>(),
                    entityId = new EntityId(entityId.id, entityId.generation),
                    componentValue = null
                };
                return;
            }

            int removedComponentId = ComponentIdManager.GetComponentId<T>();

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

        public Archetype CreateArchetype(Bitset componentBitSet, Bitset tagBitSet)
        {
            var archetype = archetypeMap.GetArchetype(componentBitSet, tagBitSet, out var newArchetypeGenerated);
            if (newArchetypeGenerated)
            {
                UpdateQueries(archetype);
            }

            return archetype;
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
}