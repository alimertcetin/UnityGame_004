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
                unitComp.resourceTransferTimer = new Timer(XIVMathf.Lerp(4f, 3f, unitComp.smartness01));
            });
        }

        public override void Update()
        {
            unitFilter.ForEach((Entity entity, ref UnitComp unitComp) =>
            {
                // update resource transfer timer
                if (unitComp.resourceTransferTimer.IsDone) unitComp.resourceTransferTimer.Restart();
                unitComp.resourceTransferTimer.Update(XTime.deltaTime);
                
                // update unit power
                float power = 0f;
                var count = unitComp.occupiedNodeEntities.Count;
                for (var i = 0; i < count; i++)
                {
                    ref var occupiedNodeEntity = ref unitComp.occupiedNodeEntities[i];
                    ref var resourceComp = ref occupiedNodeEntity.GetComponent<ResourceComp>();
                    power += resourceComp.resourceQuantity;
                }

                unitComp.totalPower = (int)power;
            });
            
            // update unit power by using transferable resources
            transferableResourceFilter.ForEach((ref TransferableResourceComp transferableResourceComp) =>
            {
                ref var unitComp = ref transferableResourceComp.unitEntity.GetComponent<UnitComp>();
                unitComp.totalPower += transferableResourceComp.quantity;
            });
        }
    }
}