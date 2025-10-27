using System.Runtime.CompilerServices;
using XIV.Core.Collections;
using XIV.Core.Utils;

namespace XIV.Ecs
{
    public static class RemoveTagOperations<T> where T : struct, ITag
    {
        static readonly int tagId = TagIdManager.GetTagId<T>();
        static DynamicArray<EntityId> entityIds;

        public static void Init()
        {
            ComponentOperationIndex.AddTagAction(Execute, false);
            entityIds = new DynamicArray<EntityId>(64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveTag(EntityId entityId)
        {
            entityIds.Add() = entityId;
        }

        public static void Execute(World world)
        {
            var entityDataList = world.entityDataList;
            var archetypeMap = world.archetypeMap;
            int len = entityIds.Count;
            for (int i = 0; i < len; i++)
            {
                ref var entityId = ref entityIds[i];
                ref var entityData = ref entityDataList[entityId.id];

                if (entityData.tagBitset.IsBit1(tagId) == false)
                {
                    // Already removed
                    return;
                }

                entityData.tagBitset.SetBit0(tagId);

                var newArchetype = archetypeMap.GetArchetype(entityData.componentBitset, entityData.tagBitset, out var newArchetypeGenerated);

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