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

        public DynamicArray<DestroyOperation> destroyedEntities;

        public World(int entityCapacity = 64)
        {
            TypeManager typeManager = new TypeManager("XIV", "Assembly-CSharp");
            ComponentIdManager.Init(typeManager);
            TagIdManager.Init(typeManager);
            ComponentOperationIndex.Init(typeManager);
            Filter.Init();
            
            entityDataList = new EntityDataList(entityCapacity, entityCapacity / 4);
            archetypeMap = new ArchetypeMap();
            queries = new List<Query>();
            filters = new List<Filter>();

            var capacity = entityCapacity / 2;
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
            ComponentOperationIndex.ExecuteAddTagActions(this);
            ComponentOperationIndex.ExecuteRemoveTagActions(this);
            ComponentOperationIndex.ExecuteAddComponentActions(this);
            ComponentOperationIndex.ExecuteRemoveComponentActions(this);
        }

        void HandleDestroyOperations()
        {
            for (int i = 0; i < destroyedEntities.Count; i++)
            {
                ref var destroyOperation = ref destroyedEntities[i];
                ref var entityData = ref entityDataList[destroyOperation.entityId.id];
                entityData.archetype.RemoveEntityAndComponents(entityData.archetypeIndex, entityDataList);

                entityDataList.Free(destroyOperation.entityId.id);
            }
            destroyedEntities.Clear();
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

            archetype.RemoveEntityAndComponents(entityData.archetypeIndex, entityDataList);

            entityDataList.Free(entityId.id);

        }

        public void AddComponent<T>(EntityId entityId, T componentValue) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entityId));
            // Debug.Assert(!HasDisabledComponent<T>(entityId));

            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.AddComponent(entityId, componentValue);
                return;
            }

            ComponentOperationIndex.AddComponent(entityId, componentValue);
            ComponentOperationIndex.ExecuteAddComponentActions(this);
        }

        public void AddTag<T>(EntityId entityId) where T : struct, ITag
        {
            // Debug.Assert(IsEntityAlive(entityId));
            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.AddTag<T>(entityId);
                return;
            }

            ComponentOperationIndex.AddTag<T>(entityId);
            ComponentOperationIndex.ExecuteAddTagActions(this);
        }
        
        public void RemoveTag<T>(EntityId entityId) where T : struct, ITag
        {
            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.RemoveTag<T>(entityId);
                return;
            }
            ComponentOperationIndex.RemoveTag<T>(entityId);
            ComponentOperationIndex.ExecuteRemoveTagActions(this);
        }
        
        public void RemoveTag(EntityId entityId, int tagId)
        {
            ref var entityData = ref entityDataList[entityId.id];

            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.RemoveTag(entityId, tagId);
                return;
            }
            ComponentOperationIndex.RemoveTag(entityId, tagId);
            ComponentOperationIndex.ExecuteRemoveTagActions(this);
        }
        
        public void RemoveComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.RemoveComponent<T>(entityId);
                return;
            }

            ComponentOperationIndex.RemoveComponent<T>(entityId);
            ComponentOperationIndex.ExecuteRemoveComponentActions(this);
        }
        
        public void RemoveComponent(EntityId entityId, int componentId)
        {
            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.RemoveComponent(entityId, componentId);
                return;
            }

            ComponentOperationIndex.RemoveComponent(entityId, componentId);
            ComponentOperationIndex.ExecuteRemoveComponentActions(this);
        }

        public bool HasComponent<T>(EntityId entity) where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            ref var entityData = ref entityDataList[entity.id];
            return entityData.componentBitset.IsBit1(componentId);
        }

        public bool HasDisabledComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            ref var entityData = ref entityDataList[entityId.id];
            return entityData.disabledComponentBitset.IsBit1(componentId);
        }

        public void EnableComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.EnableComponent<T>(entityId);
                return;
            }

            ComponentOperationIndex.EnableComponent<T>(entityId);
            ComponentOperationIndex.ExecuteEnableComponentActions(this);
        }

        public void DisableComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            ref var entityData = ref entityDataList[entityId.id];
            if (entityData.archetype != null && entityData.archetype.lockCounter > 0)
            {
                ComponentOperationIndex.DisableComponent<T>(this, entityId);
                return;
            }

            ComponentOperationIndex.DisableComponent<T>(this, entityId);
            ComponentOperationIndex.ExecuteDisableComponentActions(this);
        }
        
        public bool HasTag<T>(EntityId entity) where T : struct, ITag
        {
            // Debug.Assert(IsEntityAlive(entity));
            ref var entityData = ref entityDataList[entity.id];
            return entityData.tagBitset.IsBit1(TagIdManager.GetTagId<T>());
        }

        public ref T GetComponent<T>(EntityId entity) where T : struct, IComponent
        {
            // Debug.Assert(IsEntityAlive(entity));
            if (HasComponent<T>(entity) == false) throw new Exception($"Entity does not have component {typeof(T).Name} - Entity: {entity}");

            ref var entityData = ref entityDataList[entity.id];
            return ref entityData.archetype.GetComponent<T>(entityData.archetypeIndex);
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
            archetypeMap.customResetMap[componentId] = customReset;
        }

        public void SetCustomAssign<T>(CustomAssign<T> customAssign) where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
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
                    (entityData.archetype.componentIds.Length == 0 && entityData.tagBitset.GetSetBitCount() == 0))
                {
                    // Debug.LogError("Leaked Entity");
                }
            }
        }

        public int GetNumberOfEntities()
        {
            return entityDataList.entityCount;
        }
    }
}