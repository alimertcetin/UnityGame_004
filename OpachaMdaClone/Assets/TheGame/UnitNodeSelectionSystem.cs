using XIVEcsUnityIntegration.Extensions;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct SendResourceComp : IComponent
    {
        public int resourceQuantity;
        public Entity toEntity;
    }

    public struct SendResourceContinuouslyComp : IComponent
    {
        public float duration;
        public float currentDuration;
        public Entity toEntity;
    }

    public struct DoubleTapComp : IComponent
    {
        public float duration;
    }

    public class UnitNodeSelectionSystem : XIV.Ecs.System
    {
        readonly Filter<UnitComp, InputListenerComp> nodeSelectorFilter = null;
        readonly Filter<DoubleTapComp> doubleTapFilter = null;
        readonly ConnectionDB connectionDB = null;
        SelectionFsmManager selectionFsmManager = null;

        public override void Awake()
        {
            selectionFsmManager = new SelectionFsmManager(connectionDB);
        }

        public override void Update()
        {
            doubleTapFilter.ForEach((Entity entity, ref DoubleTapComp doubleTapComp) =>
            {
                doubleTapComp.duration -= XTime.deltaTime;
                if (doubleTapComp.duration <= 0) entity.RemoveComponent<DoubleTapComp>();
            });

            InputData input = default;
            nodeSelectorFilter.ForEach((Entity selectorEntity, ref UnitComp unitComp, ref InputListenerComp listener) =>
            {
                input = listener.input;
            });
            selectionFsmManager.Run(ref input);
        }
    }
}