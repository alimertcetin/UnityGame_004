using System;
using System.Collections;
using TheGame.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class PreGameMenuUI : PageUI
    {
        public SceneListSO sceneList;
        public Button btn_Skirmish;
        public Button btn_Back;
        public TMP_InputField inputField_Seed;
        public XIVRadioButtonGroup radioButtonGroup;
        public Slider hostileCountSlider;

        void OnEnable()
        {
            btn_Skirmish.onClick.AddListener(() => StartCoroutine(OnSkirmishButtonClicked()));
            btn_Back.onClick.AddListener(OnBackButtonClicked);
            inputField_Seed.onValueChanged.AddListener(OnSeedInputChanged);
        }

        void OnDisable()
        {
            btn_Skirmish.onClick.RemoveAllListeners();
            btn_Back.onClick.RemoveAllListeners();
            inputField_Seed.onValueChanged.RemoveAllListeners();
        }

        void OnSeedInputChanged(string text)
        {
            if (text.Length == 0) return;

            string newText = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]))
                {
                    newText += text[i];
                }
            }

            if (text != newText) inputField_Seed.text = newText;
        }

        IEnumerator OnSkirmishButtonClicked()
        {
            base.parentUI.Close();
            
            var levelSettingsMono = FindObjectOfType<LevelSettingsMono>();
            if (levelSettingsMono == false)
            {
                levelSettingsMono = new GameObject("LevelSettingsMono").AddComponent<LevelSettingsMono>();
                Debug.LogError("Couldn't find LevelSettingsMono");
            }

            FetchLevelSettings(levelSettingsMono.levelSettings);

            yield return new WaitForSeconds(base.parentUI.animationDuration);
            SceneLoader.instance.LoadScene(sceneList.GetSceneByContainingSceneName("GameScene"), SceneLoadSettings.GetDefault());
            SceneLoader.instance.sceneLoadCompleted -= UnloadMainMenu;
            SceneLoader.instance.sceneLoadCompleted += UnloadMainMenu;
        }
        
        /*
         * 
            public const float TIGHTNESS = 0f;
            public const float NODE_RADIUS = 0.5f;
            public const float DISTANCE_MULTIPLIER = 4;
            public const float TARGET_DISTANCE_BETWEEN_NODES = DISTANCE_MULTIPLIER * NODE_RADIUS;
            public const float LINK_DISTANCE = (DISTANCE_MULTIPLIER * 2) * NODE_RADIUS;
            public const float SAME_DIRECTION_CUT_THRESHOLD = 0.8f;
            
            public static readonly Vec2 RegionSize = new Vec2(30, 30);
            
            public int seed;
            public Vec2 regionSize;
            public MapSize mapSize;
            public float tightness;
            public float nodeRadius;
            public float distanceMultiplier;
            public float targetDistanceBetweenNodes;
            public float linkDistance;
            public float sameDirectionCutThreshold;

         */

        void FetchLevelSettings(LevelSettings levelSettings)
        {
            FetchSeed(levelSettings);
            FetchMapSize(levelSettings);
            FetchHostileCount(levelSettings);
        }

        void FetchSeed(LevelSettings levelSettings)
        {
            var txtSeed = inputField_Seed.text;
            if (int.TryParse(txtSeed, out var seed) == false) seed = (int)(uint)(XIVRandom.value * uint.MaxValue);
            levelSettings.levelGenerationSettings.seed = seed;
        }

        void FetchMapSize(LevelSettings levelSettings)
        {
            MapSize mapSize = MapSize.Small;
            int idx = radioButtonGroup.GetSelectedIndex();
            if (idx == -1) return;
            Array values = Enum.GetValues(typeof(MapSize));
            levelSettings.levelGenerationSettings.mapSize = (MapSize)values.GetValue(idx);
        }

        void FetchHostileCount(LevelSettings levelSettings)
        {
            levelSettings.hostileUnits = (int)hostileCountSlider.value;
        }

        void UnloadMainMenu(SceneSO loadedScene, SceneLoadSettings loadedSettings)
        {
            SceneLoader.instance.sceneLoadCompleted -= UnloadMainMenu;
            SceneLoader.instance.UnloadScene(sceneList.GetSceneByContainingSceneName("MainMenu"), SceneUnloadSettings.GetDefault());
        }

        void OnBackButtonClicked()
        {
            parentUI.SwitchPage(parentUI.mainPage);
        }
    }
}