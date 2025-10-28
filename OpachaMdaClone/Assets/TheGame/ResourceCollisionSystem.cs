using System;
using UnityEngine;
using XIV.Ecs;

namespace TheGame
{
    public class ResourceCollisionSystem : XIV.Ecs.System
    {
        struct ReturnToPoolTag : ITag { }
        readonly Filter<ResourceComp> resourceFilter = null;
        readonly Filter<ResourceComp, PooledComp> pooledResourceFilter = new  Filter<ResourceComp, PooledComp>().Tag<ReturnToPoolTag>();
        const float RESOURCE_RADIUS = 0.125f;

        public override void Update()
        {
            HandleResourceCollision();
            pooledResourceFilter.ForEach((Entity entity, ref ResourceComp resourceComp, ref PooledComp pooledComp) =>
            {
                entity.RemoveComponent<ResourceComp>();
                pooledComp.releaseToPoolAction(entity);
            });
        }

        void HandleResourceCollision()
        {
            var resourceEntities = resourceFilter.Entities();
            int len = resourceEntities.Length;
            
            for (int i = 0; i < len; i++)
            {
                var resourceEntity1 = resourceEntities[i];
                if (resourceEntity1.HasTag<ReturnToPoolTag>()) continue;
                ref var resourceComp1 = ref resourceEntity1.GetComponent<ResourceComp>();
                ref var unitComp1 = ref resourceComp1.unitEntity.GetComponent<UnitComp>();
                for (int j = 0; j < len; j++)
                {
                    if (i == j) continue;
                    var resourceEntity2 = resourceEntities[j];
                    try
                    {
                        if (resourceEntity2.HasTag<ReturnToPoolTag>()) continue;
                        ref var resourceComp2 = ref resourceEntity2.GetComponent<ResourceComp>();
                        ref var unitComp2 = ref resourceComp2.unitEntity.GetComponent<UnitComp>();

                        // ignore same unit collision
                        if (unitComp1.unitType == unitComp2.unitType) continue;

                        var distance = Vector3.Distance(resourceComp1.resourcePosition, resourceComp2.resourcePosition);
                        if (distance > RESOURCE_RADIUS) continue;

                        if (resourceComp1.quantity == resourceComp2.quantity)
                        {
                            // destroy both
                            resourceEntity1.AddTag<ReturnToPoolTag>();
                            resourceEntity2.AddTag<ReturnToPoolTag>();
                        }

                        if (resourceComp1.quantity > resourceComp2.quantity)
                        {
                            resourceComp1.quantity -= resourceComp2.quantity;
                            resourceEntity1.GetComponent<TextComp>().txt.WriteScoreText(resourceComp1.quantity);
                            resourceEntity2.AddTag<ReturnToPoolTag>();
                        }
                        else if (resourceComp2.quantity < resourceComp1.quantity)
                        {
                            resourceComp2.quantity -= resourceComp1.quantity;
                            resourceEntity2.GetComponent<TextComp>().txt.WriteScoreText(resourceComp2.quantity);
                            resourceEntity1.AddTag<ReturnToPoolTag>();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}