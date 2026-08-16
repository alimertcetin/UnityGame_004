using XIV.Ecs;
using XIV.UnityEngineIntegration;

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