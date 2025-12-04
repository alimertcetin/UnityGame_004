using XIV.Ecs;

namespace TheGame
{
    public struct NodeOccupyEventComp : IComponent
    {
        public Entity nodeEntity;
        public Entity unitEntity;
    }
}