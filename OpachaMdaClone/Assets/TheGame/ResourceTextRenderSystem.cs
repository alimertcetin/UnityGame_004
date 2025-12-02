using XIV.Ecs;

namespace TheGame
{
    public class ResourceTextRenderSystem : XIV.Ecs.System
    {
        readonly Filter<ResourceComp, TextComp> resourceDisplayFilter = null;
        readonly Filter<ResourceComp, TextComp> resourceDisplayOnChangeFilter = new Filter<ResourceComp, TextComp>().Tag<UpdateResourceQuantityTextTag>();

        public override void Start()
        {
            resourceDisplayFilter.ForEach(DisplayResourceQuantity);
        }

        public override void Update()
        {
            resourceDisplayOnChangeFilter.ForEach(DisplayResourceQuantity);
        }

        void DisplayResourceQuantity(Entity entity, ref ResourceComp resourceComp, ref TextComp textComp)
        {
            textComp.txt.WriteScoreText((int)resourceComp.resourceQuantity);
            entity.RemoveTag<UpdateResourceQuantityTextTag>();
        }
    }
}