using XIV.Core.DataStructures;
using XIV.Core.Utils;
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

    public struct HelpFrontierTargetChangeComp : IComponent
    {
        public Entity newNeighborEntity;
        public Entity newFrontierEntity;
        public Entity newTargetEntity;
    }
    
    public class NodeHelpFrontierSystem : XIV.Ecs.System
    {
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, NodeHelpFrontierComp> nodeHelpFrontierSendResourceFilter = new Filter<NodeComp, NodeHelpFrontierComp>().Exclude<SendResourceContinuouslyComp>().Exclude<NodeChangeTypeComp>();
        readonly Filter<OccupiedNodeComp, NodeHelpFrontierComp> nodeHelpFrontierFilter = null;
        readonly Filter<OccupiedNodeComp, SendResourceContinuouslyComp, NodeHelpFrontierComp, HelpFrontierTargetChangeComp> nodeHelpFrontierTargetChangeFilter = null;

        public override void Update()
        {
            nodeHelpFrontierSendResourceFilter.ForEach(SendResourceToFrontier);
            nodeHelpFrontierFilter.ForEach(CheckHelpValidity);
            nodeHelpFrontierTargetChangeFilter.ForEach(ChangeTarget);
        }

        void SendResourceToFrontier(Entity entity, ref NodeComp nodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            entity.AddComponent(new StartContinuousResourceTransferComp
            {
                targetEntity = nodeHelpFrontierComp.neighborEntity,
                sendInterval = assetReferences.generationConfigs[nodeComp.configIdx].duration,
            });
        }

        void CheckHelpValidity(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            if (CanChangeTarget(occupiedNodeComp.unitEntity, ref nodeHelpFrontierComp) == false) return;
            
            if (entity.HasComponent<SendResourceContinuouslyComp>()) entity.AddTag<StopContinuousResourceTransferTag>();
            entity.RemoveComponent<NodeHelpFrontierComp>();
        }

        void ChangeTarget(Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref SendResourceContinuouslyComp sendResourceContinuouslyComp, ref NodeHelpFrontierComp nodeHelpFrontierComp, ref HelpFrontierTargetChangeComp helpFrontierTargetChangeComp)
        {
            if (CanChangeTarget(occupiedNodeComp.unitEntity, ref nodeHelpFrontierComp) == false)
            {
                entity.RemoveComponent<HelpFrontierTargetChangeComp>();
                return;
            }
            
            if (nodeHelpFrontierComp.targetChangeDelayTimer.Update(XTime.deltaTime) == false) return;
            nodeHelpFrontierComp.targetChangeDelayTimer.Restart();
            sendResourceContinuouslyComp.toEntity = helpFrontierTargetChangeComp.newNeighborEntity;
            nodeHelpFrontierComp.neighborEntity = helpFrontierTargetChangeComp.newNeighborEntity;
            nodeHelpFrontierComp.frontierEntity = helpFrontierTargetChangeComp.newFrontierEntity;
            nodeHelpFrontierComp.targetEntity = helpFrontierTargetChangeComp.newTargetEntity;
            entity.AddTag<AddResourceTransferIndicatorTag>();
            entity.RemoveComponent<HelpFrontierTargetChangeComp>();
        }

        bool CanChangeTarget(Entity unitEntity, ref NodeHelpFrontierComp nodeHelpFrontierComp)
        {
            return connectionDB.IsTargetAlly(unitEntity, nodeHelpFrontierComp.frontierEntity) == false
                   || connectionDB.IsTargetAlly(unitEntity, nodeHelpFrontierComp.neighborEntity) == false
                   || connectionDB.IsTargetAlly(unitEntity, nodeHelpFrontierComp.targetEntity);
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