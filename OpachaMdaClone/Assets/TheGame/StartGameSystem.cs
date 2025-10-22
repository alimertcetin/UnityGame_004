using XIV.Ecs;

namespace TheGame
{
    public class StartGameSystem : XIV.Ecs.System
    {
        public override void Update()
        {
            manager.ChangeState(LevelController.States.Game);
        }
    }
}