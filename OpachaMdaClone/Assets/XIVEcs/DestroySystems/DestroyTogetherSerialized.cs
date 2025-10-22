namespace XIV.Ecs
{
    public struct DestroyTogetherComp : IComponent
    {
        public Entity entity;
    }
    
    public class DestroyTogetherSerialized : SerializedComponent<DestroyTogetherComp>
    {
        public GameObjectEntity gameObjectEntity;

        public override void AddComponentForEntity(Entity entity)
        {
            entity.AddComponent(new DestroyTogetherComp()
            {
                entity = gameObjectEntity.entity
            });
        }
    }
}