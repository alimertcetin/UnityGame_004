using System;
using System.Text;
using UnityEngine;

namespace XIV.Ecs
{
    public class Filter
    {
        public World world;
        public Query query;

        protected static int lockCounter = 0;

        public static void Init()
        {
            lockCounter = 0;
        }
        
        const int NumberOfEntitiesBufferSize = 512;
        protected static int[][] numberOfEntitiesBuffer = new int[][]
        {
            new int[NumberOfEntitiesBufferSize],
            new int[NumberOfEntitiesBufferSize],
        };
        protected int bufferIdx;
#if UNITY_EDITOR
        public string filterName;
#endif

        public int NumberOfEntities => query.NumberOfEntities();

        public Filter()
        {
            query = new Query();
        }

        public Filter Exclude<ExcludedComponent>()
            where ExcludedComponent : struct, IComponent
        {
            query.ExcludeComp<ExcludedComponent>();
            return this;
        }

        public Filter Tag<T>()
            where T : struct, ITag
        {
            query.IncludeTag<T>();
            return this;
        }

        public Filter ExcludeTag<T>()
            where T : struct, ITag
        {
            query.ExcludeTag<T>();
            return this;
        }

        protected void Lock()
        {
            lockCounter++;
            bufferIdx = lockCounter - 1;

            if (lockCounter >= numberOfEntitiesBuffer.Length)
            {
                int oldLenght = numberOfEntitiesBuffer.Length;
                Array.Resize(ref numberOfEntitiesBuffer,lockCounter * 2);
                for (int i = oldLenght; i < numberOfEntitiesBuffer.Length; i++)
                {
                    numberOfEntitiesBuffer[i] = new int[NumberOfEntitiesBufferSize];
                }
            }
            
            if (numberOfEntitiesBuffer[bufferIdx].Length < query.archetypes.Count)
            {
                Debug.Log($"[Egemen] Don't you think you have too many archetypes: {query} : {query.archetypes.Count}");
                Array.Resize(ref numberOfEntitiesBuffer[bufferIdx], query.archetypes.Count * 2);
            }
            
            for (var i = 0; i < query.archetypes.Count; i++)
            {
                query.archetypes[i].lockCounter++;
                numberOfEntitiesBuffer[bufferIdx][i] = query.archetypes[i].entities.Count;
            }
        }

        protected void Unlock()
        {
            lockCounter--;
            foreach (var archetype in query.archetypes) archetype.lockCounter--;

            Debug.Assert(lockCounter >= 0);
            if (lockCounter == 0)
            {
                world.UnlockComponentOperation();
            }
        }

        public bool IsEmpty()
        {
            return NumberOfEntities == 0;
        }

        public delegate void Iterator(Entity entity);
        public delegate void IteratorEmpty();

