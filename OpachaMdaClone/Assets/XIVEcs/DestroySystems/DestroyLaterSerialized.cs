using System;
using XIV.UnityEngineIntegration;

namespace XIV.Ecs
{
    [Serializable]
    public struct DestroyLaterComp : IComponent
    {
        public float lifeTime;
    }
    
    public class DestroyLaterSerialized : SerializedComponent<DestroyLaterComp>
    {
    }
}