using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct ResourceTransferIndicatorComp : IComponent
    {
        public Entity owner;
        public Entity target;
    }
    
    public class ResourceTransferIndicatorSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, RotationComp, ScaleComp, ResourceTransferIndicatorComp> resourceTransferIndicatorFilter = null;
        readonly Filter<NodeComp> nodeFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Start()
        {
            nodeFilter.ForEach((Entity nodeEntity) =>
            {
                var position = nodeEntity.GetTransform().position;
                var indicatorEntity = GameObjectEntity.CreateEntity(world, assetReferences.resourceTransferIndicatorPrefab, position, Quaternion.identity);
                
                indicatorEntity.AddComponent(new ResourceTransferIndicatorComp
                {
                    owner = nodeEntity,
                    target = Entity.Invalid,
                });
                indicatorEntity.GetGameObject().SetActive(false);
            });
        }

        public override void Update()
        {
            resourceTransferIndicatorFilter.ForEach((Entity indicatorEntity, ref PositionComp positionComp, ref RotationComp rotationComp, ref ScaleComp scaleComp, ref ResourceTransferIndicatorComp resourceTransferIndicatorComp) =>
            {
                var scale = scaleComp.scale.ToVector3();
                bool isIndicatorActive = indicatorEntity.GetGameObject().activeSelf;
                if (resourceTransferIndicatorComp.owner.HasComponent<SendResourceContinuouslyComp>() == false)
                {
                    resourceTransferIndicatorComp.target = Entity.Invalid;
                    if (isIndicatorActive == false) return;
                    indicatorEntity.CancelTween();
                    indicatorEntity.XIVTween()
                        .Scale(scale, Vector3.zero, 1.25f, EasingFunction.EaseOutBack)
                        .OnComplete((go) => go.SetActive(false))
                        .UseCustomDeltaTime(() => XTime.deltaTime)
                        .Start();
                    return;
                }
                
                var targetEntity = resourceTransferIndicatorComp.owner.GetComponent<SendResourceContinuouslyComp>().toEntity;
                if (targetEntity == resourceTransferIndicatorComp.target) return;
                resourceTransferIndicatorComp.target = targetEntity;
                
                ref var targetPositionComp = ref targetEntity.GetComponent<PositionComp>();
                var dirToTarget = targetPositionComp.position - positionComp.position;
                var targetAngle = Vector3.SignedAngle(Vector3.right, dirToTarget.ToVector3(), Vector3.forward);
                    
                var currentAngle = rotationComp.eulerAngles.z;
                rotationComp.rotZ = targetAngle;
                var diff = targetAngle - currentAngle;
                var dist = XIVMathf.Abs(diff);
                
                float repeatedAngle = XIVMathf.Repeat(diff + 540f, 360f) - 180f;
                targetAngle = currentAngle + repeatedAngle;
                indicatorEntity.CancelTween();
                indicatorEntity.GetGameObject().SetActive(true);

                if (isIndicatorActive == false) // indicator was not active, this is the first appearance
                {
                    currentAngle = targetAngle;
                }
                
                indicatorEntity.XIVTween()
                    .RotateZ(currentAngle, targetAngle, 1.1f - (dist / 720f), EasingFunction.EaseOutCubic)
                    .And()
                    .Scale(scale, Vector3.one * 2f, 1.25f, EasingFunction.EaseOutBack)
                    .UseCustomDeltaTime(() => XTime.deltaTime)
                    .Start();
            });
            
        }
    }
}