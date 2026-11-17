using System;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class NodeBehaviourUISystem : XIV.Ecs.System
    {
        readonly Filter<NodeSelectionListenerComp, ButtonComp, NodeBehaviourButtonComp> nodeBehaviourButtonClickedFilter = new Filter<NodeSelectionListenerComp, ButtonComp, NodeBehaviourButtonComp>().Tag<ButtonClickedTag>();
        readonly Filter<NodeSelectionListenerComp, NodeStateManagementUIComp> nodeStateManagementUIFilter = null;

        public override void Start()
        {
            nodeStateManagementUIFilter.ForEach(HideUI);
        }
        
        void HideUI(Entity entity, ref NodeSelectionListenerComp nodeSelectionListenerComp, ref NodeStateManagementUIComp uiComp)
        {
            var isUiOpen = uiComp.container.gameObject.activeSelf;
            var pos = uiComp.container.anchoredPosition;
            bool isSelectionAlive = nodeSelectionListenerComp.selectedNode.IsAlive();
            var targetPos = isSelectionAlive ? uiComp.visiblePosition : uiComp.hiddenPosition;
            
            var maxDistance = Vector2.Distance(uiComp.visiblePosition, uiComp.hiddenPosition);
            var distanceToTargetPos = Vector2.Distance(pos, targetPos);
            var normalizedDistance = distanceToTargetPos / maxDistance;
            var duration = XIVMathf.Lerp(0f, 0.5f, normalizedDistance);

            if (isUiOpen)
            {
                uiComp.container.CancelTween(false);
                uiComp.container.XIVTween()
                    .RectTransformMove(pos, targetPos, duration, EasingFunction.EaseOutExpo)
                    .OnComplete(go => go.SetActive(false))
                    .Start();
                return;
            }
            
            uiComp.container.gameObject.SetActive(true);
            uiComp.container.CancelTween(false);
            uiComp.container.XIVTween()
                .RectTransformMove(pos, targetPos, duration, EasingFunction.EaseOutExpo)
                .Start();
        }

        public override void Update()
        {
            nodeStateManagementUIFilter.ForEach(HandleUIDisplay);
            nodeBehaviourButtonClickedFilter.ForEach((Entity entity, ref NodeSelectionListenerComp nodeSelectionListenerComp, ref ButtonComp buttonComp, ref NodeBehaviourButtonComp nodeBehaviourButtonComp) =>
            {
                entity.CancelTween();
                entity.XIVTween()
                    .ScaleBounceOnce()
                    .Start();

                var selectedNode = nodeSelectionListenerComp.selectedNode;
                if (selectedNode.IsAlive() == false) return;
                
                void SendChangeTypeEvent(int config)
                {
                    world.NewEntity().AddComponent(new NodeChangeTypeEventComp
                    {
                        newConfig = config,
                        nodeEntity = selectedNode,
                        penalty = 10,
                        unitEntity = selectedNode.GetComponent<OccupiedNodeComp>().unitEntity,
                        unitEpoch = selectedNode.GetComponent<NodeComp>().unitEpoch,
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
                    // case NodeButtonBehaviourType.SEND_HALF_RESOURCE:
                    //     break;
                    default:
                        Debug.LogError(nodeBehaviourButtonComp.buttonBehaviourType + " is not supported.");
                        break;
                }
            });
        }
        
        void HandleUIDisplay(Entity entity, ref NodeSelectionListenerComp nodeSelectionListenerComp, ref NodeStateManagementUIComp uiComp)
        {
            if (uiComp.selectionVersion == nodeSelectionListenerComp.version) return;
            uiComp.selectionVersion = nodeSelectionListenerComp.version;
            uiComp.container.CancelTween();
            uiComp.container.gameObject.SetActive(true);

            bool isSelectionAlive = nodeSelectionListenerComp.selectedNode.IsAlive();
            Vector2 pos;
            Vector2 targetPos;
            if (isSelectionAlive)
            {
                pos = uiComp.hiddenPosition;
                targetPos = uiComp.visiblePosition;
            }
            else
            {
                pos = uiComp.visiblePosition;
                targetPos = uiComp.hiddenPosition;
            }

            var maxDistance = Vector2.Distance(uiComp.visiblePosition, uiComp.hiddenPosition);
            var distanceToTargetPos = Vector2.Distance(pos, targetPos);
            var normalizedDistance = distanceToTargetPos / maxDistance;
            var duration = XIVMathf.Lerp(0f, 0.5f, normalizedDistance);

            if (pos.IsNaN() || targetPos.IsNaN())
            {
                Debug.LogError("Position is NaN or target position is NaN.");
                Debug.Log("pos = " + pos);
                Debug.Log("target = " + targetPos);
                pos = default;
                entity.GetRectTransform().anchoredPosition = pos;
            }

            if (isSelectionAlive == false)
            {
                uiComp.container.XIVTween()
                    .RectTransformMove(pos, targetPos, duration, EasingFunction.EaseOutExpo)
                    .OnComplete(go => go.SetActive(false))
                    .Start();
                return;
            }
            
            uiComp.container.XIVTween()
                .RectTransformMove(pos, targetPos, duration, EasingFunction.EaseOutExpo)
                .Start();

        }
    }

}