using Dalak.Ecs;
using UnityEngine;

namespace XIV.Ecs
{
    public abstract class EasyLevelController : MonoBehaviour
    {
        public World world;
        public SystemManager manager;
        #if UNITY_EDITOR
        GameObject ecsDebug;
        #endif

        protected virtual World CreateWorld()
        {
            return new World();
        }
        
        void Awake()
        {
            world = CreateWorld();
            manager = new SystemManager(world);

#if UNITY_EDITOR
            ecsDebug = EcsDebug.CreateDebug(world,manager,"ECS-DEBUG");
#endif
            AddSystems();
            OnInject();

            manager.HandleInjections();
            manager.PreAwake();
            manager.Awake();
            XTime.fixedDeltaTime = Time.fixedDeltaTime;
        }

        void Start()
        {
            manager.Start();
#if UNITY_EDITOR
            if (world.GetNumberOfEntities() == 0)
            {
                Debug.LogError("There are no entities in the scene make sure you have a loading system");
            }
#endif
        }

        void Update()
        {
            XTime.deltaTime = Time.deltaTime;
            manager.Update();
        }

        void FixedUpdate()
        {
            XTime.deltaTime = XTime.fixedDeltaTime;
            manager.FixedUpdate();
        }

        void LateUpdate()
        {
            XTime.deltaTime = Time.deltaTime;
            manager.LateUpdate();
        }
        

        public abstract void OnInject();
        public abstract void AddSystems();

        void OnDestroy()
        {
#if UNITY_EDITOR
            Destroy(ecsDebug);
#endif
            manager.OnDestroy();
        }
    }
    
}