using XIV.Ecs;

namespace TheGame
{
    public struct NodeResourceCollisionComp : IComponent
    {
        public Entity sender;
        // public Entity receiver; // receiver is component owner
        public Entity senderUnitEntity;
        public int quantity;
    }
}