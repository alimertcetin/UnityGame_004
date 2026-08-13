using UnityEngine;

namespace TheGame
{
    public class XIVRadioButtonGroup : MonoBehaviour
    {
        [SerializeField] XIVRadioButton[] radioButtons;

        void Awake()
        {
            var len = radioButtons.Length;
            for (int i = 0; i < len; i++)
            {
                radioButtons[i].Init(this, i);
            }

            for (int i = 0; i < len; i++)
            {
                radioButtons[i].Deselect();
            }
            radioButtons[0].Select();
        }

        public void OnRadioButtonSelected(XIVRadioButton radioButton, int index)
        {
            var len = radioButtons.Length;
            for (int i = 0; i < len; i++)
            {
                if (i == index) continue;
                radioButtons[i].Deselect();
            }
        }

        public int GetSelectedIndex()
        {
            var len = radioButtons.Length;
            for (int i = 0; i < len; i++)
            {
                if (radioButtons[i].isSelected) return i;
            }

            return -1;
        }

        public XIVRadioButton GetSelected()
        {
            int idx = GetSelectedIndex();
            if (idx == -1) return null;
            return radioButtons[idx];
        }
    }
}