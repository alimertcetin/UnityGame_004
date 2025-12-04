using XIV.Ecs;

namespace TheGame
{
    public struct AddShieldEventComp : IComponent
    {
        public Entity targetEntity;
        public float max;
        public float current;
    }
    
    public class NodeAddShieldSystem : XIV.Ecs.System
    {
        readonly Filter<AddShieldEventComp> addShieldFilter = null;

        public override void Update()
        {
            addShieldFilter.ForEach(AddShield);
        }

        void AddShield(Entity entity, ref AddShieldEventComp addShieldEventComp)
        {
            entity.Destroy();
            addShieldEventComp.targetEntity.AddComponent(new ShieldComp
            {
                max = addShieldEventComp.max,
                current = addShieldEventComp.current,
            });
            world.NewEntity().AddComponent(new AddShieldRendererEventComp
            {
                targetEntity = addShieldEventComp.targetEntity,
            });
        }
    }
}