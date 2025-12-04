using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public class DebugSystem : XIV.Ecs.System
    {
        readonly LevelSettings levelSettings = null;
        
        readonly Filter<SliderComp> sliderFilter;
        readonly Filter<SliderComp, SliderValueChangedComp> sliderValueChangeFilter;
        readonly Filter<ResourceComp, OccupiedNodeComp> sendResourceToPlayerFilter = new Filter<ResourceComp, OccupiedNodeComp>().Tag<SendResourceToPlayerTag>();
        readonly Filter<ResourceComp, OccupiedNodeComp> sendResourceToAllNeighborsFilter = new Filter<ResourceComp, OccupiedNodeComp>().Tag<SendResourceToAllNeighborsTag>();
        readonly ConnectionDB connectionDB;

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
            
            sendResourceToPlayerFilter.ForEach((Entity nodeEntity, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp) =>
            {
                nodeEntity.RemoveTag<SendResourceToPlayerTag>();
                using var neighborBuffer = ArrayUtils.GetBuffer<Entity>();
                int count = connectionDB.GetHostileNeighbors(nodeEntity, occupiedNodeComp.unitEntity, neighborBuffer);

                for (int i = 0; i < count; i++)
                {
                    var neighbor = neighborBuffer[i];
                    if (neighbor.HasComponent<OccupiedNodeComp>())
                    {
                        ref var neighborOccupiedNodeComp = ref neighbor.GetComponent<OccupiedNodeComp>();
                        ref var unitComp = ref neighborOccupiedNodeComp.unitEntity.GetComponent<UnitComp>();
                        if (unitComp.unitType == UnitIdLookup.UnitType.Green)
                        {
                            world.NewEntity().AddComponent(new SendResourceEventComp
                            {
                                fromEntity = nodeEntity,
                                toEntity = neighbor,
                                resourceQuantity = (int)(resourceComp.resourceQuantity * 0.5f),
                            });
                        }
                    }
                }
            });
            
            sendResourceToAllNeighborsFilter.ForEach((Entity nodeEntity, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp) =>
            {
                nodeEntity.RemoveTag<SendResourceToAllNeighborsTag>();
                
                using var neighborBuffer = ArrayUtils.GetBuffer<Entity>();
                int count = connectionDB.GetHostileNeighbors(nodeEntity, occupiedNodeComp.unitEntity, neighborBuffer);
                var sendQuantity = (int)(resourceComp.resourceQuantity / count);

                for (int i = 0; i < count; i++)
                {
                    var neighbor = neighborBuffer[i];
                    world.NewEntity().AddComponent(new SendResourceEventComp
                    {
                        fromEntity = nodeEntity,
                        toEntity = neighbor,
                        resourceQuantity = sendQuantity,
                    });
                }
            });
        }
    }
}