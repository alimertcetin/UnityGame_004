using XIV.Core.Collections;

namespace XIV.Ecs
{
    public class EntityDataList
    {
        public ref EntityData this[int i] => ref items[i];
        public readonly DynamicArray<EntityData> items;
        public readonly DynamicArray<int> freeIndices;


        public EntityDataList(int initCapacity, int freeIndicesInitCapacity)
        {
            items = new DynamicArray<EntityData>(initCapacity);
            freeIndices = new DynamicArray<int>(freeIndicesInitCapacity);

            for (int i = 0; i < initCapacity; i++)
            {
                ref var entityData = ref items[i];
                entityData.componentBitSet = BitSetFactory.CreateComponentBitSet();
                entityData.tagBitSet = BitSetFactory.CreateTagBitSet();
                entityData.disabledComponents = new DynamicArray<DisabledComponent>();
            }
        }

        public ref EntityData Add(out int idx)
        {
            if (freeIndices.Count > 0)
            {
                idx = freeIndices.RemoveLast();
                items[idx].generationId = -items[idx].generationId;
                return ref items[idx];
            }

            // Create New Entity Data
            ref var entityData = ref items.Add();
            if (entityData.disabledComponents == null)
            {
                entityData.componentBitSet = BitSetFactory.CreateComponentBitSet();
                entityData.tagBitSet = BitSetFactory.CreateTagBitSet();
                entityData.disabledComponents = new DynamicArray<DisabledComponent>();
            }

            idx = items.Count - 1;
            return ref entityData;
        }

        public void Free(int idx)
        {
            freeIndices.Add() = idx;

            // Reset Entity Data
            ref var entityData = ref items[idx];
            // Negative positive generation entityId swapping so we can detect if an entity is freed
            entityData.generationId = -(entityData.generationId + 1);
            entityData.archetype = null;
            entityData.indexInArchetype = -1;

            entityData.componentBitSet.Clear();
            entityData.tagBitSet.Clear();
            entityData.disabledComponents.Clear();
        }
    }
}