using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    // public struct CreateResourceTransferIndicatorEventComp : IComponent
    // {
    //     public Entity fromEntity;
    //     public Entity targetEntity;
    // }
    //
    // public struct RemoveResourceTransferIndicatorEventComp : IComponent
    // {
    //     public Entity ownerEntity;
    // }

    public struct ResourceTransferIndicatorComp : IComponent
    {
        public Entity owner;
        public Entity target;
    }
    
    public class ResourceTransferIndicatorSystem : XIV.Ecs.System
    {
        // readonly Filter<CreateResourceTransferIndicatorEventComp> addResourceTransferIndicatorFilter = null;
        // readonly Filter<RemoveResourceTransferIndicatorEventComp> removeResourceTransferIndicatorFilter = null;
        readonly Filter<PositionComp, SendResourceContinuouslyComp> sendResourceContinuouslyFilter = null;
        readonly Filter<RotationComp, ScaleComp, ResourceTransferIndicatorComp> resourceTransferIndicatorFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            sendResourceContinuouslyFilter.ForEach((Entity entity, ref PositionComp positionComp, ref SendResourceContinuouslyComp sendResourceContinuouslyComp) =>
            {
                bool hasIndicator = false;
                var targetEntity = sendResourceContinuouslyComp.toEntity;
                ref var targetPositionComp = ref targetEntity.GetComponent<PositionComp>();
                var dirToTarget = targetPositionComp.position - positionComp.position;
                var targetAngle = Vector3.SignedAngle(Vector3.right, dirToTarget.ToVector3(), Vector3.forward);
                
                resourceTransferIndicatorFilter.ForEach((Entity indicatorEntity, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
                {
                    if (hasIndicator || resourceTransferIndicatorComp.owner != entity) return;
                    hasIndicator = true;
                    if (targetEntity == resourceTransferIndicatorComp.target) return;
                    
                    var currentAngle = rotationComp.eulerAngles.z;
                    rotationComp.rotZ = targetAngle;
                    var diff = targetAngle - currentAngle;
                    var dist = XIVMathf.Abs(diff);
                
                    float repeatedAngle = XIVMathf.Repeat(diff + 540f, 360f) - 180f;
                    targetAngle = currentAngle + repeatedAngle;
                    indicatorEntity.CancelTween();
                    indicatorEntity.XIVTween()
                        .RotateZ(currentAngle, targetAngle, 1.1f - (dist / 720f), EasingFunction.EaseOutCubic)
                        .UseCustomDeltaTime(() => XTime.deltaTime)
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
                    .UseCustomDeltaTime(() => XTime.deltaTime)
                    .Start();
            
                indicatorEntity.AddComponent(new ResourceTransferIndicatorComp
                {
                    owner = entity,
                    target = targetEntity,
                });
            });
            resourceTransferIndicatorFilter.ForEach((Entity indicatorEntity, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
            {
                if (resourceTransferIndicatorComp.owner.HasComponent<SendResourceContinuouslyComp>()) return;
                var scale = scaleComp.scale.ToVector3();
                scaleComp.Set(Vec3.zero);
                indicatorEntity.RemoveComponent<ResourceTransferIndicatorComp>();
                indicatorEntity.CancelTween();
                indicatorEntity.XIVTween()
                    .Scale(scale, Vector3.zero, 1.25f, EasingFunction.EaseOutBack)
                    .OnComplete(() => indicatorEntity.Destroy())
                    .UseCustomDeltaTime(() => XTime.deltaTime)
                    .Start();
            });
            // addResourceTransferIndicatorFilter.ForEach(AddResourceTransferIndicator);
            // removeResourceTransferIndicatorFilter.ForEach(RemoveResourceTransferIndicators);
        }

        // void RemoveResourceTransferIndicators(Entity entity, ref RemoveResourceTransferIndicatorEventComp removeResourceTransferIndicatorEventComp)
        // {
        //     entity.Destroy();
        //     var owner = removeResourceTransferIndicatorEventComp.ownerEntity;
        //     resourceTransferIndicatorFilter.ForEach((Entity transferIndicatorEntity, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
        //     {
        //         if (resourceTransferIndicatorComp.owner != owner) return;
        //         var scale = scaleComp.scale.ToVector3();
        //         scaleComp.Set(Vec3.zero);
        //         transferIndicatorEntity.RemoveComponent<ResourceTransferIndicatorComp>();
        //         transferIndicatorEntity.CancelTween();
        //         transferIndicatorEntity.XIVTween()
        //             .Scale(scale, Vector3.zero, 1.25f, EasingFunction.EaseOutBack)
        //             .OnComplete(() => transferIndicatorEntity.Destroy())
        //             .Start();
        //     });
        // }

        // void AddResourceTransferIndicator(Entity entity, ref CreateResourceTransferIndicatorEventComp  createResourceTransferIndicatorEventComp)
        // {
        //     entity.Destroy();
        //     var fromEntity = createResourceTransferIndicatorEventComp.fromEntity;
        //     var targetEntity = createResourceTransferIndicatorEventComp.targetEntity;
        //     ref var positionComp = ref fromEntity.GetComponent<PositionComp>();
        //     ref var targetPositionComp = ref targetEntity.GetComponent<PositionComp>();
        //     var dirToTarget = targetPositionComp.position - positionComp.position;
        //     var targetAngle = Vector3.SignedAngle(Vector3.right, dirToTarget.ToVector3(), Vector3.forward);
        //     bool hasIndicator = false;
        //     
        //     resourceTransferIndicatorFilter.ForEach((Entity transferIndicatorEntity, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
        //     {
        //         if (hasIndicator) return;
        //         if (resourceTransferIndicatorComp.owner != fromEntity) return;
        //         resourceTransferIndicatorComp.target = targetEntity;
        //         hasIndicator = true;
        //         
        //         var currentAngle = rotationComp.eulerAngles.z;
        //         rotationComp.rotZ = targetAngle;
        //         var diff = targetAngle - currentAngle;
        //         var dist = XIVMathf.Abs(diff);
        //         
        //         float repeatedAngle = XIVMathf.Repeat(diff + 540f, 360f) - 180f;
        //         targetAngle = currentAngle + repeatedAngle;
        //         transferIndicatorEntity.CancelTween();
        //         transferIndicatorEntity.XIVTween()
        //             .RotateZ(currentAngle, targetAngle, 1.1f - (dist / 720f), EasingFunction.EaseOutCubic)
        //             .Start();
        //     });
        //
        //     if (hasIndicator) return;
        //     
        //     var indicatorEntity = GameObjectEntity.CreateEntity(world, assetReferences.resourceTransferIndicatorPrefab, positionComp.position.ToVector3(), Quaternion.identity);
        //     ref var indicatorTransformComp = ref indicatorEntity.GetComponent<TransformComp>();
        //     ref var indicatorRotationComp = ref indicatorEntity.GetComponent<RotationComp>();
        //     indicatorTransformComp.transform.Rotate(Vector3.forward, targetAngle);
        //     indicatorRotationComp.Set(indicatorTransformComp.transform.rotation.eulerAngles.ToVec3());
        //
        //     var scale = indicatorTransformComp.transform.localScale;
        //     indicatorEntity.CancelTween();
        //     indicatorEntity.XIVTween()
        //         .Scale(Vector3.zero, scale, 0.25f, EasingFunction.EaseInBounce)
        //         .Start();
        //     
        //     indicatorEntity.AddComponent(new ResourceTransferIndicatorComp
        //     {
        //         owner = fromEntity,
        //         target = targetEntity,
        //     });
        // }
    }
}