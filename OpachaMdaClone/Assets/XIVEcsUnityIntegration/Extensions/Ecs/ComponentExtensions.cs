using UnityEngine;
using XIV.Ecs;

namespace XIVEcsUnityIntegration.Extensions
{
    public static class ComponentExtensions
    {
        public static Entity XIVGetEntity(this Component component)
        {
            if (component == false) return Entity.Invalid;
            return component.TryGetComponent(out GameObjectEntity goEntity) ? goEntity.entity : Entity.Invalid;
        }
    }
}