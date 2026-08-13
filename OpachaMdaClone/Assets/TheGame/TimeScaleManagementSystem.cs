using System;
using UnityEngine;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class TimeScaleManagementSystem : XIV.Ecs.System
    {
        readonly Filter<SliderComp> timeScaleSliderFilter = new Filter<SliderComp>().Tag<TimeScaleSliderTag>();
        readonly Filter<SliderComp, NumericTextComp> sliderNumericTextFilter = null;

        public override void Start()
        {
            timeScaleSliderFilter.ForEach((Entity sliderEntity, ref SliderComp sliderComp) =>
            {
                sliderComp.max = sliderComp.slider.maxValue;
                sliderComp.min = sliderComp.slider.minValue;
                sliderComp.value = sliderComp.slider.value;
                XTime.timeScale = sliderComp.value;
            });
        }

        public override void Update()
        {
            timeScaleSliderFilter.ForEach((Entity sliderEntity, ref SliderComp sliderComp) =>
            {
                if (sliderComp.IsSync()) return;
                
                sliderComp.max = sliderComp.slider.maxValue;
                sliderComp.min = sliderComp.slider.minValue;
                sliderComp.value = sliderComp.slider.value;
                XTime.timeScale = sliderComp.value;
            });
            sliderNumericTextFilter.ForEach((ref SliderComp sliderComp, ref NumericTextComp numericTextComp) =>
            {
                switch (numericTextComp.displayType)
                {
                    case NumericTextDisplayType.Exact:
                        numericTextComp.txt.text = sliderComp.value.ToString("F2");
                        break;
                    case NumericTextDisplayType.Percent:
                        var v = XIVMathf.Normalize(sliderComp.value, sliderComp.min, sliderComp.max) * 100f;
                        numericTextComp.txt.text = v.ToString("F2") + '%';
                        break;
                    case NumericTextDisplayType.Normalized01:
                        numericTextComp.txt.text = XIVMathf.Normalize(sliderComp.value, sliderComp.min, sliderComp.max).ToString("F2");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}