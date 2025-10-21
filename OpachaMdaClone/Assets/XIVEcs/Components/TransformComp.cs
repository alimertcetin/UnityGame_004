using UnityEngine;

namespace XIV.Ecs
{
    public struct TransformComp : IComponent
    {
        public Transform transform;
        public GameObjectEntity gameObjectEntity;
    }
}