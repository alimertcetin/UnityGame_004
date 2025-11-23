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

    // public struct DoubleTapComp : IComponent
    // {
    //     public Timer timer;
    // }

    public class UnitNodeSelectionSystem : XIV.Ecs.System
    {
        readonly Filter<UnitComp, InputListenerComp> nodeSelectorFilter = null;
        // readonly Filter<DoubleTapComp> doubleTapFilter = null;
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        SelectionFsmManager selectionFsmManager = null;

        public override void Awake()
        {
            selectionFsmManager = new SelectionFsmManager(connectionDB, assetReferences);
        }

        public override void Update()
        {
            // doubleTapFilter.ForEach((Entity entity, ref DoubleTapComp doubleTapComp) =>
            // {
            //     if (doubleTapComp.timer.Update(XTime.deltaTime)) entity.RemoveComponent<DoubleTapComp>();
            // });

            InputData input = default;
            nodeSelectorFilter.ForEach((Entity selectorEntity, ref UnitComp unitComp, ref InputListenerComp listener) =>
            {
                input = listener.input;
            });
            selectionFsmManager.Run(ref input);
        }
    }
}