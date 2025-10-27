using UnityEngine;

namespace XIV.Ecs
{
    public class TransformSystem : XIV.Ecs.System
    {
        public override void PreAwake()
        {
            world.SetCustomReset<TransformComp>(CustomReset);
        }
        
        static void CustomReset(ref TransformComp comp)
        {
            if (comp.gameObjectEntity != null)
            {
                Object.Destroy(comp.gameObjectEntity.gameObject);
            }

            comp = default;
        }
    }
}