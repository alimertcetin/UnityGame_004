using UnityEngine;

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
}