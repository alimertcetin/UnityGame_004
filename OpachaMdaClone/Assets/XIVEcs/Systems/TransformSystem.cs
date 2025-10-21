using UnityEngine;

namespace XIV.Ecs
{
    // TODO remove transform component owner, add new component PooledComp add custom reset to pool comp 
    // BUT can we implement it with custom reset
    
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
                if (comp.gameObjectEntity.owner != null)
                {
                    comp.gameObjectEntity.entity = Entity.Invalid;
                    comp.gameObjectEntity.owner.ReleaseView(comp.gameObjectEntity);
                }
                else
                {
                    GameObject.Destroy(comp.gameObjectEntity.gameObject);
                }
                
            }

            comp = default;
        }
    }
}