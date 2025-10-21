using UnityEngine;
using XIV.Ecs;

namespace XIVEcsUnityIntegration.Extensions
{
    public static class ComponentExtensions
    {
        public static Entity XIVGetEntity(this Component component)
        {
            if (component == false) return Entity.Invalid;
            if (component.TryGetComponent(out GameObjectEntity goEntity)) return goEntity.entity;
            return Entity.Invalid;
        }
    }
}