using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public class NodeDebugSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp, OccupiedNodeComp> occupiedNodeFilter = null;
        readonly ConnectionDB connectionDB = null;

        public override void Update()
        {
            occupiedNodeFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp) =>
            {
                using var disposable = ArrayUtils.GetBuffer(out ConnectionDB.Pair[] pairBuffer, 16);
                int count = connectionDB.GetPairs(nodeEntity, pairBuffer);
                for (var i = 0; i < count; i++)
                {
                    var opposite = pairBuffer[i].GetOpposite(nodeEntity);
                    if (opposite.HasComponent<OccupiedNodeComp>()) continue;
                    nodeEntity.AddComponent(new SendResourceContinuouslyComp
                    {
                        currentDuration = 0f,
                        duration = occupiedNodeComp.resourceGenerationSpeed,
                        toNode = opposite,
                    });
                    break;
                }
            });
        }
    }
}