using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XIV.Ecs
{
    public static class GameObjectEntityExtensions
    {
        public static Entity BindToEntity(this GameObject gameObject, World world)
        {
            return GameObjectEntity.BindGameObjectToEntity(world, gameObject);
        }
        
        /// Destroys entity without destroying GameObject
        public static void Unbind(this Entity entity)
        {
            ref var transformComp = ref entity.GetComponent<TransformComp>();
            // Object.Destroy(transformComp.gameObjectEntity);
            transformComp.gameObjectEntity.entity = Entity.Invalid;
            transformComp.gameObjectEntity = null;
            entity.Destroy();
        }
        
        /// Rebind entity to given object
        public static void Rebind(this Entity entity, GameObject gameObject)
        {
            ref var transformComp = ref entity.GetComponent<TransformComp>();
            // Object.Destroy(transformComp.gameObjectEntity);
            transformComp.gameObjectEntity.entity = Entity.Invalid;
            
            transformComp.gameObjectEntity = gameObject.AddComponent<GameObjectEntity>();
            transformComp.gameObjectEntity.entity = entity;
            transformComp.transform = gameObject.transform;
        }
        
        
        /// <summary>
        /// Destroys entity and <see cref="SerializedComponent"/>s without destroying GameObject 
        /// </summary>
        public static void UnbindWithDependencies(this Entity entity)
        {
            ref var transformComp = ref entity.GetComponent<TransformComp>();
            var serializedComponents = transformComp.transform.GetComponentsInChildren<SerializedComponent>();
            for (int i = 0; i < serializedComponents.Length; i++)
            {
                Object.Destroy(serializedComponents[i]);
            }
            // Object.Destroy(transformComp.gameObjectEntity);
            transformComp.gameObjectEntity.entity = Entity.Invalid;
            transformComp.gameObjectEntity = null;
            entity.Destroy();
        }
        
        public static Entity[] DalToEntities(this GameObjectEntity[] gameObjectEntities)
        {
            var entities = new Entity[gameObjectEntities.Length];
            for (int i = 0; i < gameObjectEntities.Length; i++)
            {
                entities[i] = gameObjectEntities[i].entity;
            }
            return entities;
        }
        
        public static Entity[] DalToEntities(this SerializedComponent[] gameObjectEntities)
        {
            var entities = new Entity[gameObjectEntities.Length];
            for (int i = 0; i < gameObjectEntities.Length; i++)
            {
                entities[i] = gameObjectEntities[i].GetComponent<GameObjectEntity>().entity;
            }
            return entities;
        }
    }
    
    [SelectionBase]
    [DisallowMultipleComponent]
    public class GameObjectEntity : MonoBehaviour
    {
        public Entity entity;
        [NonSerialized] public GameObjectEntityPool owner;
        
        public class PrefabData
        {
            // Only make sense if we are using CreateEntityPooled
            public object[] componentBuffer; // Cache the buffer not the component data
            public int[] componentIds; // Cache component ids
            public int[] tagIds; // Cache tag ids
        }

        public PrefabData prefabData;

        static void FillPrefabData(GameObjectEntity goEntity)
        {
            var serializedComponents = goEntity.GetComponents<SerializedComponent>();

            int nComponents = serializedComponents.Length;
            int ii = 0;
            for (int i = 0; i < serializedComponents.Length; i++)
            {
                if (!serializedComponents[i].add)
                {
                    nComponents--;
                    continue;
                }
                serializedComponents[ii++] = serializedComponents[i];
            }

#if UNITY_EDITOR
            int[] componentIds = new int[nComponents + 2];
            componentIds[0] = ComponentIdManager.GetComponentId<TransformComp>();
            componentIds[1] = ComponentIdManager.GetComponentId<DebugNameComp>();
            for (int i = 2; i < componentIds.Length; i++)
            {
                componentIds[i] = serializedComponents[i - 2].GetComponentId();
            }
#else
            int[] componentIds = new int[nComponents + 1];
            componentIds[0] = ComponentIdManager.GetComponentId(typeof(TransformComp));
            for (int i = 1; i < componentIds.Length; i++)
            {
                componentIds[i] = serializedComponentBuffer[i - 1].GetComponentId();
            }
#endif
            var serializedTags = goEntity.GetComponents<SerializedTag>();
            int[] tagIds = new int[serializedTags.Length];
            for (int i = 0; i < serializedTags.Length; i++)
            {
                tagIds[i] = serializedTags[i].GetTagId();
            }

            goEntity.prefabData = new PrefabData
            {
                componentBuffer = new object[componentIds.Length],
                componentIds = componentIds,
                tagIds = tagIds
            };
        }

        static List<SerializedComponent> serializedComponentBuffer = new();
        
        // first component is transform component
        public static void SetupEntity(World world, Entity e, GameObjectEntity goEntity, PrefabData cachedPrefabData)
        {
            if (serializedComponentBuffer == null)
            {
                serializedComponentBuffer = new List<SerializedComponent>();
            }
            serializedComponentBuffer.Clear();
            // 8 kb gc allocation, get components and get transform
            goEntity.entity = e;

            goEntity.GetComponents<SerializedComponent>(serializedComponentBuffer); ;
            goEntity.entity.AddComponent(new TransformComp()
            {
                gameObjectEntity = goEntity,
                transform = goEntity.transform,
            });

#if UNITY_EDITOR
            goEntity.entity.AddComponent(new DebugNameComp()
            {
                name = goEntity.name
            });
#endif

            foreach (var serializedComponent in serializedComponentBuffer)
            {
                if (!serializedComponent.add) continue;
                serializedComponent.AddComponentForEntity(goEntity.entity);
            }

            var serializedActions = goEntity.GetComponents<SerializedAction>();
            foreach (var action in serializedActions)
            {
                action.Action(world, goEntity.entity);
            }
        }
        
        public static PrefabData GetPrefabData(GameObject entityPrefab)
        {
            var prefabGameObjectEntity = entityPrefab.GetComponent<GameObjectEntity>();
            if (prefabGameObjectEntity.prefabData == null) FillPrefabData(prefabGameObjectEntity);
            return prefabGameObjectEntity.prefabData;
        }
        
        public static PrefabData GetPrefabData(GameObjectEntity goEntity)
        {
            if (goEntity.prefabData == null) FillPrefabData(goEntity);
            return goEntity.prefabData;
        }

        public static Entity CreateEntity(World world)
        {
            var entity = world.NewEntity();
            GameObject go = new GameObject();
            GameObjectEntity goe = go.AddComponent<GameObjectEntity>();
            goe.entity = entity;
            entity.AddComponent(new TransformComp()
            {
                transform = go.transform,
                gameObjectEntity = goe
            });
            return entity;
        }
     
        
        public static Entity CreateEntity(World world, GameObject entityPrefab)
        {
            var entity = world.NewEntity();
            GameObject go = Instantiate(entityPrefab);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>(), GetPrefabData(entityPrefab));
            return entity;
        }

        public static void CreateEntity(World world, Entity entity, GameObject entityPrefab)
        {
            GameObject go = Instantiate(entityPrefab);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>(), GetPrefabData(entityPrefab));
        }
        
        public static Entity CreateEntity(World world, GameObject entityPrefab,Vector3 pos,Quaternion rot)
        {
            var entity = world.NewEntity();
            GameObject go = GameObject.Instantiate(entityPrefab,pos,rot);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>(), GetPrefabData(entityPrefab));
            return entity;
        }

        public static Entity CreateEntity(World world,Entity entity, GameObject entityPrefab,Vector3 pos,Quaternion rot)
        {
            GameObject go = GameObject.Instantiate(entityPrefab,pos,rot);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>(), GetPrefabData(entityPrefab));
            return entity;
        }

        // public static Entity CreateEntityPooled(World world, GameObject entityPrefab)
        // {
        //     var entity = world.NewEntity();
        //     var gameObjectEntity  = PoolManager.GetPool(entityPrefab).GetView();
        //     gameObjectEntity.gameObject.SetActive(true);
        //     gameObjectEntity.SetupEntity(world,entity,GetPrefabData(entityPrefab));
        //     return entity;
        // }
        //
        // public static Entity CreateEntityPooled(World world, GameObject entityPrefab,Vector3 pos, Quaternion rot)
        // {
        //     var entity = world.NewEntity();
        //     var gameObjectEntity  = PoolManager.GetPool(entityPrefab).GetView();
        //     gameObjectEntity.transform.SetPositionAndRotation(pos,rot);
        //     gameObjectEntity.gameObject.SetActive(true);
        //     gameObjectEntity.SetupEntity(world,entity,GetPrefabData(entityPrefab));
        //     return entity;
        // }
        
        public static Entity[] CreateEntitiesRecursive(World world, GameObject entityPrefab)
        {
            GameObjectEntity[] gameObjectEntitiesOnPrefab = entityPrefab.GetComponentsInChildren<GameObjectEntity>();

#if UNITY_EDITOR
            if (gameObjectEntitiesOnPrefab.Length == 1)
            {
                Debug.LogWarning("[GameObjectEntity] there is only one entity in prefab, why are you using create entity recursive");
            }
#endif
            
            
            var gameObject = Instantiate(entityPrefab);
            GameObjectEntity[] gameObjectEntitiesOnInstance = gameObject.GetComponentsInChildren<GameObjectEntity>();
            Entity[] entities = new Entity[gameObjectEntitiesOnInstance.Length];

            for (int i = 0; i < gameObjectEntitiesOnInstance.Length; i++)
            {
                entities[i] = world.NewEntity();
                gameObjectEntitiesOnInstance[i].entity = entities[i];
            }

            for (int i = 0; i < gameObjectEntitiesOnInstance.Length; i++)
            {
                SetupEntity(world, entities[i], gameObjectEntitiesOnInstance[i], GetPrefabData(gameObjectEntitiesOnPrefab[i]));
            }
            
            var parentEntity = entities[0];
            for (int i = 1; i < entities.Length; i++)
            {
                entities[i].AddComponent(new DestroyTogetherComp
                {
                    entity = parentEntity
                });
            }

            return entities;
        }

        /// returns first entity
        public static Entity CreateEntityRecursive(World world, GameObject entityPrefab)
        {
            return CreateEntitiesRecursive(world, entityPrefab)[0];
        }
        
        /// <returns>First entity</returns>
        public static Entity CreateEntityRecursive(World world, GameObject entityPrefab, Vector3 pos, Quaternion rot)
        {
            Entity entity = CreateEntitiesRecursiveWithPos(world, entityPrefab,pos,rot)[0];
            return entity;
        }

        public static Entity[] CreateEntitiesRecursiveWithPos(World world, GameObject entityPrefab, Vector3 pos, Quaternion rot)
        {
            GameObjectEntity[] gameObjectEntitiesOnPrefab = entityPrefab.GetComponentsInChildren<GameObjectEntity>();

            var gameObject = Instantiate(entityPrefab, pos, rot);
            GameObjectEntity[] gameObjectEntitiesOnInstance = gameObject.GetComponentsInChildren<GameObjectEntity>();
            Entity[] entities = new Entity[gameObjectEntitiesOnInstance.Length];

            for (int i = 0; i < gameObjectEntitiesOnInstance.Length; i++)
            {
                entities[i] = world.NewEntity();
                gameObjectEntitiesOnInstance[i].entity = entities[i];
            }

            for (int i = 0; i < gameObjectEntitiesOnInstance.Length; i++)
            {
                SetupEntity(world, entities[i], gameObjectEntitiesOnInstance[i], GetPrefabData(gameObjectEntitiesOnPrefab[i]));
            }

            var parentEntity = entities[0];
            for (int i = 1; i < entities.Length; i++)
            {
                entities[i].AddComponent(new DestroyTogetherComp
                {
                    entity = parentEntity
                });
            }

            return entities;
        }

        public static Entity BindGameObjectToEntity(World world, GameObject gameObject)
        {
            var entity = world.NewEntity();
            var gameObjectEntity = gameObject.AddComponent<GameObjectEntity>();
            gameObjectEntity.entity = entity;
            entity.AddComponent(new TransformComp()
            {
                transform = gameObject.transform,
                gameObjectEntity = gameObjectEntity
            });
            return entity;
        }

        public static Entity BindGameObjectToEntityWithDependencies(World world, GameObjectEntity goEnt)
        {
            var entity = world.NewEntity();
            goEnt.entity = entity;
            entity.AddComponent(new TransformComp()
            {
                transform = goEnt.transform,
                gameObjectEntity = goEnt
            });
            SetupEntity(world, goEnt.entity, goEnt, GetPrefabData(goEnt));
            return entity;
        }
    
    }
}
