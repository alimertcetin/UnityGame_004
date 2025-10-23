using System;
using System.Runtime.CompilerServices;
using System.Text;
using XIV.Core.Collections;

namespace XIV.Ecs
{
    public sealed class Archetype
    {
        public Bitset componentBitSet;
        public Bitset tagBitSet;
        public int lockCounter;
        public DynamicArray<Entity> entities = new DynamicArray<Entity>(16);
        public ComponentPoolBase[] componentPools;
        public int[] componentIds;

        public Archetype(Bitset componentBitSet, Bitset tagBitSet)
        {
            this.componentBitSet = componentBitSet;
            this.tagBitSet = tagBitSet;

            int nComponent = componentBitSet.GetSetBitCount();
            componentPools = new ComponentPoolBase[nComponent];
            componentIds = new int[nComponent];

            int i = 0;
            foreach (int componentId in componentBitSet)
            {
                var pool = (ComponentPoolBase)Activator.CreateInstance(ComponentIdManager.GetComponentPoolType(componentId));
                componentPools[i] = pool;
                componentIds[i] = componentId;
                i++;
            }
        }

        public ComponentPoolBase GetPoolByIndex(int index) => componentPools[index];

        public int GetComponentIdByIndex(int index) => componentIds[index];

        public ComponentPoolBase GetComponentPool(int componentId)
        {
            for (int i = 0; i < componentIds.Length; i++)
            {
                if (componentIds[i] == componentId)
                    return componentPools[i];
            }

            return null;
        }

        public ComponentPool<T> GetComponentPool<T>() where T : struct, IComponent
        {
            int id = ComponentIdManager.GetComponentId<T>();
            return (ComponentPool<T>)GetComponentPool(id);
        }

        public ComponentPoolBase GetComponentPool(Type type)
        {
            int id = ComponentIdManager.GetComponentId(type);
            return (ComponentPoolBase)GetComponentPool(id);
        }

        public void MoveTo(Archetype other, int oldEntityArchetypeIndex, int newEntityArchetypeIndex, EntityDataList entityDataList)
        {
            int newIdx = 0, oldIdx = 0;

            while (oldIdx < this.componentIds.Length && newIdx < other.componentIds.Length)
            {
                int oldComponentId = this.GetComponentIdByIndex(oldIdx);
                int newComponentId = other.GetComponentIdByIndex(newIdx);

                // Remove Component from old, new archetype doesn't store this component type
                if (oldComponentId < newComponentId)
                {
                    this.GetPoolByIndex(oldIdx).SwapRemoveComponentAtIndex(oldEntityArchetypeIndex);
                    oldIdx++;
                    continue;
                }

                // new archetype stores the component that old archetype doesn't have
                if (newComponentId < oldComponentId)
                {
                    // new pool stays default LeaveNewDefault
                    newIdx++;
                    continue;
                }
                
                this.GetPoolByIndex(oldIdx).CopyTo(oldEntityArchetypeIndex, other.GetPoolByIndex(newIdx), newEntityArchetypeIndex);
                this.GetPoolByIndex(oldIdx).SwapRemoveComponentAtIndex(oldEntityArchetypeIndex, false);

                newIdx++;
                oldIdx++;
            }

            // RemoveOldOnly
            while (oldIdx < this.componentIds.Length)
            {
                this.GetPoolByIndex(oldIdx).SwapRemoveComponentAtIndex(oldEntityArchetypeIndex);
                oldIdx++;
            }
            
            entities[oldEntityArchetypeIndex] = entities.RemoveLast();
            
            if (this.entities.Count != oldEntityArchetypeIndex)
            {
                var effectedEntity = this.entities[oldEntityArchetypeIndex];
                entityDataList[effectedEntity.entityId.id].indexInArchetype = oldEntityArchetypeIndex;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetComponent<T>(int idx) where T : struct, IComponent
            => ref GetComponentPool<T>().components[idx];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent<T>(int idx, in T value) where T : struct, IComponent
        {
            var pool = GetComponentPool<T>();
            if (pool.customAssign != null)
                pool.customAssign(ref pool.components[idx], value);
            else
                pool.components[idx] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetComponent<T>(int idx) where T : struct, IComponent
        {
            var pool = GetComponentPool<T>();
            if (pool.customReset != null)
                pool.customReset(ref pool.components[idx]);
            else
                pool.components[idx] = default;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var components = ComponentIdManager.GetComponentNames(componentIds, sb);
            var tags = TagIdManager.GetTagNames(tagBitSet, sb);
            return $"Comp({components}) - Tag({tags})";
        }

        public void RemoveEntityAndComponents(int archetypeIdx, EntityDataList entityDataList)
        {
            entities[archetypeIdx] = entities.RemoveLast();

            for (int c = 0; c < componentPools.Length; c++)
            {
                GetPoolByIndex(c).SwapRemoveComponentAtIndex(archetypeIdx);
            }

            if (entities.Count != archetypeIdx)
            {
                var effectedEntity = entities[archetypeIdx];
                entityDataList[effectedEntity.entityId.id].indexInArchetype = archetypeIdx;
            }
        }

        public Bitset GetComponentBitSet() => componentBitSet;
        public Bitset GetTagBitset() => tagBitSet;

        public void AddEntity(World world, ref EntityId entityId, ref EntityData entityData)
        {
            entities.Add() = new Entity(world, entityId.id, entityId.generation);
            entityData.archetype = this;
            entityData.indexInArchetype = entities.Count - 1;
            var newArchetypeComponentPoolLength = componentPools.Length;
            for (int i = 0; i < newArchetypeComponentPoolLength; i++)
            {
                componentPools[i].AddComponent();
            }
        }
    }
}
