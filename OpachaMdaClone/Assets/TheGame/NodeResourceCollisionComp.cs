using XIV.Ecs;

namespace TheGame
{
    public struct NodeResourceCollisionComp : IComponent
    {
        public Entity senderEntity;
        // public Entity receiver; // receiver is component owner
        public Entity senderUnitEntity;
        public int quantity;
    }
}