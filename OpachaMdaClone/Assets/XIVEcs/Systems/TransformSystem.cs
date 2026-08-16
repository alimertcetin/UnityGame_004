using UnityEngine;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public class TransformSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, PositionComp> positionFilter = null;
        readonly Filter<TransformComp, ScaleComp> scaleFilter = null;
        readonly Filter<TransformComp, RotationComp> rotationFilter = null;
        
        public override void PreAwake()
        {
            world.SetCustomReset<TransformComp>(CustomReset);
        }

        public override void Update()
        {
            // TODO: TransformSystem -> We need to add support in tween system
            positionFilter.ForEach((Entity e, ref TransformComp transformComp, ref PositionComp positionComp) =>
            {
                if (e.HasTween())
                {
                    positionComp.Set(transformComp.transform.localPosition.ToVec3());
                    return;
                }

                var pos = positionComp.position.ToVector3();
                if (transformComp.transform.localPosition != pos) transformComp.transform.localPosition = pos;
            });
            scaleFilter.ForEach((Entity e, ref TransformComp transformComp, ref ScaleComp scaleComp) =>
            {
                if (e.HasTween())
                {
                    scaleComp.Set(transformComp.transform.localScale.ToVec3());
                    return;
                }

                var scale = scaleComp.scale.ToVector3();
                if (transformComp.transform.localScale != scale) transformComp.transform.localScale = scale;
            });
            rotationFilter.ForEach((Entity e, ref TransformComp transformComp, ref RotationComp rotationComp) =>
            {
                if (e.HasTween())
                {
                    rotationComp.Set(transformComp.transform.localRotation.eulerAngles.ToVec3());
                    return;
                }

                var eulerRotation = rotationComp.eulerAngles.ToVector3();
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