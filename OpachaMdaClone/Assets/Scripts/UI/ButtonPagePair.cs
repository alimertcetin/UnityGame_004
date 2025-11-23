using UnityEngine.UI;

namespace TheGame
{
    [System.Serializable]
    public struct ButtonPagePair
    {
        public Button btn; // when this button clicked
        public PageUI page; // this page should open
    }
}