using XIV.Ecs;

namespace TheGame
{
    public struct AddShieldComp : IComponent
    {
        public float max;
        public float current;
    }
    
    public class NodeAddShieldSystem : XIV.Ecs.System
    {
        readonly Filter<AddShieldComp> addShieldFilter = null;

        public override void Update()
        {
            addShieldFilter.ForEach(AddShield);
        }

        void AddShield(Entity entity, ref AddShieldComp addShieldComp)
        {
            entity.AddComponent(new ShieldComp
            {
                max = addShieldComp.max,
                current = addShieldComp.current,
            });
            entity.RemoveComponent<AddShieldComp>();
            entity.AddTag<AddShieldRendererEventTag>();
        }
    }
}