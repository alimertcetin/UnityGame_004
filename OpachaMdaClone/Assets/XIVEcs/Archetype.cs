using System;
using System.Text;
using XIV.Core.Collections;

namespace XIV.Ecs
{
    public class Archetype
    {
        public Bitset componentBitSet;
        public Bitset tagBitSet;
        public int lockCounter;
        public DynamicArray<Entity> entities = new DynamicArray<Entity>(16);
        public IComponentPool[] componentPools;
        public int[] componentIds;

        public Archetype(Bitset componentBitSet, Bitset tagBitSet)
        {
            this.componentBitSet = componentBitSet;
            this.tagBitSet = tagBitSet;

            int nComponent = componentBitSet.GetSetBitCount();
            componentPools = new IComponentPool[nComponent];
            componentIds = new int[nComponent];

            int i = 0;
            foreach (int componentId in componentBitSet)
            {
                var pool = (IComponentPool)(Activator.CreateInstance(ComponentIdManager.GetComponentPoolType(componentId)));
                componentPools[i] = pool;
                componentIds[i] = componentId;
                i++;
            }
        }

        public IComponentPool GetPoolByIndex(int index)
        {
            return componentPools[index];
        }

        public int GetComponentIdByIndex(int index)
        {
            return componentIds[index];
        }

        public IComponentPool GetComponentPool(int componentId)
        {
            var componentIdsLength = componentIds.Length;
            for (int i = 0; i < componentIdsLength; i++)
            {
                if (componentIds[i] == componentId)
                {
                    return componentPools[i];
                }
            }

            return null;
        }

        public ComponentPool<T> GetComponentPool<T>() where T : struct, IComponent
        {
            int id = ComponentIdManager.GetComponentId<T>();
            var pool = GetComponentPool(id);
            return (ComponentPool<T>)pool;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var components = ComponentIdManager.GetComponentNames(componentIds, sb);
            var tags = TagIdManager.GetTagNames(tagBitSet, sb);
            return $"Comp({components}) - Tag({tags})";
        }

        public void RemoveEntity(int index)
        {
            entities[index] = entities.RemoveLast();
        }

        public Bitset GetComponentBitSet()
        {
            return componentBitSet;
        }

        public Bitset GetTagBitset()
        {
            return tagBitSet;
        }
    }
}
