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
            XTime.timeScale = Time.timeScale;
        }

        void Start()
        {
            manager.Start();
#if UNITY_EDITOR
            var numberOfEntities = world.GetNumberOfEntities();
            Debug.Log($"Number of entities: {numberOfEntities}");
            if (numberOfEntities == 0)
            {
                Debug.LogError("There are no entities in the scene make sure you have a loading system");
            }
#endif
        }

        void Update()
        {
            XTime.deltaTime = Time.deltaTime * XTime.timeScale;
            manager.Update();
        }

        void FixedUpdate()
        {
            XTime.deltaTime = XTime.fixedDeltaTime * XTime.timeScale;
            manager.FixedUpdate();
        }

        void LateUpdate()
        {
            XTime.deltaTime = Time.deltaTime * XTime.timeScale;
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