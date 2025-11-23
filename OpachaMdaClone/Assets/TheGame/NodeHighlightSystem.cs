using XIVEcsUnityIntegration.Extensions;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct EnableHighlightTag : ITag { }
    public struct DisableHighlightTag : ITag { }

    public class NodeHighlightSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp> enableHighlightFilter = new Filter<TransformComp, NodeComp>().Tag<EnableHighlightTag>();
        readonly Filter<TransformComp, NodeComp> disableHighlightFilter = new Filter<TransformComp, NodeComp>().Tag<DisableHighlightTag>();
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
            enableHighlightFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp) =>
            {
                var pos = transformComp.transform.position;
                var rot = transformComp.transform.rotation;
                var scale = highlightEntityTransform.gameObject.activeSelf ? Vector3.one * 1.2f : Vector3.one;
                highlightEntityTransform.position = pos;
                highlightEntityTransform.rotation = rot;
                highlightEntityTransform.localScale = scale;
                var nodeHighlightEntity = GameObjectEntity.BindGameObjectToEntity(world, highlightEntityTransform.gameObject);
                highlightEntityTransform.gameObject.SetActive(true);
                
                ref var highlightComp = ref nodeHighlightEntity.GetComponent<HighlightComp>();
                highlightComp.owner = nodeEntity;
                nodeHighlightEntity.CancelTween();
                nodeHighlightEntity.XIVTween()
                    .Scale(scale, scale * 1.2f, 1f, EasingFunction.SmoothStop2, true, int.MaxValue)
                    .Start();
            });
            enableHighlightFilter.RemoveTagAll<EnableHighlightTag>();
            
            disableHighlightFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp) =>
            {
                highlightFilter.ForEach((Entity highlightEntity, ref HighlightComp highlightComp) =>
                {
                    // TODO: NodeHighlightSystem -> We don't need this
                    if (highlightComp.owner == nodeEntity)
                    {
                        highlightComp.owner = Entity.Invalid;
                        highlightEntity.CancelTween();
                        highlightEntityTransform.gameObject.SetActive(false);
                    }
                });
            });
            disableHighlightFilter.RemoveTagAll<DisableHighlightTag>();
        }
    }
}