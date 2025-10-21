using UnityEngine;

namespace XIV.Ecs
{
    public struct CameraComp : IComponent
    {
        
    }
    
    public class DestroySystem : XIV.Ecs.System
    {
        readonly Filter<DestroyLaterComp> destroyLaterFilter = null;
        readonly Filter<TransformComp,DestroyOutOfScreenComp> destroyOutOfScreenFilter = null;
        readonly Filter<CameraComp> cameraFilter = null;
        readonly Filter<DestroyTogetherComp> destroyTogetherFilter = null;

        readonly Plane[] frustumPlaneBuffer = new Plane[6];
        
        public override void LateUpdate()
        {
            Iterator.Iterate(cameraFilter,destroyOutOfScreenFilter,DestroyOutOfScreen);

            destroyLaterFilter.ForEach((Entity entity, ref DestroyLaterComp destroyLaterComp) =>
            {
                if (destroyLaterComp.lifeTime <= 0)
                {
                    entity.Destroy();
                }
                else
                {
                    destroyLaterComp.lifeTime -= XTime.deltaTime;
                }
            });
            
            destroyTogetherFilter.ForEach((Entity entity, ref DestroyTogetherComp destroyTogetherComp) =>
            {
                if (!destroyTogetherComp.entity.IsAlive())
                {
                    entity.Destroy();
                }
            } );
            
        }

        void DestroyOutOfScreen(Entity cameraEntity,ref CameraComp cameraComp, Entity entity,ref TransformComp transformComp,
            ref DestroyOutOfScreenComp destroyOutOfScreenComp)
        {
            GeometryUtility.CalculateFrustumPlanes(Camera.main,frustumPlaneBuffer);
            Vector3 boundsSize = destroyOutOfScreenComp.boundSize;
            if (!GeometryUtility.TestPlanesAABB(frustumPlaneBuffer, new Bounds(transformComp.transform.position, boundsSize)))
            {
                entity.Destroy();
            }
        }
        
    }
}