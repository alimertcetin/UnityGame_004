using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public class PageUI : MonoBehaviour
    {
        [DisplayWithoutEdit] public PageParentUI parentUI;
        public RectTransform pageRoot;
        public float animationDuration = 1f;
        public EasingFunction.Ease easing = EasingFunction.Ease.Linear;

        [Button(playModeOnly:true)]
        public virtual void Open()
        {
            pageRoot.CancelTween(false);
            
            int screenWidth = Screen.width * 2;
            pageRoot.gameObject.SetActive(true);
            pageRoot.XIVTween()
                .RectTransformMove(new Vector2(-screenWidth, 0f), Vector2.zero, animationDuration, EasingFunction.GetEasingFunction(easing))
                .Start();
        }

        [Button(playModeOnly:true)]
        public virtual void Close()
        {
            pageRoot.CancelTween(false);
            
            int screenWidth = Screen.width * 2;
            pageRoot.XIVTween()
                .RectTransformMove(pageRoot.anchoredPosition, new Vector2(-screenWidth, 0f), animationDuration, EasingFunction.GetEasingFunction(easing))
                .OnComplete(() => pageRoot.gameObject.SetActive(false))
                .Start();
        }

        public virtual void OpenImmediately()
        {
            pageRoot.CancelTween(false);
            pageRoot.anchoredPosition = Vector2.zero;
            pageRoot.gameObject.SetActive(true);
        }
        
        public virtual void CloseImmediately()
        {
            pageRoot.CancelTween(false);
            int screenWidth = Screen.width;
            pageRoot.anchoredPosition = new Vector2(-screenWidth, 0f);
            pageRoot.gameObject.SetActive(false);
        }
    }
}