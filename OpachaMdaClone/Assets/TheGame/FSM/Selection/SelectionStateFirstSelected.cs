using XIV.Ecs;

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

        public override void Update(ref SingleInputData singleInput, SwipeResult swipe)
        {
            if (singleInput.isFingerDownNoUI && singleInput.inputDuration >= LONG_PRESS_TIME)
            {
                manager.ChangeState<SelectionStateHighlighted>();
                return;
            }

            // Swipe detection
            if (singleInput.isFingerDownNoUI)
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

            // Finger released → click detection
            if (singleInput.isFingerDownThisFrameNoUI)
            {
                if (manager.TryGetEntityFromInput(ref singleInput, out var entity))
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