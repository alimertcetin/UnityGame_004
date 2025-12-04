using System.Runtime.CompilerServices;
using XIV.Core.Collections;
using XIV.Core.Utils;

namespace XIV.Ecs
{
    public class AddComponentOperations<T> where T : struct, IComponent
    {
        static readonly int componentId = ComponentIdManager.GetComponentId<T>();
        static DynamicArray<EntityId> entityIds;
        static DynamicArray<T> componentValues;

        public static void Init()
        {
            ComponentOperationIndex.AddComponentAction(Execute, true);
            entityIds = new DynamicArray<EntityId>(64);
            componentValues = new DynamicArray<T>(64);
        }

        public static void AddComponent(EntityId entityId, in T componentValue)
        {
            entityIds.Add() = entityId;
            componentValues.Add() = componentValue;
        }

        public static void Execute(World world)
        {
            var entityDataList = world.entityDataList;
            var archetypeMap = world.archetypeMap;
            int len = entityIds.Count;
            for (int i = 0; i < len; i++)
            {
                var entityId = entityIds[i];
                var component = componentValues[i];
                ref var entityData = ref entityDataList[entityId.id];
                if (entityData.componentBitset.IsBit1(componentId))
                {
                    // Already has the component
                    entityData.archetype.GetComponentPool<T>().Set(entityData.archetypeIndex, in component);
                    continue;
                }
                entityData.componentBitset.SetBit1(componentId);
                
                var newArchetype = archetypeMap.GetArchetype(entityData.componentBitset, entityData.tagBitset, out var newArchetypeGenerated);
                archetypeMap.ChangeArchetype(world, entityId, entityDataList, newArchetype);
                archetypeMap.SetNewComponent<T>(entityData, in component);
                if (newArchetypeGenerated)
                {
                    world.UpdateQueries(newArchetype);
                }
            }

            entityIds.Clear();
            componentValues.Clear();
        }
    }
}