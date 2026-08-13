using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct SliderComp : IComponent
    {
        public Slider slider;
        public float min;
        public float max;
        public float value;

        public bool IsSync()
        {
            return XIVMathf.Abs(min - slider.minValue) < XIVMathf.Epsilon &&
                   XIVMathf.Abs(max - slider.maxValue) < XIVMathf.Epsilon &&
                   XIVMathf.Abs(value - slider.value) < XIVMathf.Epsilon;
        }
    }

    public struct SliderValueChangedComp : IComponent
    {
        public float value;
    }
    
    public class SliderCompSerialized : SerializedComponent<SliderComp>
    {
        [SerializeField] Slider slider;

        public override void AddComponentForEntity(Entity entity)
        {
            component.slider = slider;
            component.min = slider.minValue;
            component.max = slider.maxValue;
            component.value = slider.value;
            base.AddComponentForEntity(entity);
        }

        [OnReset]
        void OnReset()
        {
            slider = GetComponentInChildren<Slider>();
        }
    }
}
