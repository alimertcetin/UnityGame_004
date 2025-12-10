using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct NodeHelpFrontierComp : IComponent
    {
        public Entity neighborEntity; // neighbor of the node
        public Entity frontierEntity; // closest ally to target
        public Entity targetEntity; // target we aim to capture
    }
    
    public class NodeHelpFrontierSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<OccupiedNodeComp, NodeDecisionComp, PathFinderComp> addHelpFrontierFilter = new Filter<OccupiedNodeComp, NodeDecisionComp, PathFinderComp>().Exclude<NodeHelpFrontierComp>();
        readonly Filter<NodeComp, OccupiedNodeComp, NodeHelpFrontierComp> nodeHelpFrontierSendResourceFilter = new Filter<NodeComp, OccupiedNodeComp, NodeHelpFrontierComp>().Exclude<SendResourceContinuouslyComp>();
        readonly Filter<OccupiedNodeComp, NodeHelpFrontierComp, PathFinderComp> nodeHelpFrontierFilter = null;

        public override void Update()
        {
            addHelpFrontierFilter.ForEach(AddHelpFrontier);
            nodeHelpFrontierSendResourceFilter.ForEach(SendResourceToFrontier);
            nodeHelpFrontierFilter.ForEach(CheckHelpValidity);
        }

        void AddHelpFrontier(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp, ref PathFinderComp pathFinderComp)
        {
            if (nodeDecisionComp.decisionType != DecisionType.HelpFrontier) return;
            
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            int count = connectionDB.GetHostileAndNeutralPairs(entity, occupiedNodeComp.unitEntity, indexBuffer);
            // node is already frontier. Can't help to anyone.
            // This should not happen actually. NodeDecisionSystem should take this into consideration.
            if (count > 0) return;
            
            var path = pathFinderComp.path;
            var pathLength = path.Count;
            if (pathLength < 2) return; // Path is too short.

            // var self = path[0]; // current entity  // first is current entity, second is the closest neighbor
            var neighbor = path[1];
            var frontier = path[pathLength - 2];
            var target = path[pathLength - 1];

            if (CanHelpFrontier(entity, occupiedNodeComp.unitEntity, frontier, neighbor, target) == false) return;

            entity.AddComponent(new NodeHelpFrontierComp
            {
                neighborEntity = neighbor,
                frontierEntity = frontier,
                targetEntity = target,
            });
        }

        void SendResourceToFrontier(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            if (nodeComp.isChangingType) return;
            world.NewEntity().AddComponent(new StartContinuousResourceTransferEventComp
            {
                fromUnitEntity = occupiedNodeComp.unitEntity,
                fromEntity = entity,
                targetEntity = nodeHelpFrontierComp.neighborEntity,
            });
        }

        void CheckHelpValidity(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp, ref PathFinderComp pathFinderComp)
        {
            if (CanHelpFrontier(entity, occupiedNodeComp.unitEntity, nodeHelpFrontierComp.frontierEntity, nodeHelpFrontierComp.neighborEntity, nodeHelpFrontierComp.targetEntity)) return;

            entity.RemoveComponent<SendResourceContinuouslyComp>();
            entity.RemoveComponent<NodeHelpFrontierComp>();
        }

        bool CanHelpFrontier(Entity nodeEntity, Entity unitEntity, Entity frontierEntity, Entity neighborEntity, Entity targetEntity)
        {
            if (nodeEntity == frontierEntity || nodeEntity == neighborEntity || nodeEntity == targetEntity) return false;
            
            return connectionDB.IsTargetAlly(unitEntity, frontierEntity)
                   && connectionDB.IsTargetAlly(unitEntity, neighborEntity)
                   && connectionDB.IsTargetAlly(unitEntity, targetEntity) == false;
        }

        // void ChangeTarget(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp, ref PathFinderComp pathFinderComp)
        // {
        //     if (CanHelpFrontier(entity, occupiedNodeComp.unitEntity, nodeHelpFrontierComp.frontierEntity, nodeHelpFrontierComp.neighborEntity, nodeHelpFrontierComp.targetEntity)) return;
        //     if (nodeHelpFrontierComp.targetChangeDelayTimer.Update(XTime.deltaTime) == false) return;
        //     nodeHelpFrontierComp.targetChangeDelayTimer.Restart();
        //
        //     var path = pathFinderComp.path;
        //     var pathLength = path.Count;
        //     var neighbor = path[1];
        //     var frontier = path[pathLength - 2];
        //     var target = path[pathLength - 1];
        //     
        //     if (entity.HasComponent<SendResourceContinuouslyComp>())
        //     {
        //         ref var sendResourceContinuouslyComp = ref entity.GetComponent<SendResourceContinuouslyComp>();
        //         sendResourceContinuouslyComp.toEntity = neighbor;
        //     }
        //     nodeHelpFrontierComp.neighborEntity = neighbor;
        //     nodeHelpFrontierComp.frontierEntity = frontier;
        //     nodeHelpFrontierComp.targetEntity = target;
        // }
    
        // bool IsRouteSafe(XIVMemory<Entity> path, Entity unitEntity, out float normalizedHostilesOnPath)
        // {
        //     using var entityBuffer = ArrayUtils.GetBuffer<Entity>();
        //     int totalNeighborCount = 0;
        //     int totalHostileCount = 0;
        //     int len = path.Length - 1; // last is target
        //     for (int i = 0; i < len; i++)
        //     {
        //         totalNeighborCount += connectionDB.GetAllNeighbors(path[i], entityBuffer);
        //         totalHostileCount += connectionDB.GetHostileNeighbors(path[i], unitEntity, entityBuffer);
        //     }
        //     normalizedHostilesOnPath = totalHostileCount / (float)totalNeighborCount;
        //     return normalizedHostilesOnPath < 0.25f && len > 0;
        // }
    }
}