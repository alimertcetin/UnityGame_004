using UnityEngine;
using XIV.Ecs;

namespace TheGame
{
    public struct NodeChangeTypeComp : IComponent
    {
        public float penalty;
        public int newConfig;
    }
    
    public class NodeTypeChangeSystem : XIV.Ecs.System
    {
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeComp, ResourceComp, OccupiedNodeComp, NodeChangeTypeComp> nodeTypeChangeFilter = null;

        public override void Update()
        {
            nodeTypeChangeFilter.ForEach(EvaluateTypeChange);
        }

        void EvaluateTypeChange(Entity entity, ref NodeComp nodeComp, ref ResourceComp resourceComp, ref OccupiedNodeComp occupiedNodeComp, ref NodeChangeTypeComp nodeChangeTypeComp)
        {
            if (nodeComp.configIdx == nodeChangeTypeComp.newConfig)
            {
                // node already has the config
                entity.RemoveComponent<NodeChangeTypeComp>();
                return;
            }
            
            var newQuantity = resourceComp.resourceQuantity - nodeChangeTypeComp.penalty;
            // We can't change type yet
            if (newQuantity <= 0f) return;
            
            resourceComp.resourceQuantity = newQuantity;
            entity.RemoveComponent<NodeChangeTypeComp>();
            entity.AddTag<UpdateResourceQuantityTextTag>();
            var shieldPoints = assetReferences.generationConfigs[nodeChangeTypeComp.newConfig].shieldPoints;
            if (shieldPoints > 0)
            {
                entity.AddComponent(new AddShieldComp
                {
                    max = shieldPoints,
                    current = 0f
                });
            }
            else
            {
                entity.AddTag<RemoveShieldEventTag>();
            }
            nodeComp.configIdx = nodeChangeTypeComp.newConfig;
            
            var particleEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeTypeChangeParticle, entity.GetComponent<PositionComp>().position, Quaternion.identity);
            var particleSystem = particleEntity.GetTransform().GetComponent<ParticleSystem>().main;
            particleSystem.startColor = new ParticleSystem.MinMaxGradient(UnitIdLookup.GetColor(occupiedNodeComp.unitEntity.GetComponent<UnitComp>().unitType));
            particleEntity.AddComponent(new CallLaterComp
            {
                timer = 1f,
                action = (e) => e.Destroy(),
            });
        }
    }
}