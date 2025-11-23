using System;
using System.Collections.Generic;
using UnityEngine;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class GameObjectEntity : MonoBehaviour
    {
        public Entity entity;

        static List<SerializedComponent> serializedComponentBuffer = new();
        
        public static void SetupEntity(World world, Entity entity, GameObjectEntity goEntity)
        {
            serializedComponentBuffer ??= new List<SerializedComponent>();
            serializedComponentBuffer.Clear();
            goEntity.entity = entity;
            goEntity.GetComponents<SerializedComponent>(serializedComponentBuffer);
            
            var goEntityTransform = goEntity.transform;
            entity.AddComponent(new TransformComp()
            {
                transform = goEntityTransform,
                gameObjectEntity = goEntity
            });
            entity.AddComponent(new PositionComp
            {
                position = goEntityTransform.localPosition.ToVec3(),
            });
            entity.AddComponent(new ScaleComp
            {
                scale = goEntityTransform.localScale.ToVec3(),
            });
            entity.AddComponent(new RotationComp
            {
                eulerRotation = goEntityTransform.eulerAngles.ToVec3(),
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

        public static Entity CreateEntity(World world, Entity entity, GameObject entityPrefab, Vector3 pos, Quaternion rot)
        {
            GameObject go = GameObject.Instantiate(entityPrefab, pos, rot);
            return BindGameObjectToEntity(world, entity, go);
        }

        public static Entity CreateEntity(World world, GameObject entityPrefab, Vector3 pos, Quaternion rot)
        {
            return CreateEntity(world, world.NewEntity(), entityPrefab, pos, rot);
        }

        public static Entity CreateEntity(World world, Entity entity, GameObject entityPrefab)
        {
            return CreateEntity(world, entity, entityPrefab, Vector3.zero, Quaternion.identity);
        }

        public static Entity CreateEntity(World world, GameObject entityPrefab)
        {
            return CreateEntity(world, world.NewEntity(), entityPrefab);
        }

        public static Entity CreateEntity(World world)
        {
            return BindGameObjectToEntity(world, world.NewEntity(), new GameObject());
        }

        public static Entity BindGameObjectToEntity(World world, Entity entity, GameObject gameObject)
        {
            var goEntity = gameObject.GetOrAddComponent<GameObjectEntity>();
            SetupEntity(world, entity, goEntity);
            return entity;
        }

        public static Entity BindGameObjectToEntity(World world, GameObject gameObject)
        {
            return BindGameObjectToEntity(world, world.NewEntity(), gameObject);
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
            Entity entity = CreateEntitiesRecursiveWithPos(world, entityPrefab, pos, rot)[0];
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
    
    }
}
