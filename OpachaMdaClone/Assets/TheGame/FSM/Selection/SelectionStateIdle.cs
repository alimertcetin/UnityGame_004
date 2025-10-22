using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;

namespace TheGame
{
    public class SelectionStateIdle : SelectionState
    {
        public SelectionStateIdle(SelectionFsmManager manager) : base(manager)
        {
        }

        public override void Update(ref InputData input, SwipeResult swipe)
        {
            if (input.isFingerDownThisFrameNoUI && manager.TryGetFirstFromInput(ref input, out Entity first))
            {
                manager.first = first;
                manager.ChangeState<SelectionStateFirstSelected>();
                return;
            }
        }
    }
}