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
        Transform highlightEntityTransform;

        public override void Awake()
        {
            var nodeHighlightEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeHighlightEntity);
            highlightEntityTransform = nodeHighlightEntity.GetComponent<TransformComp>().transform;
            highlightEntityTransform.gameObject.SetActive(false);
            nodeHighlightEntity.Unbind();
        }

        public override void Update()
        {
            enableHighlightFilter.ForEach((Entity entity, ref EnableHighlightEventComp enableHighlightEventComp) =>
            {
                entity.Destroy();

                var transform = enableHighlightEventComp.targetEntity.GetUnityComponent<Transform>();
                var pos = transform.position;
                var rot = transform.rotation;
                var scale = highlightEntityTransform.gameObject.activeSelf ? Vector3.one * 1.2f : Vector3.one;
                highlightEntityTransform.position = pos;
                highlightEntityTransform.rotation = rot;
                highlightEntityTransform.localScale = scale;
                var nodeHighlightEntity = GameObjectEntity.BindGameObjectToEntity(world, highlightEntityTransform.gameObject);
                highlightEntityTransform.gameObject.SetActive(true);

                ref var highlightComp = ref nodeHighlightEntity.GetComponent<HighlightComp>();
                highlightComp.owner = enableHighlightEventComp.targetEntity;
                nodeHighlightEntity.CancelTween();
                nodeHighlightEntity.XIVTween()
                    .Scale(scale, scale * 1.2f, 1f, EasingFunction.SmoothStop2, true, int.MaxValue)
                    .UseCustomDeltaTime(() => XTime.deltaTime)
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
                        highlightEntityTransform.gameObject.SetActive(false);
                    }
                });
            });
        }
    }
}