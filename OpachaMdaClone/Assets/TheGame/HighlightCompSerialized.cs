using XIV.Ecs;

namespace TheGame
{
    public struct HighlightComp : IComponent
    {
        public Entity owner;
    }

    public class HighlightCompSerialized : SerializedComponent<HighlightComp>
    {
        
    }
}