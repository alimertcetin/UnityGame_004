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

        public override void Update(ref SingleInputData singleInput, SwipeResult swipe)
        {
            if (singleInput.isFingerDownThisFrameNoUI && manager.TryGetFirstFromInput(ref singleInput, out Entity first))
            {
                manager.first = first;
                manager.ChangeState<SelectionStateFirstSelected>();
                return;
            }
        }
    }
}