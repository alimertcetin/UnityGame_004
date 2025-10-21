using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace XIV.Ecs
{
    public class SystemManager
    {
        public List<System>[] stateSystemGroups = new List<System>[32];
        public readonly List<System> systems = new List<System>(128);
        public readonly Dictionary<Type, System> systemMap = new Dictionary<Type, System>();

        public void ChangeState(int s)
        {
            newState = s;
        }

        int newState = 0;
        public int State { private set; get;}
        
        readonly Dictionary<Type,object> injections = new Dictionary<Type, object>();
        readonly World world;
#if UNITY_EDITOR
        public SystemExecutionTimer executionTimer = new SystemExecutionTimer();
#endif

        public SystemManager(World world)
        {
            this.world = world;
        }
        
        public void AddSystem(System system, params int[] states)
        {
#if UNITY_EDITOR
            Debug.Assert(!systems.Contains(system),"SystemManager already contains" + system);
#endif
            systemMap.Add(system.GetType(),system);
            systems.Add(system);
            system.world = world;
            system.manager = this;
            
            if (states.Length == 0)
            {
                AddToState(0);
                return;
            }
            
            foreach (var s in states)
            {
                AddToState(s);
            }

            void AddToState(int stateIdx)
            {
                if (stateIdx >= stateSystemGroups.Length)
                {
                    Array.Resize(ref stateSystemGroups,stateIdx * 2);
                }
                stateSystemGroups[stateIdx] ??= new List<System>();
                stateSystemGroups[stateIdx].Add(system);
            }
        }

        public void Inject(object o)
        {
            injections.Add(o.GetType(),o);
        }
        
        public void HandleInjections()
        {
            var queryMap = new Dictionary<ComponentMask, Query>();
            
            foreach (var system in systems)
            {
                var systemType = system.GetType();
                var filterType = typeof (Filter);

                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                foreach (var f in systemType.GetFields (flags))
                {
                    if (f.IsStatic) 
                    {
                        continue;
                    }
                    
                    // Filters
                    if (f.FieldType == filterType || f.FieldType.IsSubclassOf (filterType))
                    {
                        var filter = (Filter)f.GetValue(system);
                        if (filter == null)
                        {
                            filter = (Filter) Activator.CreateInstance(f.FieldType);
                            f.SetValue(system,filter);
                        }

                        var mask = filter.query.componentMask;
                        filter.world = world;

                        if (queryMap.TryGetValue(mask,out var query))
                        {
                            filter.query = query;
                        }
                        else
                        {
                            query = filter.query;
                            world.queries.Add(filter.query);
                            queryMap.Add(mask,query);
                        }
                        
                        f.SetValue(system, filter);
#if UNITY_EDITOR
                        filter.filterName = system.GetType().Name + "." + f.Name + ": " +filter;
#endif
                        
                        world.filters.Add(filter);
                        continue;
                    }
                    
                    // Other injections.
                    foreach (var pair in injections) 
                    {
                        if (f.FieldType.IsAssignableFrom (pair.Key)) 
                        {
                            f.SetValue(system, pair.Value);
                            break;
                        }
                    }
                }
            }
        }

        public void PreAwake()
        {
            State = newState;
            foreach (var system in systems)
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR   
                executionTimer.StartWatch();
                system.PreAwake();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.Awake);
#else
                system.PreAwake();
#endif
            }
        }
        
        public void Awake()
        {
            foreach (var system in systems)
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR   
                executionTimer.StartWatch();
                system.Awake();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.Awake);
#else
                system.Awake();
#endif
            }
        }

        public void Start()
        {
            foreach (var system in systems)
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR
                executionTimer.StartWatch();
                system.Start();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.Start);
#else
                system.Start();
#endif
            }
        }

        public void FixedUpdate()
        {
            foreach (var system in stateSystemGroups[State])
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR
                executionTimer.StartWatch();
                system.FixedUpdate();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.FixedUpdate);
#else
                system.FixedUpdate();
#endif
            }
        }
        
        public void Update()
        {
            State = newState;
            foreach (var system in stateSystemGroups[State])
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR
                executionTimer.StartWatch();
                system.PreUpdate();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.PreUpdate);
#else
                system.PreUpdate();
#endif
            }
            foreach (var system in stateSystemGroups[State])
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR
                executionTimer.StartWatch();
                system.Update();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.Update);
#else
                system.Update();
#endif
            }
            
            foreach (var system in stateSystemGroups[State])
            {
                if (!system.active) { continue; }
                system.coroutineManager.Update();
            }
        }

        public void LateUpdate()
        {
            foreach (var system in stateSystemGroups[State])
            {
                if (!system.active) { continue; }
#if UNITY_EDITOR
                executionTimer.StartWatch();
                system.LateUpdate();
                executionTimer.StopWatch(system,SystemExecutionTimer.MethodType.LateUpdate);
#else
                system.LateUpdate();
#endif
            }
        }

        public void OnDestroy()
        {
            foreach (var system in systems)
            {
                system.OnDestroy();
            }
        }

        public T GetSystem<T>() where T : System
        {
            if (systemMap.TryGetValue(typeof(T), out var system))
            {
                return (T)system;
            }
            return null;
        }
        
    }
}