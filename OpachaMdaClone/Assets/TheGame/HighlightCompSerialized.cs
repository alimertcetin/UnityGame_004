using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public struct HighlightComp : IComponent
    {
        public Entity highlightedEntity;
        public uint version;
    }

    public class HighlightCompSerialized : SerializedComponent<HighlightComp>
    {
        
    }
}