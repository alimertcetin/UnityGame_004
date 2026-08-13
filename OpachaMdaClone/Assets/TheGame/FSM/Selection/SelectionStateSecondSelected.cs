using XIV.Ecs;

namespace TheGame
{
    public class SelectionStateSecondSelected : SelectionState
    {
        public SelectionStateSecondSelected(SelectionFsmManager manager) : base(manager)
        {
        }

        public override void Update(ref SingleInputData singleInput, SwipeResult swipe)
        {
            manager.TransferOnce();

            // Set new selection
            manager.second = Entity.Invalid;
            manager.ChangeState<SelectionStateDeselectFirstSelected>();
        }
    }
}