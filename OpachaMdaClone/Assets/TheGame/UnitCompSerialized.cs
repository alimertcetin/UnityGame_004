using System;
using XIV.Core.Collections;
using XIV.Ecs;

namespace TheGame
{
    [Serializable]
    public struct UnitComp : IComponent
    {
        public UnitIdLookup.UnitType unitType;
        // TODO : UnitComp -> Remove GenerationStepSO[]
        public GenerationStepSO[] generationConfigs;
        [NonSerialized] public DynamicArray<Entity> occupiedNodeEntities;
        [NonSerialized] public int totalPower;
    }
    
    public class UnitCompSerialized : SerializedComponent<UnitComp>
    {
        public override void AddComponentForEntity(Entity entity)
        {
            component.occupiedNodeEntities = new DynamicArray<Entity>();
            base.AddComponentForEntity(entity);
        }
    }
}