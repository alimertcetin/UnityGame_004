using System;
using XIV.Ecs;

namespace TheGame
{
    public class NodeDecisionApplySystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp, OccupiedNodeComp, NodeDecisionComp> nodeApplyDecisionFilter = null;
        readonly ConnectionDB connectionDB = null;

        public override void Update()
        {
            nodeApplyDecisionFilter.ForEach(ApplyDecision);
        }

        void ApplyDecision(Entity entity, ref NodeComp nodeComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeDecisionComp nodeDecisionComp)
        {
            if (nodeDecisionComp.decisionChanged)
            {
                entity.RemoveComponent<SendResourceContinuouslyComp>();
            }

            switch (nodeDecisionComp.decisionType)
            {
                case DecisionType.Defend:
                    
                    if (nodeComp.configIdx != AssetReferences.DEFEND_CONFIG)
                    {
                        world.NewEntity().AddComponent(new NodeChangeTypeEventComp
                        {
                            nodeEntity = entity,
                            unitEntity = occupiedNodeComp.unitEntity,
                            penalty = 10f,
                            newConfig = AssetReferences.DEFEND_CONFIG,
                            unitEpoch = nodeComp.unitEpoch,
                        });
                    }
                    break;
                case DecisionType.Capture:
                    break;
                case DecisionType.HelpFrontier:
                    if (nodeComp.configIdx != AssetReferences.RESOURCE_GENERATOR_CONFIG)
                    {
                        world.NewEntity().AddComponent(new NodeChangeTypeEventComp
                        {
                            nodeEntity = entity,
                            unitEntity = occupiedNodeComp.unitEntity,
                            penalty = 10f,
                            newConfig = AssetReferences.RESOURCE_GENERATOR_CONFIG,
                            unitEpoch = nodeComp.unitEpoch,
                        });
                    }
                    break;
                case DecisionType.Idle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}