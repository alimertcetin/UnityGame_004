using TheGame.SceneManagement;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class LevelCompletedSystem : XIV.Ecs.System
    {
        readonly AssetReferences assetReferences;
        readonly Filter entityFilter = null;
        Timer waitTimer = new Timer(XIVMathf.Lerp(1f, 3f, XIVRandom.value));
        
        public override void Update()
        {
            if (waitTimer.Update(XTime.unscaledDeltaTime) == false) return;
            
            manager.ChangeState(EasyLevelController.States.EndGame);
            
            entityFilter.DestroyAll();
            SceneLoader.instance.LoadScene(assetReferences.sceneList.GetSceneByContainingSceneName("MainMenu"), SceneLoadSettings.GetDefault());
            SceneLoader.instance.UnloadScene(assetReferences.sceneList.GetSceneByContainingSceneName("GameScene"), SceneUnloadSettings.GetDefault());
        }
    }
}