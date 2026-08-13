using System;

namespace XIV.Ecs
{
    public struct CallLaterComp : IComponent
    {
        public float timer;
        public Action<Entity> action;
    }
    
    public class CallLaterSystem : XIV.Ecs.System
    {
        readonly Filter<CallLaterComp> callLaterFilter = null;

        public override void PreUpdate()
        {
            callLaterFilter.ForEach((Entity entity, ref CallLaterComp callLaterComp) =>
            {
                if (callLaterComp.timer <= 0)
                {
                    entity.RemoveComponent<CallLaterComp>();
                    callLaterComp.action?.Invoke(entity);
                }
                else
                {
                    callLaterComp.timer -= XTime.deltaTime;
                }
                
            });
           
        }
    }
}