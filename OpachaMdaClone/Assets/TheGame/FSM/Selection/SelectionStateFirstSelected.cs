using UnityEngine;
using XIV.Core.Utils;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;

namespace TheGame
{
    public class SelectionStateFirstSelected : SelectionState
    {
        const float LONG_PRESS_TIME = 0.5f;

        public SelectionStateFirstSelected(SelectionFsmManager manager) : base(manager)
        {
        }

        public override void Start()
        {
            manager.Highlight(manager.first, true);
        }

        public override void Update(ref InputData input, SwipeResult swipe)
        {
            if (input.isFingerDownNoUI && input.inputDuration >= LONG_PRESS_TIME)
            {
                manager.ChangeState<SelectionStateHighlighted>();
                return;
            }

            // Swipe detection
            if (input.isFingerDownNoUI)
            {
                if (swipe.direction != SwipeResult.Direction.None)
                {
                    manager.second = manager.GetPossibleTarget(swipe.directionVector);
                    // TODO : SelectionStateFirstSelected -> Do we care if it is alive or not to change the state?
                    if (manager.second.IsAlive())
                    {
                        manager.StartContinuousTransfer();
                        manager.ChangeState<SelectionStateDeselectFirstSelected>();
                        return;
                    }
                }
            }

            // Finger released → click detection
            if (input.isFingerDownThisFrameNoUI)
            {
                if (manager.TryGetEntityFromInput(ref input, out var entity))
                {
                    // TODO : SelectionStateFirstSelected -> Are we trying to retrieve second? if so why we are comparing it to first?
                    // Clicked on same
                    if (entity == manager.first)
                    {
                        manager.StopContinuousTransfer();
                        manager.ChangeState<SelectionStateDeselectFirstSelected>();
                        return;
                    }

                    // Clicked another node
                    manager.second = entity;
                    manager.ChangeState<SelectionStateSecondSelected>();
                    return;
                }
                else
                {
                    // Released on empty space
                    manager.ChangeState<SelectionStateDeselectFirstSelected>();
                    return;
                }
            }
        }
    }
}