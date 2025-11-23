using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public abstract class PageParentUI : MonoBehaviour
    {
        public RectTransform pageRoot;
        public PageUI mainPage;
        public ButtonPagePair[] buttonPagePairs;
        public float animationDuration = 1f;
        public EasingFunction.Ease easing = EasingFunction.Ease.Linear;
        [DisplayWithoutEdit] public PageUI currentPage;

        void Awake()
        {
            pageRoot.gameObject.SetActive(false);
            pageRoot.localScale = Vector3.zero;
        }

        void OnEnable()
        {
            var length = buttonPagePairs.Length;
            for (var i = 0; i < length; i++)
            {
                ref var buttonPagePair = ref buttonPagePairs[i];
                buttonPagePair.page.parentUI = this;
                var page = buttonPagePair.page;
                page.CloseImmediately();
                buttonPagePair.btn.onClick.AddListener(() => SwitchPage(page));
            }
        }

        void OnDisable()
        {
            var length = buttonPagePairs.Length;
            for (var i = 0; i < length; i++)
            {
                ref var buttonPagePair = ref buttonPagePairs[i];
                buttonPagePair.btn.onClick.RemoveAllListeners();
            }
        }

        public void SwitchPage(PageUI page)
        {
            if (currentPage != null) currentPage.Close();
            currentPage = page;
            currentPage.Open();
        }

        [Button(playModeOnly:true)]
        public virtual void Open()
        {
            pageRoot.CancelTween(false);
            
            SwitchPage(mainPage);
            pageRoot.gameObject.SetActive(true);
            pageRoot.XIVTween()
                .Scale(pageRoot.localScale, Vector3.one, animationDuration, EasingFunction.GetEasingFunction(easing))
                .Start();
        }

        [Button(playModeOnly:true)]
        public virtual void Close()
        {
            pageRoot.CancelTween(false);
            
            if (currentPage != null) currentPage.Close();
            pageRoot.XIVTween()
                .Wait(currentPage.animationDuration)
                .Scale(pageRoot.localScale, Vector3.zero, animationDuration, EasingFunction.GetEasingFunction(easing))
                .OnComplete(() =>
                {
                    if (pageRoot == false) return;
                    pageRoot.gameObject.SetActive(false);
                })
                .Start();
        }
    }
}