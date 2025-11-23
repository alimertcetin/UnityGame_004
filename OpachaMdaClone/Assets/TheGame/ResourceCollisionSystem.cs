using System;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct InitTag : ITag { }
    public struct ReturnToPoolTag : ITag { }
    public class ResourceCollisionSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, TransferableResourceComp> resourceFilter = new Filter<PositionComp, TransferableResourceComp>().ExcludeTag<ReturnToPoolTag>();
        readonly Filter<TransferableResourceComp, PooledComp> pooledResourceFilter = new  Filter<TransferableResourceComp, PooledComp>().Tag<ReturnToPoolTag>();
        readonly Filter<TransferableResourceComp> initResourceFilter = new Filter<TransferableResourceComp>().Tag<InitTag>();
        readonly Filter<TransformComp, NodeComp, NodeResourceCollisionComp> nodeResourceCollisionFilter = null;
        readonly ConnectionDB connectionDB = null;
        const float RESOURCE_RADIUS = 0.25f - ERROR;
        const float ERROR = 0.1f;

        public override void Update()
        {
            initResourceFilter.ForEach((Entity entity, ref TransferableResourceComp transferableResourceComp) =>
            {
                connectionDB[transferableResourceComp.connectionIndex].AddResourceTransfer(entity, ref transferableResourceComp);
            });
            initResourceFilter.RemoveTagAll<InitTag>();
            pooledResourceFilter.ForEach((Entity entity, ref TransferableResourceComp transferableResourceComp, ref PooledComp pooledComp) =>
            {
                connectionDB[transferableResourceComp.connectionIndex].RemoveResourceTransfer(entity, ref transferableResourceComp);
                pooledComp.releaseToPoolAction(entity);
            });
            nodeResourceCollisionFilter.ForEach(HandleResourceCollision);
            nodeResourceCollisionFilter.RemoveComponentAll<NodeResourceCollisionComp>();
            resourceFilter.ForEach(MarkCollisions);
        }

        void HandleResourceCollision(Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp, ref NodeResourceCollisionComp nodeResourceCollisionComp)
        {
            var attackerUnitEntity = nodeResourceCollisionComp.senderUnitEntity;
            if (connectionDB.IsTargetAlly(attackerUnitEntity, nodeEntity)) // ally node
            {
                nodeComp.resourceQuantity += nodeResourceCollisionComp.quantity;
            }
            else
            {
                // remaining after shield impact
                var remaining = nodeComp.shieldPoints - nodeResourceCollisionComp.quantity;
                nodeComp.shieldPoints = XIVMathf.Max(remaining, 0);
                // remaining < 0 means shield has been destroyed and there is still resources
                if (remaining < 0)
                {
                    // flip the left resource quantity so that we can apply it to node's resourceQuantity
                    remaining = -remaining;

                    nodeComp.resourceQuantity -= remaining;
                    // nodeComp.resourceQuantity <= 0 means all resources has been destroyed of the node
                    if (nodeComp.resourceQuantity <= 0)
                    {
                        // flip the send resource quantity and change the node occupation
                        nodeComp.resourceQuantity = -nodeComp.resourceQuantity;
                        nodeEntity.AddComponent(new NodeOccupyComp
                        {
                            unitEntity = attackerUnitEntity,
                        });
                    }
                }
            }
            
            // nodeEntity.AddTag<ReevaluateDecisionTag>();
            // using var entityBuffer = ArrayUtils.GetBuffer<Entity>();
            // int len = connectionDB.GetAllyNeighbors(nodeEntity, attackerUnitEntity, entityBuffer);
            // for (int i = 0; i < len; i++)
            // {
            //     entityBuffer[i].AddTag<ReevaluateDecisionTag>();
            // }
            
            transformComp.transform.CancelTween();
            transformComp.transform.XIVTween()
                .ScaleBounceOnce()
                .Start();
        }

        void MarkCollisions(Entity resourceEntity, ref PositionComp positionComp, ref TransferableResourceComp transferableResourceComp)
        {
            ref var pair = ref connectionDB[transferableResourceComp.connectionIndex];
            int len = pair.resourceEntitiesOnConnection.Count;
            for (int i = 0; i < len; i++)
            {
                var otherEntity = pair.resourceEntitiesOnConnection[i];
                if (otherEntity == resourceEntity) continue;
                if (otherEntity.HasTag<ReturnToPoolTag>()) continue;
                
                ref var otherResourceComp = ref otherEntity.GetComponent<TransferableResourceComp>();
                if (otherResourceComp.unitEntity == transferableResourceComp.unitEntity) continue;
                
                var distance = Vec3.Distance(positionComp.position, positionComp.position);
                if (distance > RESOURCE_RADIUS) continue; // no collision

                if (transferableResourceComp.quantity == otherResourceComp.quantity)
                {
                    // destroy both
                    resourceEntity.AddTag<ReturnToPoolTag>();
                    otherEntity.AddTag<ReturnToPoolTag>();
                }

                if (transferableResourceComp.quantity > otherResourceComp.quantity)
                {
                    transferableResourceComp.quantity -= otherResourceComp.quantity;
                    resourceEntity.GetComponent<TextComp>().txt.WriteScoreText(transferableResourceComp.quantity);
                    otherEntity.AddTag<ReturnToPoolTag>();
                }
                else if (otherResourceComp.quantity < transferableResourceComp.quantity)
                {
                    otherResourceComp.quantity -= transferableResourceComp.quantity;
                    otherEntity.GetComponent<TextComp>().txt.WriteScoreText(otherResourceComp.quantity);
                    resourceEntity.AddTag<ReturnToPoolTag>();
                }
            }
        }
    }
}