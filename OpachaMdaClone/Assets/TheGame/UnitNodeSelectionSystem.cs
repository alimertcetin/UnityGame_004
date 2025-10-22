using XIVEcsUnityIntegration.Extensions;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct SendResourceComp : IComponent
    {
        public Entity toNode;
        public int resourceQuantity;
    }

    public struct SendResourceContinuouslyComp : IComponent
    {
        public Entity toNode;
        public float duration;
        public float currentDuration;
    }

    public struct DoubleTapComp : IComponent
    {
        public float duration;
    }

    public class UnitNodeSelectionSystem : XIV.Ecs.System
    {
        readonly Filter<UnitComp, InputListenerComp> nodeSelectorFilter = null;
        readonly Filter<DoubleTapComp> doubleTapFilter = null;
        readonly ConnectionDB connectionDb = null;
        SelectionFsmManager selectionFsmManager;

        public override void Awake()
        {
            selectionFsmManager = new SelectionFsmManager(connectionDb);
        }

        public override void Update()
        {
            doubleTapFilter.ForEach((Entity entity, ref DoubleTapComp doubleTapComp) =>
            {
                doubleTapComp.duration -= XTime.deltaTime;
                if (doubleTapComp.duration <= 0) entity.RemoveComponent<DoubleTapComp>();
            });

            nodeSelectorFilter.ForEach((Entity selectorEntity, ref UnitComp unitComp, ref InputListenerComp listener) =>
            {
                selectionFsmManager.Run(ref listener.input);
            });
        }
    }
}