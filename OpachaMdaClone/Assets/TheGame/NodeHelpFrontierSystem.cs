using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct NodeHelpFrontierComp : IComponent
    {
        public Entity neighborEntity; // neighbor of the node
        public Entity frontierEntity; // closest ally to target
        public Entity targetEntity; // target we aim to capture
        public Timer targetChangeDelayTimer;
    }

    public struct HelpFrontierTargetChangeEventComp : IComponent
    {
        public Entity ownerEntity;
        public Entity newNeighborEntity;
        public Entity newFrontierEntity;
        public Entity newTargetEntity;
    }

    public struct FrontierHelperComp : IComponent
    {
        
    }
    
    public class NodeHelpFrontierSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<OccupiedNodeComp, NodeDecisionComp, PathFinderComp, FrontierHelperComp> addHelpFrontierFilter = new Filter<OccupiedNodeComp, NodeDecisionComp, PathFinderComp, FrontierHelperComp>().Exclude<NodeHelpFrontierComp>();
        
        readonly Filter<NodeComp, NodeHelpFrontierComp> nodeHelpFrontierSendResourceFilter = new Filter<NodeComp, NodeHelpFrontierComp>().Exclude<SendResourceContinuouslyComp>().Exclude<NodeChangeTypeComp>();
        readonly Filter<OccupiedNodeComp, NodeHelpFrontierComp> nodeHelpFrontierFilter = null;
        readonly Filter<HelpFrontierTargetChangeEventComp> nodeHelpFrontierTargetChangeFilter = null;

        public override void Update()
        {
            addHelpFrontierFilter.ForEach(AddHelpFrontier);
            nodeHelpFrontierSendResourceFilter.ForEach(SendResourceToFrontier);
            nodeHelpFrontierFilter.ForEach(CheckHelpValidity);
            nodeHelpFrontierTargetChangeFilter.ForEach(ChangeTarget);
        }

        void AddHelpFrontier(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp, ref PathFinderComp pathFinderComp, ref FrontierHelperComp frontierHelperComp)
        {
            using var indexBuffer = ArrayUtils.GetBuffer<int>();
            int count = connectionDB.GetHostileAndNeutralPairs(entity, occupiedNodeComp.unitEntity, indexBuffer);
            if (count > 0)
            {
                // node is already frontier. Can't help to anyone.
                // This should not happen actually. NodeDecisionSystem should take this into consideration.
                return;
            }
            
            var path = pathFinderComp.path;
            var pathLength = path.Count;
            if (pathLength < 2)
            {
                return; // Path is too short.
            }

            // var self = path[0]; // current entity  // first is current entity, second is the closest neighbor
            var neighbor = path[1];
            var frontier = path[pathLength - 2];
            var target = path[pathLength - 1];

            if (CanHelpFrontier(entity, occupiedNodeComp.unitEntity, frontier, neighbor, target) == false)
            {
                return;
            }

            if (entity.HasComponent<NodeHelpFrontierComp>())
            {
                world.NewEntity().AddComponent(new HelpFrontierTargetChangeEventComp
                {
                    ownerEntity = entity,
                    newNeighborEntity = neighbor,
                    newFrontierEntity = frontier,
                    newTargetEntity = target,
                });
            }
            else
            {
                entity.AddComponent(new NodeHelpFrontierComp
                {
                    neighborEntity = neighbor,
                    frontierEntity = frontier,
                    targetEntity = target,
                    targetChangeDelayTimer = new Timer(XIVMathf.Max(0.1f, occupiedNodeComp.unitEntity.GetComponent<UnitComp>().smartness01))
                });
            }
        }

        void SendResourceToFrontier(Entity entity, ref NodeComp nodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            world.NewEntity().AddComponent(new StartContinuousResourceTransferEventComp
            {
                fromEntity = entity,
                targetEntity = nodeHelpFrontierComp.neighborEntity,
                sendInterval = assetReferences.generationConfigs[nodeComp.configIdx].duration,
            });
        }

        void CheckHelpValidity(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            if (CanChangeTarget(entity, occupiedNodeComp.unitEntity, ref nodeHelpFrontierComp) == false) return;
            
            entity.RemoveComponent<SendResourceContinuouslyComp>();
            entity.RemoveComponent<NodeHelpFrontierComp>();
            entity.RemoveComponent<FrontierHelperComp>();
        }

        void ChangeTarget(Entity entity, ref HelpFrontierTargetChangeEventComp helpFrontierTargetChangeComp)
        {
            entity.Destroy();
            if (helpFrontierTargetChangeComp.ownerEntity.HasComponent<OccupiedNodeComp>() == false) return;
            if (helpFrontierTargetChangeComp.ownerEntity.HasComponent<NodeHelpFrontierComp>() == false) return;
            
            ref var occupiedNodeComp = ref helpFrontierTargetChangeComp.ownerEntity.GetComponent<OccupiedNodeComp>();
            ref var nodeHelpFrontierComp = ref helpFrontierTargetChangeComp.ownerEntity.GetComponent<NodeHelpFrontierComp>();
            
            if (CanChangeTarget(entity, occupiedNodeComp.unitEntity, ref nodeHelpFrontierComp) == false) return;
            if (nodeHelpFrontierComp.targetChangeDelayTimer.Update(XTime.deltaTime) == false) return;
            nodeHelpFrontierComp.targetChangeDelayTimer.Restart();

            if (helpFrontierTargetChangeComp.ownerEntity.HasComponent<SendResourceContinuouslyComp>() == false) return;
            ref var sendResourceContinuouslyComp = ref helpFrontierTargetChangeComp.ownerEntity.GetComponent<SendResourceContinuouslyComp>();
            sendResourceContinuouslyComp.toEntity = helpFrontierTargetChangeComp.newNeighborEntity;
            nodeHelpFrontierComp.neighborEntity = helpFrontierTargetChangeComp.newNeighborEntity;
            nodeHelpFrontierComp.frontierEntity = helpFrontierTargetChangeComp.newFrontierEntity;
            nodeHelpFrontierComp.targetEntity = helpFrontierTargetChangeComp.newTargetEntity;
        }

        bool CanChangeTarget(Entity nodeEntity, Entity unitEntity, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            return CanHelpFrontier(nodeEntity, unitEntity, nodeHelpFrontierComp.frontierEntity, nodeHelpFrontierComp.neighborEntity, nodeHelpFrontierComp.targetEntity) == false;
        }

        bool CanHelpFrontier(Entity nodeEntity, Entity unitEntity, Entity frontierEntity, Entity neighborEntity, Entity targetEntity)
        {
            if (nodeEntity == frontierEntity || nodeEntity == neighborEntity || nodeEntity == targetEntity) return false;
            
            return connectionDB.IsTargetAlly(unitEntity, frontierEntity)
                   && connectionDB.IsTargetAlly(unitEntity, neighborEntity)
                   && connectionDB.IsTargetAlly(unitEntity, targetEntity) == false;
        }
    
        bool IsRouteSafe(XIVMemory<Entity> path, Entity unitEntity, out float normalizedHostilesOnPath)
        {
            using var entityBuffer = ArrayUtils.GetBuffer<Entity>();
            int totalNeighborCount = 0;
            int totalHostileCount = 0;
            int len = path.Length - 1; // last is target
            for (int i = 0; i < len; i++)
            {
                totalNeighborCount += connectionDB.GetAllNeighbors(path[i], entityBuffer);
                totalHostileCount += connectionDB.GetHostileNeighbors(path[i], unitEntity, entityBuffer);
            }
            normalizedHostilesOnPath = totalHostileCount / (float)totalNeighborCount;
            return normalizedHostilesOnPath < 0.25f && len > 0;
        }
    }
}