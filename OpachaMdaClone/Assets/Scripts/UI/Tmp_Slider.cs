using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame
{
    public class Tmp_Slider : MonoBehaviour
    {
        public Slider slider;
        public TMP_Text txt;

        void OnEnable()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            OnSliderValueChanged(slider.value);
        }

        void OnDisable() => slider.onValueChanged.RemoveListener(OnSliderValueChanged);

        void OnSliderValueChanged(float val)
        {
            txt.text = val.ToString(CultureInfo.InvariantCulture);
        }

        void OnValidate()
        {
            slider = GetComponentInChildren<Slider>();
            txt = GetComponentInChildren<TMP_Text>();
        }
    }
}