namespace XIV.Ecs
{
    public struct DestroyTogetherComp : IComponent
    {
        public Entity entity;
    }
    
    public class DestroyTogetherSerialized : SerializedComponent<DestroyTogetherComp>
    {
        public GameObjectEntity gameObjectEntity;
        public override object GetComponentData(World world)
        {
            return new DestroyTogetherComp()
            {
                entity = gameObjectEntity.entity
            };
        }
    }
}