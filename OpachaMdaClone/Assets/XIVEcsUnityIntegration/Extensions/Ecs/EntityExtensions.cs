using XIV.Core.TweenSystem;
using XIV.Ecs;

namespace XIVEcsUnityIntegration.Extensions
{
    public static class EntityExtensions
    {
        public static XIVTweenBuilder XIVTween(this Entity entity)
        {
            return entity.GetComponent<TransformComp>().transform.XIVTween();
        }

        public static bool HasTween(this Entity entity)
        {
            return entity.GetComponent<TransformComp>().transform.HasTween();
        }

        public static void CancelTween(this Entity entity)
        {
            entity.GetComponent<TransformComp>().transform.CancelTween();
        }
    }
}