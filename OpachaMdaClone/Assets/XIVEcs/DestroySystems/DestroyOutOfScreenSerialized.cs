using System;
using UnityEngine;

namespace XIV.Ecs
{
    [Serializable]
    public struct DestroyOutOfScreenComp : IComponent
    {
        public Vector3 boundSize;
    }
    
    public class DestroyOutOfScreenSerialized : SerializedComponent<DestroyOutOfScreenComp>
    {
        public Vector3 boundSize;
    }
}