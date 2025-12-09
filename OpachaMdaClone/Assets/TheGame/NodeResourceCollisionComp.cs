using XIV.Ecs;

namespace TheGame
{
    public struct NodeResourceCollisionEventComp : IComponent
    {
        // TODO: Make it an event comp
        public Entity senderEntity;
        public Entity receiver;
        public Entity senderUnitEntity;
        public int quantity;
    }
}