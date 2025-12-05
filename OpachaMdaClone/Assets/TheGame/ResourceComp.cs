using XIV.Ecs;

namespace TheGame
{
    public struct ResourceComp : IComponent
    {
        public bool isGeneratingResource;
        public float resourceQuantity;
    }
}