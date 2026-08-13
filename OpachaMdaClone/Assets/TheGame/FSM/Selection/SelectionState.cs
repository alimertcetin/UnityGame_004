using XIV.Ecs;

namespace TheGame
{
    public abstract class SelectionState
    {
        protected readonly SelectionFsmManager manager;

        public SelectionState(SelectionFsmManager manager)
        {
            this.manager = manager;
        }

        public virtual void Start()
        {
        }

        public virtual void Update(ref SingleInputData singleInput, SwipeResult swipe)
        {
        }

        public virtual void BeforeStateChange()
        {
        }
    }
}