using UnityEngine;
using XIV.Ecs;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    // TODO: Make it an event comp
    public struct NodeChangeTypeEventComp : IComponent
    {
        public Entity nodeEntity;
        public Entity unitEntity; // owner unit when event fired
        public float penalty;
        public int newConfig;
    }
    
    public class NodeTypeChangeSystem : XIV.Ecs.System
    {
        readonly AssetReferences assetReferences = null;
        readonly Filter<NodeChangeTypeEventComp> nodeTypeChangeFilter = null;

        public override void Update()
        {
            nodeTypeChangeFilter.ForEach(EvaluateTypeChange);
        }

        void EvaluateTypeChange(Entity entity, ref NodeChangeTypeEventComp nodeChangeTypeEventComp)
        {
            entity.Destroy();
            ref var nodeComp = ref nodeChangeTypeEventComp.nodeEntity.GetComponent<NodeComp>();
            if (nodeComp.configIdx == nodeChangeTypeEventComp.newConfig) return;
            
            ref var resourceComp = ref nodeChangeTypeEventComp.nodeEntity.GetComponent<ResourceComp>();
            
            var newQuantity = resourceComp.resourceQuantity - nodeChangeTypeEventComp.penalty;
            // We can't change type yet
            if (newQuantity <= 0f) return;
            
            resourceComp.resourceQuantity = newQuantity;
            nodeComp.isChangingType = false;
            
            var shieldPoints = assetReferences.generationConfigs[nodeChangeTypeEventComp.newConfig].shieldPoints;
            if (shieldPoints > 0)
            {
                float prevShieldPoints = 0f;
                if (nodeChangeTypeEventComp.nodeEntity.HasComponent<ShieldComp>()) prevShieldPoints = nodeChangeTypeEventComp.nodeEntity.GetComponent<ShieldComp>().current;
                world.NewEntity().AddComponent(new AddShieldEventComp
                {
                    targetEntity = nodeChangeTypeEventComp.nodeEntity,
                    max = shieldPoints,
                    current = prevShieldPoints,
                });
            }
            else
            {
                world.NewEntity().AddComponent(new RemoveShieldEventComp
                {
                    targetEntity = nodeChangeTypeEventComp.nodeEntity,
                });
            }
            nodeComp.configIdx = nodeChangeTypeEventComp.newConfig;

            ref var positionComp = ref nodeChangeTypeEventComp.nodeEntity.GetComponent<PositionComp>();
            var particleEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeTypeChangeParticle, positionComp.position.ToVector3(), Quaternion.identity);
            var particleSystem = particleEntity.GetTransform().GetComponent<ParticleSystem>().main;
            particleSystem.startColor = new ParticleSystem.MinMaxGradient(UnitIdLookup.GetColor(nodeChangeTypeEventComp.unitEntity.GetComponent<UnitComp>().unitType));
            particleEntity.AddComponent(new CallLaterComp
            {
                timer = 1f,
                action = (e) => e.Destroy(),
            });
        }
    }
}