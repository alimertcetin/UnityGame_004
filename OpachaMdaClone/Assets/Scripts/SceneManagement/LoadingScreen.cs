using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.XIVMath;

namespace TheGame.SceneManagement
{
    public class LoadingScreen : MonoBehaviour
    {
        public static bool isActive = false;
        public RectTransform loadingScreenRoot;
        public Slider slider;
        public TMP_Text txt_Percent;
        public Camera loadingCamera;
        float actualProgress;
        Coroutine fakeLoading;

        void Awake()
        {
            ToggleLoadingScreen(false);
        }

        void OnEnable()
        {
            SceneLoader.onSceneLoadingStarted += OnSceneLoadingStarted;
        }

        void OnDisable()
        {
            SceneLoader.onSceneLoadingStarted -= OnSceneLoadingStarted;
        }

        void OnSceneLoadingStarted(SceneSO sceneToLoad, SceneLoadSettings settings)
        {
            if (settings.showLoadingScreen == false) return;
            
            slider.value = 0;
            txt_Percent.text = "%0";
            RegisterSceneLoading();
            ToggleLoadingScreen(true);
            fakeLoading = StartCoroutine(FakeLoading());
        }

        IEnumerator FakeLoading()
        {
            float fakeLoadProgress = 0f;
            float max = XIVMathf.Clamp(XIVRandom.value * 4f, 0f, 2.5f);
            while (fakeLoadProgress < max)
            {
                fakeLoadProgress += XIVRandom.value * 2f * Time.deltaTime;
                var percent = fakeLoadProgress / max;
                slider.value = percent;
                txt_Percent.text = (percent * 100f).ToString("0") + "%";
                yield return null;
            }
            while (actualProgress < 1)
            {
                slider.value = actualProgress;
                txt_Percent.text = (actualProgress * 100f).ToString("0") + "%";
                yield return null;
            }

            fakeLoading = null;
            ToggleLoadingScreen(false);
        }

        void OnSceneLoading(float percent01)
        {
            actualProgress = percent01;
        }

        void OnSceneLoadComplete(SceneSO loadedScene, SceneLoadSettings settings)
        {
            UnregisterSceneLoading();
            if (fakeLoading != null) return;
            ToggleLoadingScreen(false);
        }

        void ToggleLoadingScreen(bool v)
        {
            isActive = v;
            loadingCamera.gameObject.SetActive(v);
            loadingScreenRoot.gameObject.SetActive(v);
        }

        void RegisterSceneLoading()
        {
            SceneLoader.onSceneLoading += OnSceneLoading;
            SceneLoader.onSceneLoadComplete += OnSceneLoadComplete;
        }

        void UnregisterSceneLoading()
        {
            SceneLoader.onSceneLoading -= OnSceneLoading;
            SceneLoader.onSceneLoadComplete -= OnSceneLoadComplete;
        }
    }
}
