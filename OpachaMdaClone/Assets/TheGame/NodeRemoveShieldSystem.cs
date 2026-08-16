using XIV.Ecs;

namespace TheGame
{
    public struct RemoveShieldEventComp : IComponent
    {
        public Entity targetEntity;
    }
    
    public class NodeRemoveShieldSystem : XIV.Ecs.System
    {
        readonly Filter<RemoveShieldEventComp> removeShieldFilter = null;

        public override void Update()
        {
            removeShieldFilter.ForEach(RemoveShield);
        }

        void RemoveShield(Entity entity, ref RemoveShieldEventComp removeShieldEventComp)
        {
            entity.Destroy();
            removeShieldEventComp.targetEntity.RemoveComponent<ShieldComp>();
        }
        
    }
}