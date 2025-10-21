using System;
using System.Collections.Generic;

namespace XIV.Ecs
{
    public class ArchetypeMap
    {
        public class Node
        {
            public Archetype archetype;
            public Node next;
        }

        public readonly List<Archetype> archetypes = new List<Archetype>();
        public Node[] buckets = new Node[128];

        public Dictionary<int, Delegate> customResetMap;
        public Dictionary<int, Delegate> customAssignMap;

        public ArchetypeMap()
        {
            customResetMap = new Dictionary<int, Delegate>(ComponentIdManager.ComponentTypeCount);
            customAssignMap = new Dictionary<int, Delegate>(ComponentIdManager.ComponentTypeCount);
        }

        Archetype cachedArchetype = null;

        public Archetype GetArchetype(Bitset componentBitSet, Bitset tagBitSet, out bool newArchetypeGenerated)
        {
            if (cachedArchetype != null
                && cachedArchetype.GetComponentBitSet().Equals(ref componentBitSet)
                && cachedArchetype.GetTagBitset().Equals(ref tagBitSet))
            {
                newArchetypeGenerated = false;
                return cachedArchetype;
            }

            int hash = HashCode.Combine(componentBitSet.GetHashCode(), tagBitSet.GetHashCode());
            if (hash < 0)
            {
                hash = -hash;
            }

            int bucketIdx = hash % buckets.Length;
            var node = buckets[bucketIdx];

            Archetype archetype = null;
            newArchetypeGenerated = false;


            if (node == null)
            {
                // Create New Archetype
                archetype = new Archetype(Bitset.Copy(ref componentBitSet), Bitset.Copy(ref tagBitSet));
                newArchetypeGenerated = true;

                buckets[bucketIdx] = new Node()
                {
                    archetype = archetype,
                    next = null
                };
            }
            else
            {
                while (true)
                {
                    if (node.archetype.GetComponentBitSet().Equals(ref componentBitSet)
                        && node.archetype.GetTagBitset().Equals(ref tagBitSet))
                    {
                        archetype = node.archetype;
                        break;
                    }

                    if (node.next == null)
                    {
                        // Create New Archetype
                        archetype = new Archetype(Bitset.Copy(ref componentBitSet), Bitset.Copy(ref tagBitSet));
                        newArchetypeGenerated = true;

                        node.next = new Node()
                        {
                            archetype = archetype,
                            next = null
                        };
                        break;
                    }

                    node = node.next;
                }
            }

            if (newArchetypeGenerated)
            {
                archetypes.Add(archetype);

                var componentIdsLength = archetype.componentIds.Length;
                for (int i = 0; i < componentIdsLength; i++)
                {
                    int componentId = archetype.GetComponentIdByIndex(i);

                    if (customResetMap.TryGetValue(componentId, out var customReset))
                    {
                        archetype.GetPoolByIndex(i).SetCustomReset(customReset);
                    }

                    if (customAssignMap.TryGetValue(componentId, out var customAssign))
                    {
                        archetype.GetPoolByIndex(i).SetCustomAssign(customAssign);
                    }
                }
            }

            cachedArchetype = archetype;
            return cachedArchetype;
        }
        
        public void ChangeArchetype(World world, EntityId entityId, EntityDataList entityDataList, Archetype newArchetype)
        {
            ref var entityData = ref entityDataList[entityId.id];
            var oldArchetype = entityData.archetype;
            int oldArchetypeIdx = entityData.archetypeIdx;

            // Early exit: same archetype -> nothing to do
            if (oldArchetype == newArchetype) return;

            // 1) Add & reserve slots in new archetype
            AddEntityToNewArchetype(world, newArchetype, ref entityId, ref entityData);

            // 2) If there was no old archetype, we're done
            if (oldArchetype == null) return;

            // 3) Merge/copy/remove components
            MergeAndMoveComponents(oldArchetype, newArchetype, oldArchetypeIdx, entityData.archetypeIdx);

            // 4) Remove entity from the old archetype and fix indices
            RemoveEntityFromOldArchetype(oldArchetype, entityDataList, oldArchetypeIdx);
        }
        
