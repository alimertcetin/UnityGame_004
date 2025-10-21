namespace XIV.Ecs
{
    public static class Iterator
    {
        public delegate void Iterator2<T0, T1>(ref T0 t0, ref T1 t1);

        public delegate void Iterator3<T0, T1, T2>(ref T0 t0, ref T1 t1, ref T2 t2);

        public delegate void Iterator4<T0, T1, T2, T3>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3);

        public delegate void Iterator5<T0, T1, T2, T3, T4>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);

        public delegate void Iterator6<T0, T1, T2, T3, T4, T5>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
            ref T5 t5);

        public delegate void Iterator7<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3,
            ref T4 t4, ref T5 t5, ref T6 t6);

        public delegate void Iterator8<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3,
            ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7);

        public delegate void Iterator9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3,
            ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8);

        public delegate void Iterator10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref T0 t0, ref T1 t1, ref T2 t2,
            ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8, ref T9 t9);

        public delegate void Iterator11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref T0 t0, ref T1 t1, ref T2 t2,
            ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8, ref T9 t9, ref T10 t10);

        public delegate void Iterator12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref T0 t0, ref T1 t1,
            ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8, ref T9 t9, ref T10 t10,
            ref T11 t11);

        public delegate void IteratorWithEntity1_1<T10, T20>(Entity entity0, ref T10 t10, Entity entity1, ref T20 t20);

        public delegate void IteratorWithEntity1_2<T10, T20, T21>(Entity entity0, ref T10 t10, Entity entity1,
            ref T20 t20, ref T21 t21);

        public delegate void IteratorWithEntity1_3<T10, T20, T21, T22>(Entity entity0, ref T10 t10, Entity entity1,
            ref T20 t20, ref T21 t21, ref T22 t22);

        public delegate void IteratorWithEntity1_4<T10, T20, T21, T22, T23>(Entity entity0, ref T10 t10, Entity entity1,
            ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23);

        public delegate void IteratorWithEntity1_5<T10, T20, T21, T22, T23, T24>(Entity entity0, ref T10 t10,
            Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24);

        public delegate void IteratorWithEntity1_6<T10, T20, T21, T22, T23, T24, T25>(Entity entity0, ref T10 t10,
            Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24, ref T25 t25);

        public delegate void IteratorWithEntity2_1<T10, T11, T20>(Entity entity0, ref T10 t10, ref T11 t11,
            Entity entity1, ref T20 t20);

        public delegate void IteratorWithEntity2_2<T10, T11, T20, T21>(Entity entity0, ref T10 t10, ref T11 t11,
            Entity entity1, ref T20 t20, ref T21 t21);

        public delegate void IteratorWithEntity2_3<T10, T11, T20, T21, T22>(Entity entity0, ref T10 t10, ref T11 t11,
            Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22);

        public delegate void IteratorWithEntity2_4<T10, T11, T20, T21, T22, T23>(Entity entity0, ref T10 t10,
            ref T11 t11, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23);

        public delegate void IteratorWithEntity2_5<T10, T11, T20, T21, T22, T23, T24>(Entity entity0, ref T10 t10,
            ref T11 t11, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24);

        public delegate void IteratorWithEntity2_6<T10, T11, T20, T21, T22, T23, T24, T25>(Entity entity0, ref T10 t10,
            ref T11 t11, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24, ref T25 t25);

        public delegate void IteratorWithEntity3_1<T10, T11, T12, T20>(Entity entity0, ref T10 t10, ref T11 t11,
            ref T12 t12, Entity entity1, ref T20 t20);

        public delegate void IteratorWithEntity3_2<T10, T11, T12, T20, T21>(Entity entity0, ref T10 t10, ref T11 t11,
            ref T12 t12, Entity entity1, ref T20 t20, ref T21 t21);

        public delegate void IteratorWithEntity3_3<T10, T11, T12, T20, T21, T22>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22);

        public delegate void IteratorWithEntity3_4<T10, T11, T12, T20, T21, T22, T23>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23);

        public delegate void IteratorWithEntity3_5<T10, T11, T12, T20, T21, T22, T23, T24>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24);

        public delegate void IteratorWithEntity3_6<T10, T11, T12, T20, T21, T22, T23, T24, T25>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23,
            ref T24 t24, ref T25 t25);

        public delegate void IteratorWithEntity4_1<T10, T11, T12, T13, T20>(Entity entity0, ref T10 t10, ref T11 t11,
            ref T12 t12, ref T13 t13, Entity entity1, ref T20 t20);

        public delegate void IteratorWithEntity4_2<T10, T11, T12, T13, T20, T21>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, Entity entity1, ref T20 t20, ref T21 t21);

        public delegate void IteratorWithEntity4_3<T10, T11, T12, T13, T20, T21, T22>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22);

        public delegate void IteratorWithEntity4_4<T10, T11, T12, T13, T20, T21, T22, T23>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23);

        public delegate void IteratorWithEntity4_5<T10, T11, T12, T13, T20, T21, T22, T23, T24>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22,
            ref T23 t23, ref T24 t24);

        public delegate void IteratorWithEntity4_6<T10, T11, T12, T13, T20, T21, T22, T23, T24, T25>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22,
            ref T23 t23, ref T24 t24, ref T25 t25);

        public delegate void IteratorWithEntity5_1<T10, T11, T12, T13, T14, T20>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, Entity entity1, ref T20 t20);

        public delegate void IteratorWithEntity5_2<T10, T11, T12, T13, T14, T20, T21>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, Entity entity1, ref T20 t20, ref T21 t21);

        public delegate void IteratorWithEntity5_3<T10, T11, T12, T13, T14, T20, T21, T22>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22);

        public delegate void IteratorWithEntity5_4<T10, T11, T12, T13, T14, T20, T21, T22, T23>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, Entity entity1, ref T20 t20, ref T21 t21,
            ref T22 t22, ref T23 t23);

        public delegate void IteratorWithEntity5_5<T10, T11, T12, T13, T14, T20, T21, T22, T23, T24>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, Entity entity1, ref T20 t20, ref T21 t21,
            ref T22 t22, ref T23 t23, ref T24 t24);

        public delegate void IteratorWithEntity5_6<T10, T11, T12, T13, T14, T20, T21, T22, T23, T24, T25>(
            Entity entity0, ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, Entity entity1,
            ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24, ref T25 t25);

        public delegate void IteratorWithEntity6_1<T10, T11, T12, T13, T14, T15, T20>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, ref T15 t15, Entity entity1, ref T20 t20);

        public delegate void IteratorWithEntity6_2<T10, T11, T12, T13, T14, T15, T20, T21>(Entity entity0, ref T10 t10,
            ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, ref T15 t15, Entity entity1, ref T20 t20, ref T21 t21);

        public delegate void IteratorWithEntity6_3<T10, T11, T12, T13, T14, T15, T20, T21, T22>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, ref T15 t15, Entity entity1, ref T20 t20,
            ref T21 t21, ref T22 t22);

        public delegate void IteratorWithEntity6_4<T10, T11, T12, T13, T14, T15, T20, T21, T22, T23>(Entity entity0,
            ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, ref T15 t15, Entity entity1, ref T20 t20,
            ref T21 t21, ref T22 t22, ref T23 t23);

        public delegate void IteratorWithEntity6_5<T10, T11, T12, T13, T14, T15, T20, T21, T22, T23, T24>(
            Entity entity0, ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, ref T15 t15,
            Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24);

        public delegate void IteratorWithEntity6_6<T10, T11, T12, T13, T14, T15, T20, T21, T22, T23, T24, T25>(
            Entity entity0, ref T10 t10, ref T11 t11, ref T12 t12, ref T13 t13, ref T14 t14, ref T15 t15,
            Entity entity1, ref T20 t20, ref T21 t21, ref T22 t22, ref T23 t23, ref T24 t24, ref T25 t25);


        static int[] numberOfEntitiesBuffer1 = new int[128];
        static int[] numberOfEntitiesBuffer2 = new int[128];

        static void Lock(Filter filter1, Filter filter2)
        {
            for (int a = 0; a < filter1.query.archetypes.Count; a++)
            {
                numberOfEntitiesBuffer1[a] = filter1.query.archetypes[a].entities.Count;
                filter1.query.archetypes[a].lockCounter++;
            }

            for (int a = 0; a < filter2.query.archetypes.Count; a++)
            {
                numberOfEntitiesBuffer2[a] = filter2.query.archetypes[a].entities.Count;
                filter2.query.archetypes[a].lockCounter++;
            }
        }

        static void Unlock(Filter filter1, Filter filter2)
        {
            foreach (var archetype in filter1.query.archetypes) archetype.lockCounter--;
            foreach (var archetype in filter2.query.archetypes) archetype.lockCounter--;
            filter1.world.UnlockComponentOperation();
        }


        public static void Iterate<F1C0, F2C0>(Filter<F1C0> filter1, Filter<F2C0> filter2,
            Iterator2<F1C0, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(ref f1c0, ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0>(Filter<F1C0> filter1, Filter<F2C0> filter2,
            IteratorWithEntity1_1<F1C0, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(entity0, ref f1c0, archetype2.entities[i1], ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1>(Filter<F1C0> filter1, Filter<F2C0, F2C1> filter2,
            Iterator3<F1C0, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(ref f1c0, ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1>(Filter<F1C0> filter1, Filter<F2C0, F2C1> filter2,
            IteratorWithEntity1_2<F1C0, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(entity0, ref f1c0, archetype2.entities[i1], ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2>(Filter<F1C0> filter1, Filter<F2C0, F2C1, F2C2> filter2,
            Iterator4<F1C0, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(ref f1c0, ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2>(Filter<F1C0> filter1, Filter<F2C0, F2C1, F2C2> filter2,
            IteratorWithEntity1_3<F1C0, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(entity0, ref f1c0, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2, F2C3>(Filter<F1C0> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3> filter2, Iterator5<F1C0, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(ref f1c0, ref f2c0, ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2, F2C3>(Filter<F1C0> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3> filter2, IteratorWithEntity1_4<F1C0, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(entity0, ref f1c0, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4>(Filter<F1C0> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2, Iterator6<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(ref f1c0, ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4>(Filter<F1C0> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            IteratorWithEntity1_5<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(entity0, ref f1c0, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2, ref f2c3,
                                ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(Filter<F1C0> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            Iterator7<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(ref f1c0, ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(Filter<F1C0> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            IteratorWithEntity1_6<F1C0, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(entity0, ref f1c0, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2, ref f2c3,
                                ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0>(Filter<F1C0, F1C1> filter1, Filter<F2C0> filter2,
            Iterator3<F1C0, F1C1, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0>(Filter<F1C0, F1C1> filter1, Filter<F2C0> filter2,
            IteratorWithEntity2_1<F1C0, F1C1, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, archetype2.entities[i1], ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1>(Filter<F1C0, F1C1> filter1, Filter<F2C0, F2C1> filter2,
            Iterator4<F1C0, F1C1, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1>(Filter<F1C0, F1C1> filter1, Filter<F2C0, F2C1> filter2,
            IteratorWithEntity2_2<F1C0, F1C1, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, archetype2.entities[i1], ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2> filter2, Iterator5<F1C0, F1C1, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2> filter2, IteratorWithEntity2_3<F1C0, F1C1, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, archetype2.entities[i1], ref f2c0, ref f2c1,
                                ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3> filter2, Iterator6<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f2c0, ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3> filter2, IteratorWithEntity2_4<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2, Iterator7<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            IteratorWithEntity2_5<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            Iterator8<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(Filter<F1C0, F1C1> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            IteratorWithEntity2_6<F1C0, F1C1, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0>(Filter<F1C0, F1C1, F1C2> filter1, Filter<F2C0> filter2,
            Iterator4<F1C0, F1C1, F1C2, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0>(Filter<F1C0, F1C1, F1C2> filter1, Filter<F2C0> filter2,
            IteratorWithEntity3_1<F1C0, F1C1, F1C2, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, archetype2.entities[i1], ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1> filter2, Iterator5<F1C0, F1C1, F1C2, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1> filter2, IteratorWithEntity3_2<F1C0, F1C1, F1C2, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, archetype2.entities[i1], ref f2c0,
                                ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1, F2C2> filter2, Iterator6<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1, F2C2> filter2, IteratorWithEntity3_3<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, archetype2.entities[i1], ref f2c0, ref f2c1,
                                ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3> filter2, Iterator7<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f2c0, ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            IteratorWithEntity3_4<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, archetype2.entities[i1], ref f2c0, ref f2c1,
                                ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            Iterator8<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4>(Filter<F1C0, F1C1, F1C2> filter1,
            Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            IteratorWithEntity3_5<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, archetype2.entities[i1], ref f2c0, ref f2c1,
                                ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            Iterator9<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4,
                                ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            IteratorWithEntity3_6<F1C0, F1C1, F1C2, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, archetype2.entities[i1], ref f2c0, ref f2c1,
                                ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0>(Filter<F1C0, F1C1, F1C2, F1C3> filter1,
            Filter<F2C0> filter2, Iterator5<F1C0, F1C1, F1C2, F1C3, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0>(Filter<F1C0, F1C1, F1C2, F1C3> filter1,
            Filter<F2C0> filter2, IteratorWithEntity4_1<F1C0, F1C1, F1C2, F1C3, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, archetype2.entities[i1],
                                ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1>(Filter<F1C0, F1C1, F1C2, F1C3> filter1,
            Filter<F2C0, F2C1> filter2, Iterator6<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1>(Filter<F1C0, F1C1, F1C2, F1C3> filter1,
            Filter<F2C0, F2C1> filter2, IteratorWithEntity4_2<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, archetype2.entities[i1], ref f2c0,
                                ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2>(Filter<F1C0, F1C1, F1C2, F1C3> filter1,
            Filter<F2C0, F2C1, F2C2> filter2, Iterator7<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2>(Filter<F1C0, F1C1, F1C2, F1C3> filter1,
            Filter<F2C0, F2C1, F2C2> filter2, IteratorWithEntity4_3<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, archetype2.entities[i1], ref f2c0,
                                ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3>(
            Filter<F1C0, F1C1, F1C2, F1C3> filter1, Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            Iterator8<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f2c0, ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3>(
            Filter<F1C0, F1C1, F1C2, F1C3> filter1, Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            IteratorWithEntity4_4<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, archetype2.entities[i1], ref f2c0,
                                ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4>(
            Filter<F1C0, F1C1, F1C2, F1C3> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            Iterator9<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f2c0, ref f2c1, ref f2c2, ref f2c3,
                                ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4>(
            Filter<F1C0, F1C1, F1C2, F1C3> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            IteratorWithEntity4_5<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, archetype2.entities[i1], ref f2c0,
                                ref f2c1, ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2, F1C3> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            Iterator10<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f2c0, ref f2c1, ref f2c2, ref f2c3,
                                ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2, F1C3> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            IteratorWithEntity4_6<F1C0, F1C1, F1C2, F1C3, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, archetype2.entities[i1], ref f2c0,
                                ref f2c1, ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0>(Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1,
            Filter<F2C0> filter2, Iterator6<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0>(Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1,
            Filter<F2C0> filter2, IteratorWithEntity5_1<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, archetype2.entities[i1],
                                ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1> filter2,
            Iterator7<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1> filter2,
            IteratorWithEntity5_2<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, archetype2.entities[i1],
                                ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2> filter2,
            Iterator8<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2> filter2,
            IteratorWithEntity5_3<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, archetype2.entities[i1],
                                ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            Iterator9<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            IteratorWithEntity5_4<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, archetype2.entities[i1],
                                ref f2c0, ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            Iterator10<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            IteratorWithEntity5_5<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, archetype2.entities[i1],
                                ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            Iterator11<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f2c0, ref f2c1, ref f2c2,
                                ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            IteratorWithEntity5_6<F1C0, F1C1, F1C2, F1C3, F1C4, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, archetype2.entities[i1],
                                ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0> filter2,
            Iterator7<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5, ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0> filter2,
            IteratorWithEntity6_1<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5,
                                archetype2.entities[i1], ref f2c0);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1> filter2,
            Iterator8<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5, ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1> filter2,
            IteratorWithEntity6_2<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5,
                                archetype2.entities[i1], ref f2c0, ref f2c1);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2> filter2,
            Iterator9<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5, ref f2c0, ref f2c1,
                                ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2> filter2,
            IteratorWithEntity6_3<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5,
                                archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            Iterator10<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5, ref f2c0, ref f2c1,
                                ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2, F2C3> filter2,
            IteratorWithEntity6_4<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5,
                                archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2, ref f2c3);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            Iterator11<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5, ref f2c0, ref f2c1,
                                ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4> filter2,
            IteratorWithEntity6_5<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5,
                                archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            Iterator12<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5, ref f2c0, ref f2c1,
                                ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }

        public static void Iterate<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5>(
            Filter<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5> filter1, Filter<F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> filter2,
            IteratorWithEntity6_6<F1C0, F1C1, F1C2, F1C3, F1C4, F1C5, F2C0, F2C1, F2C2, F2C3, F2C4, F2C5> iterator)
            where F1C0 : struct, IComponent
            where F1C1 : struct, IComponent
            where F1C2 : struct, IComponent
            where F1C3 : struct, IComponent
            where F1C4 : struct, IComponent
            where F1C5 : struct, IComponent
            where F2C0 : struct, IComponent
            where F2C1 : struct, IComponent
            where F2C2 : struct, IComponent
            where F2C3 : struct, IComponent
            where F2C4 : struct, IComponent
            where F2C5 : struct, IComponent

        {
            Lock(filter1, filter2);
            int nArchetypes = filter1.query.archetypes.Count;
            for (int a = 0; a < nArchetypes; a++)
            {
                var archetype1 = filter1.query.archetypes[a];
                var filter1ComponentPool0 = archetype1.GetComponentPool<F1C0>();
                var filter1ComponentPool1 = archetype1.GetComponentPool<F1C1>();
                var filter1ComponentPool2 = archetype1.GetComponentPool<F1C2>();
                var filter1ComponentPool3 = archetype1.GetComponentPool<F1C3>();
                var filter1ComponentPool4 = archetype1.GetComponentPool<F1C4>();
                var filter1ComponentPool5 = archetype1.GetComponentPool<F1C5>();
                for (int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)
                {
                    ref var f1c0 = ref filter1ComponentPool0.components[i0];
                    ref var f1c1 = ref filter1ComponentPool1.components[i0];
                    ref var f1c2 = ref filter1ComponentPool2.components[i0];
                    ref var f1c3 = ref filter1ComponentPool3.components[i0];
                    ref var f1c4 = ref filter1ComponentPool4.components[i0];
                    ref var f1c5 = ref filter1ComponentPool5.components[i0];
                    var entity0 = archetype1.entities[i0];
                    int nArchetypes2 = filter2.query.archetypes.Count;
                    for (int a2 = 0; a2 < nArchetypes2; a2++)
                    {
                        var archetype2 = filter2.query.archetypes[a2];
                        var filter2ComponentPool0 = archetype2.GetComponentPool<F2C0>();
                        var filter2ComponentPool1 = archetype2.GetComponentPool<F2C1>();
                        var filter2ComponentPool2 = archetype2.GetComponentPool<F2C2>();
                        var filter2ComponentPool3 = archetype2.GetComponentPool<F2C3>();
                        var filter2ComponentPool4 = archetype2.GetComponentPool<F2C4>();
                        var filter2ComponentPool5 = archetype2.GetComponentPool<F2C5>();
                        for (int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)
                        {
                            ref var f2c0 = ref filter2ComponentPool0.components[i1];
                            ref var f2c1 = ref filter2ComponentPool1.components[i1];
                            ref var f2c2 = ref filter2ComponentPool2.components[i1];
                            ref var f2c3 = ref filter2ComponentPool3.components[i1];
                            ref var f2c4 = ref filter2ComponentPool4.components[i1];
                            ref var f2c5 = ref filter2ComponentPool5.components[i1];
                            iterator(entity0, ref f1c0, ref f1c1, ref f1c2, ref f1c3, ref f1c4, ref f1c5,
                                archetype2.entities[i1], ref f2c0, ref f2c1, ref f2c2, ref f2c3, ref f2c4, ref f2c5);
                        }
                    }
                }
            }

            Unlock(filter1, filter2);
        }
    }
}