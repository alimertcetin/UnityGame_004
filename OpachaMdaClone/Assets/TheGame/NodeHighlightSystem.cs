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
        readonly PrefabReferences prefabReferences;

        public override void Update()
        {
            enableHighlightFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp) =>
            {
                var pos = transformComp.transform.position;
                var rot = transformComp.transform.rotation;
                
                var nodeHighlightEntity = GameObjectEntity.CreateEntity(world, prefabReferences.nodeHighlightEntity, pos, rot);
                ref var highlightTransformComp = ref nodeHighlightEntity.GetComponent<TransformComp>();
                var scale = highlightTransformComp.transform.localScale;
                ref var highlightComp = ref nodeHighlightEntity.GetComponent<HighlightComp>();
                highlightComp.owner = nodeEntity;
                nodeHighlightEntity.XIVTween()
                    .Scale(scale, scale * 1.2f, 1f, EasingFunction.SmoothStop2, true, int.MaxValue)
                    .Start();
            });
            enableHighlightFilter.RemoveTagAll<EnableHighlightTag>();
            
            disableHighlightFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp) =>
            {
                highlightFilter.ForEach((Entity highlightEntity, ref HighlightComp highlightComp) =>
                {
                    if (highlightComp.owner == nodeEntity) highlightEntity.Destroy();
                });
            });
            disableHighlightFilter.RemoveTagAll<DisableHighlightTag>();
        }
    }
}