        void AddEntityToNewArchetype(World world, Archetype newArchetype, ref EntityId entityId, ref EntityData entityData)
        {
            newArchetype.entities.Add() = new Entity(world, entityId.id, entityId.generation);
            entityData.archetype = newArchetype;
            entityData.archetypeIdx = newArchetype.entities.Count - 1;
            var newArchetypeComponentPoolLength = newArchetype.componentPools.Length;
            for (int i = 0; i < newArchetypeComponentPoolLength; i++)
            {
                newArchetype.GetPoolByIndex(i).AddComponent();
            }
        }

        void MergeAndMoveComponents(Archetype oldArchetype, Archetype newArchetype, int oldIndex, int newIndex)
        {
            int iNew = 0, iOld = 0;
        
            while (iOld < oldArchetype.componentIds.Length && iNew < newArchetype.componentIds.Length)
            {
                int oldComponentId = oldArchetype.GetComponentIdByIndex(iOld);
                int newComponentId = newArchetype.GetComponentIdByIndex(iNew);
        
                // Remove Component from old, new archetype doesn't store this component type
                if (oldComponentId < newComponentId)
                {
                    oldArchetype.GetPoolByIndex(iOld).SwapRemoveComponentAtIndex(oldIndex);
                    iOld++;
                    continue;
                }
        
                // new archetype stores the component that old archetype doesn't have
                if (newComponentId < oldComponentId)
                {
                    // new pool stays default LeaveNewDefault
                    iNew++;
                    continue;
                }
        
                // equal: copy then swap-remove-move MoveShared
                var val = oldArchetype.GetPoolByIndex(iOld).Get(oldIndex);
                newArchetype.GetPoolByIndex(iNew).Set(newIndex, val);
                oldArchetype.GetPoolByIndex(iOld).SwapRemoveMovedComponent(oldIndex);
        
                iNew++; iOld++;
            }
        
            // RemoveOldOnly
            for (; iOld < oldArchetype.componentIds.Length; iOld++)
                oldArchetype.GetPoolByIndex(iOld).SwapRemoveComponentAtIndex(oldIndex);
        }
        
        // void MergeAndMoveComponents(Archetype oldArchetype, Archetype newArchetype, int oldIndex, int newIndex)
        // {
        //     int iNew = 0, iOld = 0;
        //
        //     while (iOld < oldArchetype.componentIds.Length && iNew < newArchetype.componentIds.Length)
        //     {
        //         int oldComponentId = oldArchetype.GetComponentIdByIndex(iOld);
        //         int newComponentId = newArchetype.GetComponentIdByIndex(iNew);
        //
        //         if (oldComponentId < newComponentId)
        //         {
        //             oldArchetype.GetPoolByIndex(iOld).SwapRemoveComponentAtIndex(oldIndex);
        //             iOld++;
        //             continue;
        //         }
        //
        //         if (newComponentId < oldComponentId)
        //         {
        //             // new pool stays default
        //             iNew++;
        //             continue;
        //         }
        //
        //         // equal: copy then swap-remove-move
        //         var srcPool = oldArchetype.GetPoolByIndex(iOld);
        //         var dstPool = newArchetype.GetPoolByIndex(iNew);
        //
        //         // typed copy without boxing
        //         srcPool.CopyTo(oldIndex, dstPool, newIndex);
        //
        //         // remove moved component from old pool
        //         srcPool.SwapRemoveMovedComponent(oldIndex);
        //
        //         iNew++; iOld++;
        //     }
        //
        //     for (; iOld < oldArchetype.componentIds.Length; iOld++)
        //         oldArchetype.GetPoolByIndex(iOld).SwapRemoveComponentAtIndex(oldIndex);
        // }

        

        void RemoveEntityFromOldArchetype(Archetype oldA, EntityDataList entityDataList, int oldIndex)
        {
            oldA.RemoveEntity(oldIndex);
            // if not last entity
            if (oldA.entities.Count != oldIndex)
            {
                // entity slot has been changed, get the entity in the changed slot
                ref var effected = ref oldA.entities[oldIndex];
                // set its index to point to correct location
                entityDataList[effected.entityId.id].archetypeIdx = oldIndex;
            }
        }

    }
}