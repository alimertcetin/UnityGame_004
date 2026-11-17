using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;

namespace TheGame
{
    public class NodeHighlightSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeSelectionListenerComp, HighlightComp> highlightFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Awake()
        {
            var nodeHighlightEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeHighlightEntity);
            nodeHighlightEntity.GetTransform().gameObject.SetActive(false);
        }

        public override void Update()
        {
            highlightFilter.ForEach((Entity highlightEntity, ref TransformComp transformComp, ref NodeSelectionListenerComp nodeSelectionListenerComp, ref HighlightComp highlightComp) =>
            {
                if (highlightComp.version == nodeSelectionListenerComp.version) return;
                highlightComp.version = nodeSelectionListenerComp.version;

                if (nodeSelectionListenerComp.selectedNode == Entity.Invalid)
                {
                    highlightComp.highlightedEntity = Entity.Invalid;
                    highlightEntity.CancelTween();
                    highlightEntity.GetTransform().gameObject.SetActive(false);
                    return;
                }

                var transform = nodeSelectionListenerComp.selectedNode.GetComponent<TransformComp>().transform;
                var pos = transform.position;
                var rot = transform.rotation;
                var highlightEntityTransform = transformComp.transform;
                var scale = highlightComp.highlightedEntity == nodeSelectionListenerComp.previousSelectedNode ? Vector3.one * 1.2f : Vector3.one;

                highlightEntityTransform.position = pos;
                highlightEntityTransform.rotation = rot;
                highlightEntityTransform.localScale = scale;
                highlightEntityTransform.gameObject.SetActive(true);

                highlightComp.highlightedEntity = nodeSelectionListenerComp.selectedNode;
                highlightEntity.CancelTween();
                highlightEntity.XIVTween()
                    .Scale(scale, scale * 1.2f, 1f, EasingFunction.SmoothStop2, true, int.MaxValue)
                    .UseCustomDeltaTime(() => XTime.unscaledDeltaTime)
                    .Start();
            });
        }

    }
}