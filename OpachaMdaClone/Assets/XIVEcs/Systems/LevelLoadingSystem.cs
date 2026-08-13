using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;

namespace XIV.Ecs
{
    public class LevelLoadingSystem : XIV.Ecs.System
    {
        public override void Awake()
        {
            GameObjectEntity[] gameObjectEntities = Object.FindObjectsOfType<GameObjectEntity>();
            using XIVBuffer<Entity> entities = ArrayUtils.GetBuffer<Entity>(gameObjectEntities.Length);

            for (int i = 0; i < gameObjectEntities.Length; i++)
            {
                var newEntity = world.NewEntity();
                entities[i] = newEntity;
                gameObjectEntities[i].entity = newEntity;
            }

            for (int i = 0; i < gameObjectEntities.Length; i++)
            {
                GameObjectEntity.SetupEntity(world, entities[i], gameObjectEntities[i]);
            }

#if UNITY_EDITOR
            Debug.Log("Level Loading Number of Entities: " + entities.Length);
#endif
        }
    }
}