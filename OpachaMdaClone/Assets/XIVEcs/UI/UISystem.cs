namespace XIV.Ecs
{
    public class UISystem : XIV.Ecs.System
    {
        readonly Filter<ButtonComp> buttonFilter = null;
        readonly Filter clickedFilter = new Filter().Tag<ButtonClickedTag>();
        
        public override void PreUpdate()
        {
            clickedFilter.RemoveTagAll<ButtonClickedTag>();
            
            buttonFilter.ForEach((Entity entity,ref ButtonComp buttonComp) =>
            {
                if (!buttonComp.buttonMono.clicked)
                {
                    return;
                }
                buttonComp.buttonMono.clicked = false;
                entity.AddTag<ButtonClickedTag>();
            });
        }
    }
}