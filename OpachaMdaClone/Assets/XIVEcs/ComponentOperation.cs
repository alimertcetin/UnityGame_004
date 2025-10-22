using System;
using System.Collections.Generic;
using XIV.Core.Collections;

namespace XIV.Ecs
{
    public struct ComponentOperation
    {
        public EntityId entityId;
        public object componentValue;
        public int componentId;
    }

    public static class ComponentOperationIndex
    {
        static List<Action<World, EntityData, ArchetypeMap, EntityId, EntityDataList>> actions = new();

        public static void AddComponent<T>(EntityId entityId, T componentValue) where T : struct, IComponent
        {
            ComponentOperations<T>.AddComponent(entityId, componentValue);
        }

        public static void RemoveComponent<T>(EntityId entityId)
        {
            
        }

        public static void AddComponentOperationAction(Action<World, EntityData, ArchetypeMap, EntityId, EntityDataList> action)
        {
            actions.Add(action);
        }
        
        public static void Execute(Type componentType, World world, EntityData entityData, ArchetypeMap archetypeMap, EntityId entityId, EntityDataList entityDataList)
        {
            foreach (var action in actions)
            {
                action(world, entityData, archetypeMap, entityId, entityDataList);
            }
        }
    }

    public static class ComponentOperations<T> where T : struct, IComponent
    {
        public static DynamicArray<EntityId> entityIds = new(64);
        public static DynamicArray<T> componentValues = new(64);
        public static DynamicArray<int> componentIds = new(64);
        static Type componentType;
        static int newComponentId;
        
        public static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            ComponentOperationIndex.AddComponentOperationAction(Execute);
            componentType = typeof(T);
            newComponentId = ComponentIdManager.GetComponentId(componentType);
            initialized = true;
        }

        public static void AddComponent(EntityId entityId, T componentValue)
        {
            Init();
            entityIds.Add() = entityId;
            componentValues.Add() = componentValue;
            componentIds.Add() = ComponentIdManager.GetComponentId<T>();
        }
        
        public static int GetIndexByComponentId(int componentId)
        {
            Init();
            int len = componentIds.Count;
            for (int i = 0; i < len; i++)
            {
                if (componentIds[i] == componentId) return i;
            }
            return -1;
        }

        public static int GetIndexByEntityId(EntityId entityId)
        {
            Init();
            return entityIds.Exists(p => p.id == entityId.id);
        }
        
        public static void Execute(World world, EntityData entityData, ArchetypeMap archetypeMap, EntityId entityId, EntityDataList entityDataList)
        {
            Init();
            var componentPool = (ComponentPool<T>)entityData.archetype.GetComponentPool(componentType);
            int len = componentIds.Count;
            for (int i = 0; i < len; i++)
            {
                if (entityData.componentBitSet.IsBit1(newComponentId))
                {
                    // Already has the component
                    componentPool.Set(entityData.indexInArchetype, componentValues[i]);
                    continue;
                }


                entityData.componentBitSet.SetBit1(newComponentId);

                var newArchetype = archetypeMap.GetArchetype(entityData.componentBitSet, entityData.tagBitSet, out var newArchetypeGenerated);

                archetypeMap.ChangeArchetype(world, entityId, entityDataList, newArchetype);

                // entityData.archetype.GetComponentPool(newComponentId).SetNewComponent(entityData.indexInArchetype, componentValue);
                componentPool.SetNewComponent(entityData.indexInArchetype, componentValues[i]);

                if (newArchetypeGenerated)
                {
                    world.UpdateQueries(newArchetype);
                }
            }
            Clear();
        }

        static void Clear()
        {
            entityIds.Clear();
            componentValues.Clear();
            componentIds.Clear();
        }
    }
}