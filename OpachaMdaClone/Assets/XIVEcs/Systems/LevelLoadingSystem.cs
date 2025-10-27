using UnityEngine;

namespace XIV.Ecs
{
    public class LevelLoadingSystem : XIV.Ecs.System
    {
        public override void Awake()
        {
            GameObjectEntity[] gameObjectEntities = Object.FindObjectsOfType<GameObjectEntity>();
            Entity[] entities = new Entity[gameObjectEntities.Length];

            for (int i = 0; i < gameObjectEntities.Length; i++)
            {
                entities[i] = world.NewEntity();
                gameObjectEntities[i].entity = entities[i];
            }

            for (int i = 0; i < gameObjectEntities.Length; i++)
            {
                GameObjectEntity.SetupEntity(world,entities[i],  gameObjectEntities[i]);
            }

#if UNITY_EDITOR
            Debug.Log("Level Loading Number of Entities: " + entities.Length);
#endif
        }
    }
}