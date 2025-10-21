using System;

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