        public void ForEach(Iterator iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++) iteratorFunc(archetype.entities[i]);
            }
            Unlock();
        }
        
        public void ForEach(IteratorEmpty iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++) iteratorFunc();
            }
            Unlock();
        }
        
        /// Only use if you are not going to mutate entities, don't support nested foreach
        public void ForEachWithoutLock(Iterator iteratorFunc)
        {
            var nArchetypes = query.archetypes.Count;
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var nEntities = query.archetypes[a].entities.Count;
                for (var i = 0; i < nEntities; i++) iteratorFunc(archetype.entities[i]);
            }
        }

        public void DestroyAll()
        {
            ForEach(e => { e.Destroy(); });
        }

        public void RemoveComponentAll<T>() where T : struct, IComponent
        {
            ForEach(e => e.RemoveComponent<T>());
        }

        public void RemoveTagAll<T>() where T : struct, ITag
        {
            ForEach(e => e.RemoveTag<T>());
        }

        public void AddComponentAll<T>(T value) where T : struct, IComponent
        {
            ForEach(e => e.AddComponent(value));
        }

        public void AddTagAll<T>() where T : struct, ITag
        {
            ForEach(e => e.AddTag<T>());
        }

        public override string ToString()
        {
            var sb = new StringBuilder(128);
            var includedComps = ComponentIdManager.GetComponentNames(query.componentMask.includeComponentSet, sb);
            var excludedComps = ComponentIdManager.GetComponentNames(query.componentMask.excludeComponentSet, sb);

            var includedTags = TagIdManager.GetTagNames(query.componentMask.includeTagSet, sb);
            var excludedTags = TagIdManager.GetTagNames(query.componentMask.excludeTagSet, sb);

            return $"Filter<{includedComps}>.Exclude<{excludedComps}>.Tag<{includedTags}>.Exclude<{excludedTags}>";
        }
    }

    public class Filter<Component0> : Filter
        where Component0 : struct, IComponent
    {
        public Filter()
        {
            query = new Query();
            query.IncludeComp<Component0>();
        }

        public new Filter<Component0> Exclude<T>() where T : struct, IComponent

        {
            query.ExcludeComp<T>();
            return this;
        }

        public new Filter<Component0> Tag<T>() where T : struct, ITag

        {
            query.IncludeTag<T>();
            return this;
        }

        public new Filter<Component0> ExcludeTag<T>() where T : struct, ITag

        {
            query.ExcludeTag<T>();
            return this;
        }

        public delegate void IteratorFunc(ref Component0 comp0);

        public delegate void IteratorFuncWithEntity(Entity entity, ref Component0 comp0);

        public void ForEach(IteratorFunc iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++) iteratorFunc(ref componentPool0.components[i]);
            }

            Unlock();
        }

        public void ForEach(IteratorFuncWithEntity iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++) iteratorFunc(archetype.entities[i], ref componentPool0.components[i]);
            }
            Unlock();
        }
    }

    public class Filter<Component0, Component1> : Filter
        where Component0 : struct, IComponent
        where Component1 : struct, IComponent
    {
        public Filter()
        {
            query = new Query();
            query.IncludeComp<Component0>();
            query.IncludeComp<Component1>();
        }

        public new Filter<Component0, Component1> Exclude<T>() where T : struct, IComponent

        {
            query.ExcludeComp<T>();
            return this;
        }

        public new Filter<Component0, Component1> Tag<T>() where T : struct, ITag

        {
            query.IncludeTag<T>();
            return this;
        }

        public new Filter<Component0, Component1> ExcludeTag<T>() where T : struct, ITag

        {
            query.ExcludeTag<T>();
            return this;
        }

        public delegate void IteratorFunc(ref Component0 comp0, ref Component1 comp1);

        public delegate void IteratorFuncWithEntity(Entity entity, ref Component0 comp0, ref Component1 comp1);

        public void ForEach(IteratorFunc iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++) iteratorFunc(ref componentPool0.components[i], ref componentPool1.components[i]);
            }
            Unlock();
        }

        public void ForEach(IteratorFuncWithEntity iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(archetype.entities[i], ref componentPool0.components[i], ref componentPool1.components[i]);
            }

            Unlock();
        }
    }

    public class Filter<Component0, Component1, Component2> : Filter
        where Component0 : struct, IComponent
        where Component1 : struct, IComponent
        where Component2 : struct, IComponent
    {
        public Filter()
        {
            query = new Query();
            query.IncludeComp<Component0>();
            query.IncludeComp<Component1>();
            query.IncludeComp<Component2>();
        }

        public new Filter<Component0, Component1, Component2> Exclude<T>() where T : struct, IComponent

        {
            query.ExcludeComp<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2> Tag<T>() where T : struct, ITag

        {
            query.IncludeTag<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2> ExcludeTag<T>() where T : struct, ITag

        {
            query.ExcludeTag<T>();
            return this;
        }

        public delegate void IteratorFunc(ref Component0 comp0, ref Component1 comp1, ref Component2 comp2);

        public delegate void IteratorFuncWithEntity(Entity entity, ref Component0 comp0, ref Component1 comp1,
            ref Component2 comp2);

        public void ForEach(IteratorFunc iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(ref componentPool0.components[i], ref componentPool1.components[i], ref componentPool2.components[i]);
            }

            Unlock();
        }

        public void ForEach(IteratorFuncWithEntity iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(archetype.entities[i], ref componentPool0.components[i], ref componentPool1.components[i], ref componentPool2.components[i]);
            }

            Unlock();
        }
    }

    public class Filter<Component0, Component1, Component2, Component3> : Filter
        where Component0 : struct, IComponent
        where Component1 : struct, IComponent
        where Component2 : struct, IComponent
        where Component3 : struct, IComponent
    {
        public Filter()
        {
            query = new Query();
            query.IncludeComp<Component0>();
            query.IncludeComp<Component1>();
            query.IncludeComp<Component2>();
            query.IncludeComp<Component3>();
        }

        public new Filter<Component0, Component1, Component2, Component3> Exclude<T>() where T : struct, IComponent

        {
            query.ExcludeComp<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2, Component3> Tag<T>() where T : struct, ITag

        {
            query.IncludeTag<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2, Component3> ExcludeTag<T>() where T : struct, ITag

        {
            query.ExcludeTag<T>();
            return this;
        }

        public delegate void IteratorFunc(ref Component0 comp0, ref Component1 comp1, ref Component2 comp2,
            ref Component3 comp3);

        public delegate void IteratorFuncWithEntity(Entity entity, ref Component0 comp0, ref Component1 comp1,
            ref Component2 comp2, ref Component3 comp3);

        public void ForEach(IteratorFunc iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var componentPool3 = archetype.GetComponentPool<Component3>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(ref componentPool0.components[i], ref componentPool1.components[i], ref componentPool2.components[i], ref componentPool3.components[i]);
            }

            Unlock();
        }

        public void ForEach(IteratorFuncWithEntity iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var componentPool3 = archetype.GetComponentPool<Component3>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(archetype.entities[i], ref componentPool0.components[i], ref componentPool1.components[i], ref componentPool2.components[i], ref componentPool3.components[i]);
            }

            Unlock();
        }
    }

    public class Filter<Component0, Component1, Component2, Component3, Component4> : Filter
        where Component0 : struct, IComponent
        where Component1 : struct, IComponent
        where Component2 : struct, IComponent
        where Component3 : struct, IComponent
        where Component4 : struct, IComponent
    {
        public Filter()
        {
            query = new Query();
            query.IncludeComp<Component0>();
            query.IncludeComp<Component1>();
            query.IncludeComp<Component2>();
            query.IncludeComp<Component3>();
            query.IncludeComp<Component4>();
        }

        public new Filter<Component0, Component1, Component2, Component3, Component4> Exclude<T>()
            where T : struct, IComponent

        {
            query.ExcludeComp<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2, Component3, Component4> Tag<T>() where T : struct, ITag

        {
            query.IncludeTag<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2, Component3, Component4> ExcludeTag<T>()
            where T : struct, ITag

        {
            query.ExcludeTag<T>();
            return this;
        }

        public delegate void IteratorFunc(ref Component0 comp0, ref Component1 comp1, ref Component2 comp2,
            ref Component3 comp3, ref Component4 comp4);

        public delegate void IteratorFuncWithEntity(Entity entity, ref Component0 comp0, ref Component1 comp1,
            ref Component2 comp2, ref Component3 comp3, ref Component4 comp4);

        public void ForEach(IteratorFunc iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var componentPool3 = archetype.GetComponentPool<Component3>();
                var componentPool4 = archetype.GetComponentPool<Component4>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(ref componentPool0.components[i], ref componentPool1.components[i], ref componentPool2.components[i],
                        ref componentPool3.components[i], ref componentPool4.components[i]);
            }

            Unlock();
        }

        public void ForEach(IteratorFuncWithEntity iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var componentPool3 = archetype.GetComponentPool<Component3>();
                var componentPool4 = archetype.GetComponentPool<Component4>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(archetype.entities[i], ref componentPool0.components[i], ref componentPool1.components[i],
                        ref componentPool2.components[i], ref componentPool3.components[i], ref componentPool4.components[i]);
            }

            Unlock();
        }
    }

    public class Filter<Component0, Component1, Component2, Component3, Component4, Component5> : Filter
        where Component0 : struct, IComponent
        where Component1 : struct, IComponent
        where Component2 : struct, IComponent
        where Component3 : struct, IComponent
        where Component4 : struct, IComponent
        where Component5 : struct, IComponent
    {
        public Filter()
        {
            query = new Query();
            query.IncludeComp<Component0>();
            query.IncludeComp<Component1>();
            query.IncludeComp<Component2>();
            query.IncludeComp<Component3>();
            query.IncludeComp<Component4>();
            query.IncludeComp<Component5>();
        }

        public new Filter<Component0, Component1, Component2, Component3, Component4, Component5> Exclude<T>()
            where T : struct, IComponent

        {
            query.ExcludeComp<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2, Component3, Component4, Component5> Tag<T>()
            where T : struct, ITag

        {
            query.IncludeTag<T>();
            return this;
        }

        public new Filter<Component0, Component1, Component2, Component3, Component4, Component5> ExcludeTag<T>()
            where T : struct, ITag

        {
            query.ExcludeTag<T>();
            return this;
        }

        public delegate void IteratorFunc(ref Component0 comp0, ref Component1 comp1, ref Component2 comp2,
            ref Component3 comp3, ref Component4 comp4, ref Component5 comp5);

        public delegate void IteratorFuncWithEntity(Entity entity, ref Component0 comp0, ref Component1 comp1,
            ref Component2 comp2, ref Component3 comp3, ref Component4 comp4, ref Component5 comp5);

        public void ForEach(IteratorFunc iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var componentPool3 = archetype.GetComponentPool<Component3>();
                var componentPool4 = archetype.GetComponentPool<Component4>();
                var componentPool5 = archetype.GetComponentPool<Component5>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(ref componentPool0.components[i], ref componentPool1.components[i], ref componentPool2.components[i],
                        ref componentPool3.components[i], ref componentPool4.components[i], ref componentPool5.components[i]);
            }

            Unlock();
        }

        public void ForEach(IteratorFuncWithEntity iteratorFunc)
        {
            Lock();
            var nArchetypes = query.archetypes.Count;
            
            for (var a = 0; a < nArchetypes; a++)
            {
                var archetype = query.archetypes[a];
                var componentPool0 = archetype.GetComponentPool<Component0>();
                var componentPool1 = archetype.GetComponentPool<Component1>();
                var componentPool2 = archetype.GetComponentPool<Component2>();
                var componentPool3 = archetype.GetComponentPool<Component3>();
                var componentPool4 = archetype.GetComponentPool<Component4>();
                var componentPool5 = archetype.GetComponentPool<Component5>();
                var nEntities = numberOfEntitiesBuffer[bufferIdx][a];
                for (var i = 0; i < nEntities; i++)
                    iteratorFunc(archetype.entities[i], ref componentPool0.components[i], ref componentPool1.components[i],
                        ref componentPool2.components[i], ref componentPool3.components[i], ref componentPool4.components[i], ref componentPool5.components[i]);
            }

            Unlock();
        }
    }
}