using XIV.Core.TweenSystem;
using XIV.Ecs;

namespace XIVEcsUnityIntegration.Extensions
{
    public static class EntityExtensions
    {
        public static XIVTweenBuilder XIVTween(this Entity entity)
        {
            return entity.HasComponent<TransformComp>() ? entity.GetComponent<TransformComp>().transform.XIVTween() : entity.GetComponent<RectTransformComp>().rectTransform.XIVTween();
        }

        public static bool HasTween(this Entity entity)
        {
            return entity.HasComponent<TransformComp>() ? entity.GetComponent<TransformComp>().transform.HasTween() : entity.GetComponent<RectTransformComp>().rectTransform.HasTween();
        }

        public static void CancelTween(this Entity entity)
        {
            if (entity.HasComponent<TransformComp>())
            {
                entity.GetComponent<TransformComp>().transform.CancelTween();
            }
            else if (entity.HasComponent<RectTransformComp>())
            {
                entity.GetComponent<RectTransformComp>().rectTransform.CancelTween();
            }
        }
    }
}