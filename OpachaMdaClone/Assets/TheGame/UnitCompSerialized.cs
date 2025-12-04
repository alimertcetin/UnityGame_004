using System;
using XIV.Core.Collections;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    [Serializable]
    public struct UnitComp : IComponent
    {
        public UnitIdLookup.UnitType unitType;
        [NonSerialized] public DynamicArray<Entity> occupiedNodeEntities;
        [NonSerialized] public int totalPower;
        public float smartness01;
        public Timer resourceTransferTimer;
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