using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public struct SwipeDetectorComp : IComponent
    {
        public SwipeDetector swipeDetector;
        public SwipeResult swipeResult;
    }
    
    public class SerializedSwipeDetectorComp : SerializedComponent<SwipeDetectorComp>
    {
        public override void AddComponentForEntity(Entity entity)
        {
            component.swipeDetector = SwipeDetector.New();
            base.AddComponentForEntity(entity);
        }
    }
}