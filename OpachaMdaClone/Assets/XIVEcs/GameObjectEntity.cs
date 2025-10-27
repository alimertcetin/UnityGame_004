using System;
using System.Collections.Generic;
using UnityEngine;

namespace XIV.Ecs
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class GameObjectEntity : MonoBehaviour
    {
        public Entity entity;

        static List<SerializedComponent> serializedComponentBuffer = new();
        
        // first component is transform component
        public static void SetupEntity(World world, Entity e, GameObjectEntity goEntity)
        {
            if (serializedComponentBuffer == null)
            {
                serializedComponentBuffer = new List<SerializedComponent>();
            }
            serializedComponentBuffer.Clear();
            // 8 kb gc allocation, get components and get transform
            goEntity.entity = e;

            goEntity.GetComponents<SerializedComponent>(serializedComponentBuffer);
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

            // var serializedActions = goEntity.GetComponents<SerializedAction>();
            // foreach (var action in serializedActions)
            // {
            //     action.Action(world, goEntity.entity);
            // }
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
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>());
            return entity;
        }

        public static void CreateEntity(World world, Entity entity, GameObject entityPrefab)
        {
            GameObject go = Instantiate(entityPrefab);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>());
        }
        
        public static Entity CreateEntity(World world, GameObject entityPrefab,Vector3 pos,Quaternion rot)
        {
            var entity = world.NewEntity();
            GameObject go = GameObject.Instantiate(entityPrefab,pos,rot);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>());
            return entity;
        }

        public static Entity CreateEntity(World world,Entity entity, GameObject entityPrefab,Vector3 pos,Quaternion rot)
        {
            GameObject go = GameObject.Instantiate(entityPrefab,pos,rot);
            SetupEntity(world, entity, go.GetComponent<GameObjectEntity>());
            return entity;
        }
        
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
                SetupEntity(world, entities[i], gameObjectEntitiesOnInstance[i]);
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
                SetupEntity(world, entities[i], gameObjectEntitiesOnInstance[i]);
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
            SetupEntity(world, goEnt.entity, goEnt);
            return entity;
        }
    
    }
}
