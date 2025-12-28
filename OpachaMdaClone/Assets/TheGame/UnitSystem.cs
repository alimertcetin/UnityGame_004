using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public class UnitSystem : XIV.Ecs.System
    {
        readonly Filter<UnitComp> unitFilter = null;
        readonly Filter<TransferableResourceComp> transferableResourceFilter = null;

        public override void Start()
        {
            unitFilter.ForEach((ref UnitComp unitComp) =>
            {
                unitComp.resourceTransferTimer = new Timer(XIVMathf.Lerp(4f, 2f, unitComp.smartness01));
            });
        }

        public override void Update()
        {
            unitFilter.ForEach(UpdateResourceTransferTimer);
            unitFilter.ForEach(UpdateUnitPower);
        }

        void UpdateUnitPower(Entity entity, ref UnitComp unitComp)
        {
            float power = 0f;
            var count = unitComp.occupiedNodeEntities.Count;
            for (var i = 0; i < count; i++)
            {
                ref var occupiedNodeEntity = ref unitComp.occupiedNodeEntities[i];
                ref var resourceComp = ref occupiedNodeEntity.GetComponent<ResourceComp>();
                power += resourceComp.resourceQuantity;
            }
            
            transferableResourceFilter.ForEach((ref TransferableResourceComp transferableResourceComp) =>
            {
                if (transferableResourceComp.unitEntity == entity)
                {
                    power += transferableResourceComp.quantity;
                }
            });

            unitComp.totalPower = (int)power;
        }

        void UpdateResourceTransferTimer(Entity entity, ref UnitComp unitComp)
        {
            if (unitComp.resourceTransferTimer.IsDone) unitComp.resourceTransferTimer.Restart();
            unitComp.resourceTransferTimer.Update(XTime.deltaTime);
        }
    }
}