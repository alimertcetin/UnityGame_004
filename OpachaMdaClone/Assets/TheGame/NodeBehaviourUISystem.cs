using System;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class NodeBehaviourUISystem : XIV.Ecs.System
    {
        readonly Filter<ButtonComp, NodeBehaviourButtonComp> nodeBehaviourButtonClickedFilter = new Filter<ButtonComp, NodeBehaviourButtonComp>().Tag<ButtonClickedTag>();
        readonly Filter<NodeStateManagementUIComp> nodeStateManagementUIFilter = null;
        readonly Filter<HighlightComp> highlightedNodeFilter = null;

        public override void Update()
        {
            HandleUIDisplay();
            nodeBehaviourButtonClickedFilter.ForEach((Entity entity, ref ButtonComp buttonComp, ref NodeBehaviourButtonComp nodeBehaviourButtonComp) =>
            {
                entity.CancelTween();
                entity.XIVTween().ScaleBounceOnce();
                
                Entity highlightedNodeEntity = Entity.Invalid;
                highlightedNodeFilter.ForEach((ref HighlightComp highlightComp) =>
                {
                    highlightedNodeEntity = highlightComp.owner;
                });

                if (highlightedNodeEntity.IsAlive() == false) return;
                
                void SendChangeTypeEvent(int config)
                {
                    world.NewEntity().AddComponent(new NodeChangeTypeEventComp
                    {
                        newConfig = config,
                        nodeEntity = highlightedNodeEntity,
                        penalty = 10,
                        unitEntity = highlightedNodeEntity.GetComponent<OccupiedNodeComp>().unitEntity,
                    });
                }
                
                switch (nodeBehaviourButtonComp.buttonBehaviourType)
                {
                    case NodeButtonBehaviourType.CHANGE_TYPE_DEFAULT:
                        SendChangeTypeEvent(0);
                        break;
                    case NodeButtonBehaviourType.CHANGE_TYPE_ADC:
                        SendChangeTypeEvent(1);
                        break;
                    case NodeButtonBehaviourType.CHANGE_TYPE_TANK:
                        SendChangeTypeEvent(2);
                        break;
                    case NodeButtonBehaviourType.SEND_HALF_RESOURCE:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        void HandleUIDisplay()
        {
            Entity highlightedNodeEntity = Entity.Invalid;
            highlightedNodeFilter.ForEach(((ref HighlightComp highlightComp) =>
            {
                highlightedNodeEntity = highlightComp.owner;
            }));
            
            nodeStateManagementUIFilter.ForEach((Entity entity, ref NodeStateManagementUIComp uiComp) =>
            {
                var uiContainer = uiComp.container;
                var isUiOpen = uiContainer.gameObject.activeSelf;

                uiComp.highlightedNodeEntity = highlightedNodeEntity;
                var isAlive = highlightedNodeEntity.IsAlive();
                var pos = uiContainer.anchoredPosition;
                var scale = uiContainer.sizeDelta;
                
                if (isAlive && isUiOpen == false)
                {
                    // display ui
                    uiContainer.gameObject.SetActive(true);
                    uiContainer.CancelTween();
                    uiContainer.XIVTween()
                        .RectTransformMove(pos.SetY(pos.y - scale.y), pos, 0.5f, EasingFunction.EaseOutElastic)
                        .Start();
                }

                if (isAlive == false && isUiOpen && uiContainer.HasTween() == false)
                {
                    // close ui
                    uiContainer.CancelTween();
                    uiContainer.XIVTween()
                        .RectTransformMove(pos, pos.SetY(pos.y - scale.y), 0.5f, EasingFunction.EaseOutQuart)
                        .OnComplete(() =>
                        {
                            uiContainer.anchoredPosition = pos;
                            uiContainer.gameObject.SetActive(false);
                        })
                        .Start();
                }
            });
        }
    }
}