using XIV.Ecs;

namespace TheGame
{
    public class ResourceTextRenderSystem : XIV.Ecs.System
    {
        readonly Filter<ResourceComp, TextComp> resourceDisplayFilter = null;

        public override void Start()
        {
            resourceDisplayFilter.ForEach(DisplayResourceQuantity);
        }

        public override void Update()
        {
            resourceDisplayFilter.ForEach(DisplayResourceQuantity);
        }

        void DisplayResourceQuantity(Entity entity, ref ResourceComp resourceComp, ref TextComp textComp)
        {
            textComp.txt.WriteScoreText((int)resourceComp.resourceQuantity);
        }
    }
}