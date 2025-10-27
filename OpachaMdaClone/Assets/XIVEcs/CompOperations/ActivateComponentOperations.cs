using XIV.Core.Collections;
using XIV.Core.DataStructures;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    public static class ActivateComponentOperations<T> where T : struct, IComponent
    {
        static readonly int componentId = ComponentIdManager.GetComponentId<T>();
        static DynamicArray<EntityId> disabledComponentOwners;
        static DynamicArray<T> disabledComponents;
        static DynamicArray<int> enabledComponentOwnerIndices;

        public static void Init()
        {
            ComponentOperationIndex.AddActivateComponentAction(ExecuteEnableComponent, true);
            ComponentOperationIndex.AddActivateComponentAction(ExecuteDisableComponent, false);
            disabledComponentOwners = new DynamicArray<EntityId>(64);
            disabledComponents = new DynamicArray<T>(64);
            enabledComponentOwnerIndices = new DynamicArray<int>(64);
        }

        public static void EnableComponent(EntityId entityId)
        {
            int idx = disabledComponentOwners.Exists(p => p.id == entityId.id && p.generation == entityId.generation);
            // throw exception?
            if (idx < 0) return;
            enabledComponentOwnerIndices.Add() = idx;
        }

        public static void DisableComponent(World world, EntityId entityId)
        {
            disabledComponentOwners.Add() = entityId;
            disabledComponents.Add() = world.GetComponent<T>(entityId);
        }

        public static void ExecuteEnableComponent(World world)
        {
            int len = enabledComponentOwnerIndices.Count;
            for (int i = 0; i < len; i++)
            {
                ref var entityId = ref disabledComponentOwners[i];
                world.entityDataList[entityId.id].disabledComponentBitset.SetBit0(componentId);
                var idx = enabledComponentOwnerIndices[i];
                ComponentOperationIndex.AddComponent<T>(disabledComponentOwners[idx], disabledComponents[idx]);
            }
            
            QuickSort(enabledComponentOwnerIndices.AsXIVMemory());

            for (int i = len - 1; i >= 0; i--)
            {
                var idx = enabledComponentOwnerIndices[i];
                disabledComponentOwners.RemoveAt(idx);
                disabledComponents.RemoveAt(idx);
            }
            enabledComponentOwnerIndices.Clear();
            ComponentOperationIndex.ExecuteAddComponentActions(world);
        }

        public static void ExecuteDisableComponent(World world)
        {
            int len = disabledComponentOwners.Count;
            for (int i = 0; i < len; i++)
            {
                ref var entityId = ref disabledComponentOwners[i];
                world.entityDataList[entityId.id].disabledComponentBitset.SetBit1(componentId);
                ComponentOperationIndex.RemoveComponent<T>(entityId);
            }
            ComponentOperationIndex.ExecuteRemoveComponentActions(world);
        }
        
        public static void QuickSort(XIVMemory<int> arr)
        {
            QuickSort(arr, 0, arr.Length - 1);
        }

        static void QuickSort(XIVMemory<int> arr, int left, int right)
        {
            if (left >= right) return;

            int pivot = arr[(left + right) / 2];
            int index = Partition(arr, left, right, pivot);
            QuickSort(arr, left, index - 1);
            QuickSort(arr, index, right);
        }

        static int Partition(XIVMemory<int> arr, int left, int right, int pivot)
        {
            while (left <= right)
            {
                while (arr[left] < pivot) left++;
                while (arr[right] > pivot) right--;

                if (left <= right)
                {
                    (arr[left], arr[right]) = (arr[right], arr[left]);
                    left++;
                    right--;
                }
            }
            return left;
        }
    }
}