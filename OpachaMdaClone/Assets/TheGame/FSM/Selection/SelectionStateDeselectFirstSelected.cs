using XIV.Ecs;

namespace TheGame
{
    public class SelectionStateDeselectFirstSelected : SelectionState
    {
        public SelectionStateDeselectFirstSelected(SelectionFsmManager manager) : base(manager)
        {
        }

        public override void Start()
        {
            manager.Highlight(manager.first, false);
            manager.first = Entity.Invalid;
            manager.ChangeState<SelectionStateIdle>();
        }
    }
}