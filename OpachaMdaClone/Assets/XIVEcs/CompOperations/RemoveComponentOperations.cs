using System.Runtime.CompilerServices;
using XIV.Core.Collections;
using XIV.Core.Utils;

namespace XIV.Ecs
{
    public static class RemoveComponentOperations<T> where T : struct, IComponent
    {
        static readonly int componentId = ComponentIdManager.GetComponentId<T>();
        static DynamicArray<EntityId> entityIds;

        public static void Init()
        {
            ComponentOperationIndex.AddComponentAction(Execute, false);
            entityIds = new DynamicArray<EntityId>(64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent(EntityId entityId)
        {
            entityIds.Add() = entityId;
        }

        public static void Execute(World world)
        {
            var entityDataList = world.entityDataList;
            var archetypeMap = world.archetypeMap;
            int entityCount = entityIds.Count;
            for (int i = 0; i < entityCount; i++)
            {
                var entityId = entityIds[i];
                ref var entityData = ref entityDataList[entityId.id];
                int removedComponentId = componentId;

                if (!entityData.componentBitset.IsBit1(removedComponentId))
                {
                    // Already removed
                    continue;
                }

                entityData.componentBitset.SetBit0(removedComponentId);

                var newArchetype = archetypeMap.GetArchetype(
                    entityData.componentBitset, entityData.tagBitset, out var newArchetypeGenerated);

                archetypeMap.ChangeArchetype(world, entityId, entityDataList, newArchetype);

                if (newArchetypeGenerated)
                {
                    world.UpdateQueries(newArchetype);
                }
            }

            entityIds.Clear();
        }

    }
}