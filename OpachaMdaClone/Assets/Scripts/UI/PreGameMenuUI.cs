using System.Collections;
using TheGame.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using XIV.Ecs;

namespace TheGame
{
    public class PreGameMenuUI : PageUI
    {
        public SceneListSO sceneList;
        public Button btn_Skirmish;
        public Button btn_Back;

        void OnEnable()
        {
            btn_Skirmish.onClick.AddListener(() => StartCoroutine(OnSkirmishButtonClicked()));
            btn_Back.onClick.AddListener(OnBackButtonClicked);
        }

        void OnDisable()
        {
            btn_Skirmish.onClick.RemoveAllListeners();
            btn_Back.onClick.RemoveAllListeners();
        }

        IEnumerator OnSkirmishButtonClicked()
        {
            base.parentUI.Close();
            yield return new WaitForSeconds(base.parentUI.animationDuration);
            var levelSettingsMono = FindObjectOfType<LevelSettingsMono>();
            levelSettingsMono.levelSettings ??= new LevelSettings();
            levelSettingsMono.levelSettings.levelGenerationSettings = LevelGenerationSettings.CreateRandom();
            SceneLoader.loadScene?.Invoke(sceneList.GetSceneByContainingSceneName("GameScene"), SceneLoadSettings.GetDefault());
            SceneLoader.onSceneLoadComplete += OnGameSceneLoadComplete;
        }

        void OnGameSceneLoadComplete(SceneSO obj, SceneLoadSettings settings)
        {
            SceneLoader.onSceneLoadComplete -= OnGameSceneLoadComplete;
            new GameObject("EasyLevelController").AddComponent<EasyLevelController>();
        }

        void OnBackButtonClicked()
        {
            parentUI.SwitchPage(parentUI.mainPage);
        }
    }
}