using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XIV.Ecs;

namespace TheGame
{
    public struct SliderComp : IComponent
    {
        public float min;
        public float max;
        public float value;
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
            component.min = slider.minValue;
            component.max = slider.maxValue;
            component.value = slider.value;
            slider.onValueChanged.AddListener((v) =>
            {
               entity.AddComponent(new SliderValueChangedComp
               {
                   value = v,
               }); 
            });
            base.AddComponentForEntity(entity);
        }

        [OnReset]
        void OnReset()
        {
            slider = GetComponentInChildren<Slider>();
        }
    }
}
