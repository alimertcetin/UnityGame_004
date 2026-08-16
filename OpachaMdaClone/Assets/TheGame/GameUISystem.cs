using TheGame.SceneManagement;
using XIV.Ecs;

namespace TheGame
{
    public class GameUISystem : XIV.Ecs.System
    {
        readonly Filter<ButtonComp> btnBackToMainMenuFilter = new Filter<ButtonComp>().Tag<ButtonBackToMainMenuTag>().Tag<ButtonClickedTag>();
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            btnBackToMainMenuFilter.ForEach((Entity e) =>
            {
                manager.ChangeState(EasyLevelController.States.EndGame);
                SceneLoader.instance.LoadScene(assetReferences.sceneList.GetSceneByContainingSceneName("MainMenu"), SceneLoadSettings.GetDefault());
                SceneLoader.instance.UnloadScene(assetReferences.sceneList.GetSceneByContainingSceneName("GameScene"), SceneUnloadSettings.GetDefault());
            });
        }
    }
}