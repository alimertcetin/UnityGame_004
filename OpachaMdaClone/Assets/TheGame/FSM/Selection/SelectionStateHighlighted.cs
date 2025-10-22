using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;

namespace TheGame
{
    public class SelectionStateHighlighted : SelectionState
    {
        public SelectionStateHighlighted(SelectionFsmManager manager) : base(manager)
        {
        }

        public override void Start()
        {
            manager.Highlight(manager.first, true);
        }

        public override void Update(ref InputData input, SwipeResult swipe)
        {
            if (input.isFingerDownThisFrameNoUI)
            {
                if (manager.TryGetEntityFromInput(ref input, out var entity) == false)
                {
                    // Clicked empty → unhighlight
                    manager.ChangeState<SelectionStateDeselectFirstSelected>();
                    return;
                }

                if (manager.TryGetSecondFromInput(ref input, out var second))
                {
                    // Clicked another → transfer half
                    manager.second = second;
                    manager.TransferOnce(false);
                }
                // else
                // {
                //     // manager.Highlight(manager.first, false);
                //     // manager.ChangeState<SelectionStateDeselectFirstSelected>();
                // }
            }

            // Swipe detection
            if (input.isFingerUpThisFrame)
            {
                if (swipe.direction != SwipeResult.Direction.None)
                {
                    manager.second = manager.GetPossibleTarget(swipe.directionVector);
                    if (manager.second.IsAlive())
                    {
                        manager.StartContinuousTransfer();
                        manager.ChangeState<SelectionStateDeselectFirstSelected>();
                        return;
                    }
                }
            }
        }
    }
}