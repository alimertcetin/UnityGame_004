using UnityEngine;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public class TransformSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, PositionComp> positionFilter;
        readonly Filter<TransformComp, ScaleComp> scaleFilter;
        readonly Filter<TransformComp, RotationComp> rotationFilter;
        
        public override void PreAwake()
        {
            world.SetCustomReset<TransformComp>(CustomReset);
        }

        public override void Update()
        {
            positionFilter.ForEach((Entity e, ref TransformComp transformComp, ref PositionComp positionComp) =>
            {
                var pos = positionComp.position.ToVector3();
                if (transformComp.transform.localPosition != pos) transformComp.transform.localPosition = pos;
            });
            scaleFilter.ForEach((ref TransformComp transformComp, ref ScaleComp scaleComp) =>
            {
                var scale = scaleComp.scale.ToVector3();
                if (transformComp.transform.localScale != scale) transformComp.transform.localScale = scale;
            });
            rotationFilter.ForEach((ref TransformComp transformComp, ref RotationComp rotationComp) =>
            {
                var eulerRotation = rotationComp.eulerRotation.ToVector3();
                if (transformComp.transform.localRotation.eulerAngles != eulerRotation) transformComp.transform.localRotation = Quaternion.Euler(eulerRotation);
            });
        }

        static void CustomReset(ref TransformComp comp)
        {
            if (comp.gameObjectEntity != null)
            {
                Object.Destroy(comp.gameObjectEntity.gameObject);
            }

            comp = default;
        }
    }
}