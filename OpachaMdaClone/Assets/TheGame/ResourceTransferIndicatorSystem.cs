using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct RemoveResourceTransferIndicatorTag : ITag
    {
            
    }
    public struct AddResourceTransferIndicatorTag : ITag
    {
            
    }

    public struct ResourceTransferIndicatorComp : IComponent
    {
        public Entity owner;
        public Entity target;
    }
    
    public class ResourceTransferIndicatorSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, SendResourceContinuouslyComp> addResourceTransferIndicatorFilter = new Filter<PositionComp, SendResourceContinuouslyComp>().Tag<AddResourceTransferIndicatorTag>();
        readonly Filter removeResourceTransferIndicatorFilter = new Filter().Tag<RemoveResourceTransferIndicatorTag>();
        readonly Filter<RotationComp, ScaleComp, ResourceTransferIndicatorComp> resourceTransferIndicatorFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            addResourceTransferIndicatorFilter.ForEach(AddResourceTransferIndicator);
            removeResourceTransferIndicatorFilter.ForEach(RemoveResourceTransferIndicator);
        }

        void AddResourceTransferIndicator(Entity entity, ref PositionComp positionComp, ref SendResourceContinuouslyComp sendResourceContinuouslyComp)
        {
            entity.RemoveTag<AddResourceTransferIndicatorTag>();
            
            Entity targetEntity = sendResourceContinuouslyComp.toEntity;
            ref var targetPositionComp = ref targetEntity.GetComponent<PositionComp>();
            var dirToTarget = targetPositionComp.position - positionComp.position;
            var targetAngle = Vector3.SignedAngle(Vector3.right, dirToTarget.ToVector3(), Vector3.forward);
            bool hasIndicator = false;
            
            resourceTransferIndicatorFilter.ForEach((Entity transferIndicatorEntity, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
            {
                if (hasIndicator) return;
                if (resourceTransferIndicatorComp.owner != entity) return;
                resourceTransferIndicatorComp.target = targetEntity;
                
                hasIndicator = true;
                var currentAngle = rotationComp.eulerAngles.z;
                rotationComp.rotZ = targetAngle;
                var diff = targetAngle - currentAngle;
                var dist = XIVMathf.Abs(diff);
                
                float repeatedAngle = XIVMathf.Repeat(diff + 540f, 360f) - 180f;
                targetAngle = currentAngle + repeatedAngle;
                transferIndicatorEntity.CancelTween();
                transferIndicatorEntity.XIVTween()
                    .RotateZ(currentAngle, targetAngle, 1.1f - (dist / 720f), EasingFunction.EaseOutCubic)
                    .Start();
            });

            if (hasIndicator) return;
            
            var indicatorEntity = GameObjectEntity.CreateEntity(world, assetReferences.resourceTransferIndicatorPrefab, positionComp.position.ToVector3(), Quaternion.identity);
            ref var indicatorTransformComp = ref indicatorEntity.GetComponent<TransformComp>();
            ref var indicatorRotationComp = ref indicatorEntity.GetComponent<RotationComp>();
            indicatorTransformComp.transform.Rotate(Vector3.forward, targetAngle);
            indicatorRotationComp.Set(indicatorTransformComp.transform.rotation.eulerAngles.ToVec3());

            var scale = indicatorTransformComp.transform.localScale;
            indicatorEntity.CancelTween();
            indicatorEntity.XIVTween()
                .Scale(Vector3.zero, scale, 0.25f, EasingFunction.EaseInBounce)
                .Start();
            
            indicatorEntity.AddComponent(new ResourceTransferIndicatorComp
            {
                owner = entity,
                target = targetEntity,
            });
        }

        void RemoveResourceTransferIndicator(Entity entity)
        {
            entity.RemoveTag<RemoveResourceTransferIndicatorTag>();
            resourceTransferIndicatorFilter.ForEach((Entity transferIndicatorEntity, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
            {
                if (resourceTransferIndicatorComp.owner != entity) return;
                var scale = scaleComp.scale.ToVector3();
                scaleComp.Set(Vec3.zero);
                transferIndicatorEntity.RemoveComponent<ResourceTransferIndicatorComp>();
                transferIndicatorEntity.CancelTween();
                transferIndicatorEntity.XIVTween()
                    .Scale(scale, Vector3.zero, 1.25f, EasingFunction.EaseOutBack)
                    .OnComplete(() => transferIndicatorEntity.Destroy())
                    .Start();
            });
        }
    }
}