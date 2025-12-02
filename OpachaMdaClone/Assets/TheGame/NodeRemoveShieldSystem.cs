using XIV.Ecs;

namespace TheGame
{
    public struct RemoveShieldEventTag : ITag { }

    public class NodeRemoveShieldSystem : XIV.Ecs.System
    {
        readonly Filter removeShieldFilter = new Filter().Tag<RemoveShieldEventTag>();

        public override void Update()
        {
            removeShieldFilter.ForEach(RemoveShield);
        }

        void RemoveShield(Entity entity)
        {
            entity.RemoveTag<RemoveShieldEventTag>();
            
            entity.RemoveComponent<ShieldComp>();
            entity.RemoveComponent<ShieldGeneratorComp>();
            
            entity.AddTag<RemoveShieldRendererEventTag>();
        }
        
    }
}