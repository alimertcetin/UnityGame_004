using XIVEcsUnityIntegration.Extensions;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public class UnitNodeSelectionSystem : XIV.Ecs.System
    {
        readonly Filter<UnitComp, InputListenerComp> nodeSelectorFilter = null;
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        SelectionFsmManager selectionFsmManager = null;

        public override void Awake()
        {
            selectionFsmManager = new SelectionFsmManager(connectionDB, assetReferences);
        }

        public override void Update()
        {
            InputData input = default;
            nodeSelectorFilter.ForEach((Entity selectorEntity, ref UnitComp unitComp, ref InputListenerComp listener) =>
            {
                input = listener.input;
            });
            selectionFsmManager.Run(ref input);
        }
    }
}