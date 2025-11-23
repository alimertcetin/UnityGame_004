using XIV.Ecs;

namespace TheGame
{
    public class DebugSystem : XIV.Ecs.System
    {
        LevelSettings levelSettings = null;
        
        Filter<SliderComp> sliderFilter;
        Filter<SliderComp, SliderValueChangedComp> sliderValueChangeFilter;

        public override void Start()
        {
            sliderFilter.ForEach((ref SliderComp sliderComp) =>
            {
                sliderComp.value = levelSettings.timeScale;
            });
        }


        public override void Update()
        {
            sliderValueChangeFilter.ForEach((ref SliderComp sliderComp, ref SliderValueChangedComp sliderValueChangedComp) =>
            {
                sliderComp.value = sliderValueChangedComp.value;
                levelSettings.timeScale = sliderValueChangedComp.value;
                XTime.timeScale = sliderValueChangedComp.value;
            });
        }
    }
}