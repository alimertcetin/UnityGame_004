using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame
{
    public class XIVRadioButton : MonoBehaviour
    {
        public bool isSelected => btn_SelectionToggle.interactable == false;
        [SerializeField] TMP_Text text;
        [SerializeField] Button btn_SelectionToggle;
        [SerializeField] Image img_ButtonImage;
        [SerializeField] Sprite defaultSprite;
        [SerializeField] Sprite selectedSprite;
        XIVRadioButtonGroup owner;
        int index;

        public void Init(XIVRadioButtonGroup owner, int index)
        {
            this.owner = owner;
            this.index = index;
        }

        void OnEnable() => btn_SelectionToggle.onClick.AddListener(Select);

        void OnDisable() => btn_SelectionToggle.onClick.RemoveAllListeners();

        public void Select()
        {
            btn_SelectionToggle.interactable = false;
            img_ButtonImage.sprite = selectedSprite;
            this.owner.OnRadioButtonSelected(this, index);
        }

        public void Deselect()
        {
            btn_SelectionToggle.interactable = true;
            img_ButtonImage.sprite = defaultSprite;
        }

        void Reset()
        {
            OnValidate();
        }

        void OnValidate()
        {
            text = GetComponentInChildren<TMP_Text>();
            btn_SelectionToggle = GetComponentInChildren<Button>();
            img_ButtonImage = GetComponentInChildren<Image>();
        }
    }
}