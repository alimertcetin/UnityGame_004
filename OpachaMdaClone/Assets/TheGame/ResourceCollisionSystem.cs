using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIVEcsUnityIntegration.Extensions;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct ReturnToPoolTag : ITag { }
    
    public class ResourceCollisionSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, TransferableResourceComp> resourceFilter = new Filter<PositionComp, TransferableResourceComp>().ExcludeTag<ReturnToPoolTag>();
        readonly Filter<TransferableResourceComp, PooledComp> pooledResourceFilter = new  Filter<TransferableResourceComp, PooledComp>().Tag<ReturnToPoolTag>();
        readonly Filter<ScaleComp, ResourceComp, NodeResourceCollisionComp> nodeResourceCollisionFilter = null;
        readonly ConnectionDB connectionDB = null;
        readonly AssetReferences assetReferences = null;
        readonly HashSet<Entity> evaluatedEntities = new HashSet<Entity>();
        
        const float ERROR = 0.1f;
        const float RESOURCE_RADIUS = 0.25f - ERROR;

        public override void Update()
        {
            pooledResourceFilter.ForEach(ReturnToPool);
            nodeResourceCollisionFilter.ForEach(HandleResourceCollision);
            nodeResourceCollisionFilter.RemoveComponentAll<NodeResourceCollisionComp>();
            
            evaluatedEntities.Clear();
            resourceFilter.ForEach(MarkCollisions);
        }

        void ReturnToPool(Entity entity, ref TransferableResourceComp transferableResourceComp, ref PooledComp pooledComp)
        {
            connectionDB[transferableResourceComp.connectionIndex].RemoveResourceTransfer(entity, ref transferableResourceComp);
            pooledComp.releaseToPoolAction(entity);
        }

        void HandleResourceCollision(Entity nodeEntity, ref ScaleComp scaleComp, ref ResourceComp resourceComp, ref NodeResourceCollisionComp nodeResourceCollisionComp)
        {
            var attackerUnitEntity = nodeResourceCollisionComp.senderUnitEntity;
            if (connectionDB.IsTargetAlly(attackerUnitEntity, nodeEntity))
            {
                resourceComp.resourceQuantity += nodeResourceCollisionComp.quantity;
            }
            else
            {
                world.NewEntity().AddComponent(new ResourceDamageEventComp
                {
                    damagedEntity = nodeEntity,
                    attackerUnitEntity = nodeResourceCollisionComp.senderUnitEntity,
                    amount = nodeResourceCollisionComp.quantity,
                });
            }

            if (nodeEntity.HasTween() == false)
            {
                var scale = scaleComp.scale.ToVector3();
                nodeEntity.XIVTween()
                    .Scale(scale, scale * 1.1f, 0.5f, EasingFunction.EaseOutCubic, true)
                    .UseCustomDeltaTime(() => XTime.deltaTime)
                    .Start();
            }
        }

        // Resource x Resource collision
        void MarkCollisions(Entity resourceEntity, ref PositionComp positionComp, ref TransferableResourceComp transferableResourceComp)
        {
            if (evaluatedEntities.Add(resourceEntity) == false) return;
            
            ref var pair = ref connectionDB[transferableResourceComp.connectionIndex];
            int len = pair.resourceEntitiesOnConnection.Count;
            for (int i = 0; i < len; i++)
            {
                var otherEntity = pair.resourceEntitiesOnConnection[i];
                if (otherEntity == resourceEntity) continue;
                if (evaluatedEntities.Add(otherEntity) == false) continue;
                
                ref var otherResourceComp = ref otherEntity.GetComponent<TransferableResourceComp>();
                if (otherResourceComp.unitEntity == transferableResourceComp.unitEntity) continue;

                ref var otherPosition = ref otherEntity.GetComponent<PositionComp>();
                
                var distance = Vec3.Distance(positionComp.position, otherPosition.position);
                if (distance > RESOURCE_RADIUS) continue; // no collision

                if (transferableResourceComp.quantity == otherResourceComp.quantity)
                {
                    // destroy both
                    resourceEntity.AddTag<ReturnToPoolTag>();
                    otherEntity.AddTag<ReturnToPoolTag>();
                }
                else if (transferableResourceComp.quantity > otherResourceComp.quantity)
                {
                    transferableResourceComp.quantity -= otherResourceComp.quantity;
                    resourceEntity.GetComponent<TextComp>().txt.WriteScoreText(transferableResourceComp.quantity);
                    otherEntity.AddTag<ReturnToPoolTag>();
                }
                else if (transferableResourceComp.quantity < otherResourceComp.quantity)
                {
                    otherResourceComp.quantity -= transferableResourceComp.quantity;
                    otherEntity.GetComponent<TextComp>().txt.WriteScoreText(otherResourceComp.quantity);
                    resourceEntity.AddTag<ReturnToPoolTag>();
                }
            }
        }
    }
}