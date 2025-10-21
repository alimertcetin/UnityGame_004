namespace XIV.Ecs
{
    public class AnimationEventSystem : XIV.Ecs.System
    {
        readonly Filter<AnimatorComp> animatorFilter = null;

        public override void PreUpdate()
        {
            animatorFilter.ForEach((ref AnimatorComp animatorComp) =>
            {
                animatorComp.animationEvents.Clear();
                foreach (var eventName in animatorComp.animatorListener.animationEvents)
                {
                    animatorComp.animationEvents.Add(eventName);
                }
                animatorComp.animatorListener.animationEvents.Clear();
            });
        }
    }
}