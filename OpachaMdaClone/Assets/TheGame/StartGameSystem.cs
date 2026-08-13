using TheGame.SceneManagement;
using XIV.Ecs;

namespace TheGame
{
    public class StartGameSystem : XIV.Ecs.System
    {
        readonly AssetReferences assetReferences = null;
        
        public override void Update()
        {
            if (SceneLoader.instance.lastLoadedScene != assetReferences.sceneList.GetSceneByContainingSceneName("GameScene")) return;
            
            manager.ChangeState(EasyLevelController.States.InitializeNodes);
        }
    }
}