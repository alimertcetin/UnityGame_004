using XIVEcsUnityIntegration.Extensions;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct EnableHighlightEventComp : IComponent
    {
        public Entity targetEntity;
    }

    public struct DisableHighlightEventComp : IComponent
    {
        public Entity targetEntity;
    }

    public class NodeHighlightSystem : XIV.Ecs.System
    {
        readonly Filter<EnableHighlightEventComp> enableHighlightFilter = null;
        readonly Filter<DisableHighlightEventComp> disableHighlightFilter = null;
        readonly Filter<HighlightComp> highlightFilter = new Filter<HighlightComp>();
        readonly AssetReferences assetReferences;
        Entity nodeHighlightEntity;

        public override void Awake()
        {
            nodeHighlightEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeHighlightEntity);
            nodeHighlightEntity.GetTransform().gameObject.SetActive(false);
            nodeHighlightEntity.RemoveComponent<ScaleComp>();
        }

        public override void OnDestroy()
        {
            if (nodeHighlightEntity.IsAlive() == false) return;
            nodeHighlightEntity.Destroy();
        }

        public override void Update()
        {
            enableHighlightFilter.ForEach((Entity entity, ref EnableHighlightEventComp enableHighlightEventComp) =>
            {
                entity.Destroy();

                var transform = enableHighlightEventComp.targetEntity.GetTransform();
                var pos = transform.position;
                var rot = transform.rotation;
                var highlightEntityTransform = nodeHighlightEntity.GetTransform();
                var scale = highlightEntityTransform.gameObject.activeSelf ? Vector3.one * 1.2f : Vector3.one;

                ref var positionComp = ref nodeHighlightEntity.GetComponent<PositionComp>();
                ref var rotationComp = ref nodeHighlightEntity.GetComponent<RotationComp>();
                positionComp.Set(pos.ToVec3());
                rotationComp.Set(rot.eulerAngles.ToVec3());
                
                highlightEntityTransform.localScale = scale;
                highlightEntityTransform.gameObject.SetActive(true);

                ref var highlightComp = ref nodeHighlightEntity.GetComponent<HighlightComp>();
                highlightComp.owner = enableHighlightEventComp.targetEntity;
                nodeHighlightEntity.CancelTween();
                nodeHighlightEntity.XIVTween()
                    .Scale(scale, scale * 1.2f, 1f, EasingFunction.SmoothStop2, true, int.MaxValue)
                    .UseCustomDeltaTime(() => XTime.unscaledDeltaTime)
                    .Start();
            });
            
            disableHighlightFilter.ForEach((Entity entity, ref DisableHighlightEventComp disableHighlightEventComp) =>
            {
                entity.Destroy();
                var targetEntity = disableHighlightEventComp.targetEntity;
                highlightFilter.ForEach((Entity highlightEntity, ref HighlightComp highlightComp) =>
                {
                    // TODO: NodeHighlightSystem -> We don't need this
                    if (highlightComp.owner == targetEntity)
                    {
                        highlightComp.owner = Entity.Invalid;
                        highlightEntity.CancelTween();
                        nodeHighlightEntity.GetTransform().gameObject.SetActive(false);
                    }
                });
            });
        }
    }
}